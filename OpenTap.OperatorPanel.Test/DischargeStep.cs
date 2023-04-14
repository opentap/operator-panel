namespace OpenTap.OperatorPanel.Test
{
    [Display("Charge", Group: "Demo")]
    public class DischargeStep : TestStep
    {
        public double LoadImpedance { get; set; } = 8;
        public BatteryDut Battery { get; set; }
        public override void Run()
        {
            double time = 1.0;
            while (Battery.V_Bat >= Battery.MinVoltage * 1.001)
            {
                Battery.ApplyLoad(LoadImpedance, time);
                Log.Info("Battery voltage: {0}", Battery.V_Bat);
            }
        }
    }
}
