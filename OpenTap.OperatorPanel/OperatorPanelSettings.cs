using System.Collections.Generic;
using OpenTap;

namespace PluginDevelopment.Gui.OperatorPanel
{
    [Display("Operator Panel Settings")]
    public class OperatorPanelSettings : ComponentSettings<OperatorPanelSettings>
    {
        [Display("Operator Panels")]
        public List<OperatorPanelSetting> OperatorUis { get; set; } = new List<OperatorPanelSetting>()
        {
            new OperatorPanelSetting { Name = "Panel 1" }
        };
        [Display("Rows", Order: 1, Group:"Layout")]
        public int Rows { get; set; } = 1;
        [Display("Columns", Order: 1, Group:"Layout")]
        public int Columns { get; set; } = 1;
    }
}