namespace BallisticCalculator.Models
{
    public class BallisticSolution
    {
        public double Azimuth { get; set; } // mils
        public double Elevation { get; set; } // mils
        public Ammunition SelectedCharge { get; set; }
        public double TimeOfFlight { get; set; } // seconds
        public double Range { get; set; } // meters
        public double ElevationCorrection { get; set; } // mils
        public double AzimuthCorrection { get; set; } // mils
        public double ImpactVelocity { get; set; } // m/s
        public double ImpactAngle { get; set; } // degrees
        public double MaximumHeight { get; set; } // meters
        public bool IsValid { get; set; }

        public override string ToString()
        {
            return $"Azimuth: {Azimuth:F2} mils, Elevation: {Elevation:F2} mils, Charge: {SelectedCharge?.ChargeName}, TOF: {TimeOfFlight:F2}s";
        }
    }
}