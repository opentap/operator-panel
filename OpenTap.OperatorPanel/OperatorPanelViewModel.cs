using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Keysight.OpenTap.Gui;
using Keysight.OpenTap.Wpf;
using OpenTap;
using OpenTap.Diagnostic;

namespace PluginDevelopment.Gui.OperatorPanel
{
    public class OperatorPanelViewModel : INotifyPropertyChanged
    {
        readonly Stopwatch startedTimer = new Stopwatch();
        
        TestPlanStatus status;
        public string ElapsedTime => $"{startedTimer.Elapsed.Minutes} m {startedTimer.Elapsed.Seconds}.{startedTimer.Elapsed.Milliseconds/100} s";

        public TestPlanStatus Status
        {
            get => status;
            set
            {
                status = value;
                OnPropertyChanged();
            }
        }

        public ITapDockContext Context { get; set; }

        public bool AskForDutID { get; set; }

        public class DutView : INotifyPropertyChanged
        {
            readonly IMemberData member;
            readonly IDut dut;
            public string Name { get; set; }
            public string ID { get; set; }
            public DutView(IDut dut, IMemberData idMember)
            {
                ID = idMember.GetValue(dut)?.ToString() ?? "";
                this.dut = dut;
                this.member = idMember;
            }

            public void UpdateId()
            {
                member.SetValue(dut, ID);
            }
            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        
        public class PromotedResults : INotifyPropertyChanged
        {
            public string Name { get; set; }
            public string Value { get; set; }
            public ITestStep StepSource { get; set; }
            public IMemberData Member { get; set; }
            public Verdict Verdict { get; set; }
            public event PropertyChangedEventHandler PropertyChanged;

            public void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public List<DutView> DutsList { get; set; } = new List<DutView>();

        public List<PromotedResults> ResultsList { get; set; }= new List<PromotedResults>();
        
        int dutViewIndex = 0;
        public DutView CurrentDut => DutsList.Skip(dutViewIndex).FirstOrDefault() ?? DutsList.FirstOrDefault();

        
        public double DurationSecs => startedTimer.Elapsed.TotalSeconds * 10.0;
        public string Name
        {
            get => operatorPanelSetting.Name ?? "Panel";
            set
            {
                operatorPanelSetting.Name = value;
                GuiHelpers.GuiInvoke(() => OnPropertyChanged(nameof(Name)));
            }
        }
        
        void UpdatePromotedResults()
        {
            var newResults = new List<PromotedResults>();
            var allSteps = (currentPlan ?? Context.Plan).Steps.RecursivelyGetAllTestSteps(TestStepSearch.EnabledOnly);
            
            foreach (var step in allSteps)
            {
                var resultMembers = TypeData.GetTypeData(step)
                    .GetMembers()
                    .Where(x => ReflectionDataExtensions.HasAttribute<ResultAttribute>(x))
                    .ToArray();
                foreach (var r in resultMembers)
                {
                    newResults.Add(new
                        PromotedResults {
                            Name = r.GetDisplayAttribute().Name,
                            Value = "  ",
                            StepSource = step,
                            Member = r,
                            Verdict = Verdict.NotSet
                        });   
                }
                
            }

            ResultsList = newResults;
            OnPropertyChanged(nameof(ResultsList));
            var newDuts = new List<DutView>();
            var plan = (currentPlan ?? Context.Plan);
            foreach (var parameter in TypeData.GetTypeData(plan).GetMembers().OfType<IParameterMemberData>())
            {
                if (parameter.TypeDescriptor.DescendsTo(typeof(IDut)) == false) continue;
                var dutInstance = parameter.GetValue(plan) as IDut;
                if (dutInstance == null) continue;
                var idMember = TypeData.GetTypeData(dutInstance)?.GetMember(nameof(Dut.ID));
                if (idMember == null) continue;
                var newDut = new DutView(dutInstance, idMember )
                {
                    Name = parameter.Name,
                    ID = idMember.GetValue(dutInstance)?.ToString() ?? ""
                };

                newDuts.Add(newDut);
            }
            DutsList = newDuts;
            OnPropertyChanged(nameof(DutsList));

        }

