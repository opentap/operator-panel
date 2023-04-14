using System;
using OpenTap;

namespace OpenTap.OperatorPanel.Test
{

    [Display("Charge", Group: "Demo")]
    public class ChargeStep : TestStep
    {
        public BatteryDut Battery { get; set; }
        public double ChargeVoltage { get; set; }
        public double ChargeGoal { get; set; } = 0.95;  

        public override void Run()
        {
            double time = 1.0;
            while (Battery.V_Bat < Math.Min(Battery.MaxVoltage, ChargeVoltage * ChargeGoal))
            {
                Battery.ApplyVoltage(ChargeVoltage, time);
                Log.Info("Battery voltage: {0}", Battery.V_Bat);
            }
        }
    }

}
