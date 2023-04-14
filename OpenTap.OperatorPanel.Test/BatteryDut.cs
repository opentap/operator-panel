using System;
namespace OpenTap.OperatorPanel.Test
{
    [Display("Battery", Group: "Demo")]
    public class BatteryDut : Dut
    {
        public BatteryDut()
        {
            Name = "BAT";
        }
        
        [Display("Ω")]
        public double InternalResistance { get; set; } = 50.0e-3;

        [Display("Ω")]
        public double ChargeResistance { get; set; } = 1;

        
        [Display("J")]
        public double Capacity { get; set; } = 100000;

        public double Charge { get; internal set; }
        double Voltage = 0.0;
        public double V_Bat => V_empty + (V_full - V_empty) * (Charge / Capacity);

        public double MaxVoltage => V_full;
        public double MinVoltage => V_empty;
        
        double V_empty = 1.2;
        double V_full = 5;
        

        public void ApplyVoltage(double V_apply, double time)
        {
            double R_charge = InternalResistance + ChargeResistance;
            var I = (V_apply - (V_empty + (V_full - V_empty) * (Charge / Capacity))) / (R_charge);

            var addedCharge = I * time;
            Charge = Math.Min(Charge + addedCharge, Capacity);
        }
        
        public void ApplyLoad(double R_load, double time)
        {
            double R_charge = InternalResistance;
            double R_total = R_load + R_charge;

            var I = V_Bat / R_total;
            var addedCharge = -I * time;
            Charge = Math.Max(0, Charge + addedCharge);
        }
    }
}
