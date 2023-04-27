using System;
using System.Diagnostics;
using OpenTap;

namespace OpenTap.OperatorPanel.Test
{

    [Display("Charge", Group: "Demo")]
    public class ChargeStep : TestStep
    {
        public BatteryDut Battery { get; set; }
        public double ChargeVoltage { get; set; }
        public double ChargeGoal { get; set; } = 0.95;  
        
        [Result]
        [Display("Charge Time")]
        [Unit("s")]
        public double ChargeTime { get; private set; }
        
        [Display("Charge Temperature")]
        [Unit("C")]
        [Result]
        public double Temperature { get; set; }


        public override void Run()
        {
            var stopwatch = Stopwatch.StartNew();
            double time = 1.0;
            while (Battery.V_Bat < Math.Min(Battery.MaxVoltage, ChargeVoltage * ChargeGoal))
            {
                Battery.ApplyVoltage(ChargeVoltage, time);
                Log.Info("Battery voltage: {0}", Battery.V_Bat);
            }
            ChargeTime = stopwatch.Elapsed.TotalSeconds;
            Temperature = (stopwatch.Elapsed.Ticks % 1000) * 0.01 + 30;

            UpgradeVerdict(Verdict.Pass);

        }
    }

}
