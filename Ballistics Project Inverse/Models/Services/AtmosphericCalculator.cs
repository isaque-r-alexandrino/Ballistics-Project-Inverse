using System;
using BallisticCalculator.Models;

namespace BallisticCalculator.Services
{
    public class AtmosphericCalculator
    {
        private const double GAS_CONSTANT = 287.05; // J/(kg·K)
        private const double STANDARD_TEMPERATURE = 15.0; // Celsius
        private const double STANDARD_PRESSURE = 1013.25; // hPa
        private const double GRAVITY = 9.80665; // m/s²

        public double CalculateAirDensity(EnvironmentalData env)
        {
            double T = env.Temperature + 273.15; // Kelvin
            double P = env.Pressure * 100; // hPa to Pa
            return P / (GAS_CONSTANT * T);
        }

        public double CalculateSpeedOfSound(EnvironmentalData env)
        {
            double T = env.Temperature + 273.15;
            return 331.3 * Math.Sqrt(T / 273.15);
        }

        public double CalculateAirViscosity(EnvironmentalData env)
        {
            double T = env.Temperature + 273.15;
            double T0 = 273.15;
            double mu0 = 1.716e-5; // Pa·s at T0
            double S = 110.4; // Sutherland's constant

            return mu0 * Math.Pow(T / T0, 1.5) * (T0 + S) / (T + S);
        }

        public double GetDensityCorrection(EnvironmentalData env)
        {
            double standardDensity = CalculateAirDensity(new EnvironmentalData
            {
                Temperature = STANDARD_TEMPERATURE,
                Pressure = STANDARD_PRESSURE
            });

            double actualDensity = CalculateAirDensity(env);
            return actualDensity / standardDensity;
        }
    }
}