        public event PropertyChangedEventHandler PropertyChanged;

        TestPlan plan;
        public void UpdateTime()
        {
            OnPropertyChanged(nameof(ElapsedTime));
            OnPropertyChanged(nameof(DurationSecs));
            if (plan != Context.Plan)
            {
                plan = Context.Plan;
                GuiHelpers.GuiInvoke(UpdatePromotedResults);
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary> Used for stopping the test plan run. </summary>
        CancellationTokenSource cancellationToken;

        class UserInputOverride : IUserInputInterface, IUserInterface
        {
            readonly Action enterDutStarted;
            readonly Action enterDutEnded;
            readonly Func<bool> enterNextDut;
            IUserInputInterface prev;
            IUserInterface prev2;

            public UserInputOverride(object prev, Action enterDutStarted,Func<bool> nextDut, Action enterDutEnded)
            {
                this.enterDutStarted = enterDutStarted;
                this.enterDutEnded = enterDutEnded;
                this.enterNextDut = nextDut;
                this.prev = prev as IUserInputInterface;
                this.prev2 = prev as IUserInterface;
            }

            readonly ManualResetEventSlim enterComplete = new ManualResetEventSlim();
            
            public void RequestUserInput(object dataObject, TimeSpan Timeout, bool modal)
            {
                enterComplete.Reset();
                if (dataObject.GetType().Name == "MetadataPromptObject")
                {
                    enterDutStarted?.Invoke();
                    try
                    {
                        while (true)
                        {
                            if (Timeout.TotalSeconds > 10000)
                                enterComplete.Wait(TapThread.Current.AbortToken);
                            else
                                enterComplete.Wait(Timeout, TapThread.Current.AbortToken);
                            if (!enterNextDut())
                            {
                                break;
                            }
                            enterComplete.Reset();
                        }
                    }
                    finally
                    {
                        enterDutEnded?.Invoke();
                    }

                }else
                    prev?.RequestUserInput(dataObject, Timeout, modal);
            }

            public void EnterComplete()
            {
                enterComplete.Set();
            }

            public void NotifyChanged(object obj, string property)
            {
                prev2?.NotifyChanged(obj, property);
            }
        }

        TestPlan currentPlan;
        Session executorSession;
        internal OperatorPanelSetting operatorPanelSetting;
        readonly List<Event> logEvents = new List<Event>();
        public IEnumerable<Event> LogEvents => logEvents;

        public void ExecuteTestPlan()
        {
            // save the current test plan XML as a byte array. 
            var xml = Context.Plan.GetCachedXml();
            if (xml == null)
            {
                var str = new MemoryStream();
                Context.Plan.Save(str);
                xml = str.ToArray();
            }

            // clear the current log events.
            logEvents.Clear();

            // start a new session to avoid interfering with settings (or resources) used by other panels.
            // overlay the component settings means that we make a copy of the current settings for the session.
            // redirect logging means that any log message inside the session will be redirected to new log listeners
            //     instead of the ones used by the default session.
            executorSession = Session.Create(SessionOptions.OverlayComponentSettings | SessionOptions.RedirectLogging);
            {
                // redirect trace listener.
                // note, that stores the log in memory, using this may cause out of memory exceptions
                // on limited systems and very large / logging test plans.
                var log = new EventTraceListener();
                log.MessageLogged += events => logEvents.AddRange(events);
                Log.AddListener(log);
                
                // load the test plan in the new session.
                currentPlan = TestPlan.Load(new MemoryStream(xml), Context.Plan.Path);

                // load the panel settings.
                var a = AnnotationCollection.Annotate(currentPlan);
                var ds = DutSettings.Current;
                foreach (var member in a.Get<IMembersAnnotation>().Members)
                {
                    var str = member.Get<IStringValueAnnotation>();
                    if (str == null) continue; // this parameter cannot be set.
                    var name = member.Get<IDisplayAnnotation>()?.Name;
                    var param = operatorPanelSetting.Parameters.FirstOrDefault(x => x.Name == name);
                    if (param == null) continue;
                    try
                    {
                        str.Value = param.Value;
                    }
                    catch
                    {
                        
                    }
                }

                // write the changes to the test plan.
                a.Write();
                
                // override the current user input interface.
                // this UserInputOverride only intercepts some specific events.
                // all other events are redirected to the default user input interface.
                var prev = UserInput.Interface as IUserInputInterface;
                var ui = new UserInputOverride(prev,
                    () =>
                    {
                        GuiHelpers.GuiInvoke(() =>
                        {
                            dutViewIndex = 0;
                            AskForDutID = true;
                            OnPropertyChanged("");
                        });
                    },
                    () =>
                    {
                        dutViewIndex++;
                        GuiHelpers.GuiInvoke(() =>
                        {
                            OnPropertyChanged(nameof(CurrentDut));
                        });
                        if (dutViewIndex >= DutsList.Count) return false;
                        return true;
                    },
                    () =>
                    {
                        GuiHelpers.GuiInvoke(() =>
                        {
                            AskForDutID = false;
                            OnPropertyChanged("");
                            UpdatePromotedResults();

                        });
                    });
                UserInput.SetInterface(ui);
                
                // promoted results are [Result] properties extracted from the test plan.
                UpdatePromotedResults();
                
                OnPropertyChanged("");
                var resultListeners = ResultSettings.Current;
                
                // setup the UI Update result listener 
                var uiListener = new OperatorResultListener();
                uiListener.TestStepRunCompleted += UiListener_OnTestStepRunCompleted;
                
                startedTimer.Restart();
                Status = TestPlanStatus.Running;
                
                // this is used to cancel test plan execution.
                cancellationToken = new CancellationTokenSource();
                
                // run the test plan.
                var runTask = currentPlan.ExecuteAsync(resultListeners.Concat(new IResultListener[] { uiListener }),
                    Array.Empty<ResultParameter>(), null, cancellationToken.Token);

                runTask.ContinueWith(t =>
                {
                    startedTimer.Stop();
                    GuiHelpers.GuiInvoke(() =>
                    {
                        if (t.IsCanceled)
                            Status = TestPlanStatus.Aborted;
                        else if (t.Result.Verdict == Verdict.Aborted)
                            Status = TestPlanStatus.Aborted;
                        else if (t.Result.Verdict > Verdict.Pass)
                            Status = TestPlanStatus.Failed;
                        else
                            Status = TestPlanStatus.Passed;
                        UserInput.SetInterface(prev);
                        executorSession.Dispose();
                        OnPropertyChanged("");
                    });
                });
                var prevSession = executorSession;
                // create a sub session before disposing the current session.
                // this is because we want to make sure executor session 
                // needed for the DutIdEntered callback
                executorSession = Session.Create(SessionOptions.None);
                prevSession.Dispose();
            }
        }


        void UiListener_OnTestStepRunCompleted(object sender, TestStepRun run)
        {
            // Update the results in the UI when a test step run is finished.
            foreach (var mem in ResultsList)
            {
                if (mem.StepSource.Id == run.TestStepId)
                {
                    mem.Value = mem.Member.GetValue(mem.StepSource)?.ToString() ?? "<null>";
                    var unit = mem.Member.GetAttribute<UnitAttribute>()?.Unit;
                    if(unit != null)
                        mem.Value += " " + unit;
                    mem.Verdict = run.Verdict;
                    
                    GuiHelpers.GuiInvokeAsync(() => mem.OnPropertyChanged(string.Empty));
                }
            }
        }

        public void StopTestPlan() => cancellationToken?.Cancel();

        public void DutIdEntered()
        {
            CurrentDut.UpdateId();
            executorSession.RunInSession(() =>
            {
                (UserInput.Interface as UserInputOverride)?.EnterComplete();
            });
        }
    }
}