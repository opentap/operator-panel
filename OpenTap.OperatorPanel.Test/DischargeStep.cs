using System.Diagnostics;
namespace OpenTap.OperatorPanel.Test
{
    [Display("Discharge", Group: "Demo")]
    public class DischargeStep : TestStep
    {
        public double LoadImpedance { get; set; } = 8;
        public BatteryDut Battery { get; set; }
        
        [Result]
        [Display("Discharge Time")]
        [Unit("s")]
        public double DischargeTime { get; private set; }
        
        [Display("Discharge Temperature")]
        [Unit("C")]
        [Result]
        public double Temperature { get; set; }

        public override void Run()
        {
            double time = 1.0;
            var stopwatch = Stopwatch.StartNew();

            while (Battery.V_Bat >= Battery.MinVoltage * 1.001)
            {
                Battery.ApplyLoad(LoadImpedance, time);
                Log.Info("Battery voltage: {0}", Battery.V_Bat);
            }
            DischargeTime = stopwatch.Elapsed.TotalSeconds;
            Temperature = (stopwatch.Elapsed.Ticks % 1000) * 0.01 + 30;

            UpgradeVerdict(Verdict.Pass);
        }
    }
}
