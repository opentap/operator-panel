using OpenTap;

namespace PluginDevelopment.Gui.OperatorPanel
{
    [Display("Session Parameter")]
    public class PanelParameter
    {
        [Display("External Name")]
        public string Name { get; set; } = "";
        public string Value { get; set; } = "";
    }
}