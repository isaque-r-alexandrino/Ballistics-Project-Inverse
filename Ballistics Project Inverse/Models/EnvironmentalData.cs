namespace BallisticCalculator.Models
{
    public class EnvironmentalData
    {
        public double Temperature { get; set; } // Celsius
        public double Pressure { get; set; } // hPa
        public double Humidity { get; set; } // percentage
        public double WindSpeed { get; set; } // m/s
        public double WindDirection { get; set; } // degrees from North
        public double CoriolisEffect { get; set; } // boolean flag
    }
}