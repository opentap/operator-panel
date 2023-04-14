using System.Collections.Generic;

namespace PluginDevelopment.Gui.OperatorPanel
{
    public class OperatorMainPanelViewModel
    {
        public int Rows => OperatorPanelSettings.Current.Rows;
        public int Columns => OperatorPanelSettings.Current.Columns;
        public IEnumerable<OperatorPanelSetting> Items => OperatorPanelSettings.Current.OperatorUis;
    }
}