namespace BallisticCalculator.Models
{
    public class Ammunition
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ChargeName { get; set; }
        public double MuzzleVelocity { get; set; } // m/s
        public double MaxRange { get; set; } // meters
        public double ProjectileMass { get; set; } // kg
        public double Caliber { get; set; } // meters
        public double BallisticCoefficient { get; set; }
        public double DragCoefficient { get; set; }

        public override string ToString()
        {
            return $"{ChargeName} - V0: {MuzzleVelocity:F0} m/s, Max: {MaxRange:F0}m";
        }
    }
}