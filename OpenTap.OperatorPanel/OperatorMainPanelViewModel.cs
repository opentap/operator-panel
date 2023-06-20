using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace PluginDevelopment.Gui.OperatorPanel
{
    public class OperatorMainPanelViewModel : INotifyPropertyChanged
    {
        public int Rows => OperatorPanelSettings.Current.Rows;
        public int Columns => OperatorPanelSettings.Current.Columns;

        
        List<OperatorPanelSetting> deleteBuffer = new List<OperatorPanelSetting>();
        public IEnumerable<OperatorPanelSetting> Items
        {
            get
            {
                var uis = OperatorPanelSettings.Current.OperatorUis;
                int totalCount = Rows * Columns;
                bool changed = uis.Count != totalCount;
                while (uis.Count > totalCount)
                {
                    deleteBuffer.Add(uis.Last());
                    uis.RemoveAt(uis.Count - 1);
                }
                while (uis.Count < totalCount)
                {
                    if (deleteBuffer.Count > 0)
                    {
                        uis.Add(deleteBuffer.Last());
                        deleteBuffer.RemoveAt(deleteBuffer.Count - 1);
                    }
                    else
                    {
                        var panel = new OperatorPanelSetting
                        {
                            Name = "Panel " + (uis.Count + 1)
                        };
                        uis.Add(panel);
                    }
                }
                
                if (changed)
                {
                    OperatorPanelSettings.Current.OperatorUis = uis.ToList();
                    OperatorPanelSettings.Current.Save();
                }
                return OperatorPanelSettings.Current.OperatorUis;
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}