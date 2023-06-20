using System.Collections.Generic;
using System.ComponentModel;
using OpenTap;

namespace PluginDevelopment.Gui.OperatorPanel
{
    [Display("Operator Panel")] 
    public class OperatorPanelSettings : ComponentSettings<OperatorPanelSettings>
    {

        List<OperatorPanelSetting> uiPanels = new List<OperatorPanelSetting>
        {
            new OperatorPanelSetting { Name = "Panel 1" }
        };
        int rows = 1;
        int columns = 1;

        [Display("Operator Panels")]
        [Browsable(false)]
        public List<OperatorPanelSetting> OperatorUis
        {
            get
            {
                return uiPanels;
            }
            set
            {
                uiPanels = value;
            }
        }
        [Display("Rows", Order: 1, Group: "Layout")]
        public int Rows
        {
            get => rows;
            set
            {
                if (value == rows)
                    return;
                rows = value;
                OnPropertyChanged(nameof(Rows));
            }
        }
        
        [Display("Columns", Order: 1, Group: "Layout")]
        public int Columns
        {
            get => columns;
            set
            {
                if (value == columns)
                    return;
                columns = value;
                OnPropertyChanged(nameof(Columns));
            }
        }
    }
}