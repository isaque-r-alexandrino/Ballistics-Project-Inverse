using System;
using System.Collections.Generic;
using System.Linq;
using BallisticCalculator.Models;

namespace BallisticCalculator.Services
{
    public class BallisticCalculatorService
    {
        private readonly TrajectorySimulator _trajectorySimulator;
        private readonly AtmosphericCalculator _atmosCalc;

        public BallisticCalculatorService()
        {
            _trajectorySimulator = new TrajectorySimulator();
            _atmosCalc = new AtmosphericCalculator();
        }

        public BallisticSolution CalculateSolution(
            TargetData target,
            GunData gun,
            EnvironmentalData env,
            List<Ammunition> ammunitionCatalog)
        {
            var solution = new BallisticSolution();

            try
            {
                // Calculate geometry
                double deltaN = target.Northing - gun.Northing;
                double deltaE = target.Easting - gun.Easting;
                double horizontalDistance = Math.Sqrt(deltaN * deltaN + deltaE * deltaE);
                double deltaHeight = target.Altitude - gun.Altitude;

                // Calculate azimuth (in mils, 1 mil = 0.05625 degrees)
                double azimuthRad = Math.Atan2(deltaE, deltaN);
                double azimuthMils = azimuthRad * 1000; // Approximate conversion
                if (azimuthMils < 0) azimuthMils += 6400;

                // Find best charge
                var bestCharge = SelectBestCharge(horizontalDistance, ammunitionCatalog);
                if (bestCharge == null)
                {
                    solution.IsValid = false;
                    return solution;
                }

                // Calculate elevation using bisection method
                double elevationMils = SolveElevation(
                    bestCharge.MuzzleVelocity,
                    horizontalDistance,
                    deltaHeight,
                    env,
                    bestCharge,
                    azimuthRad);

                // Simulate final trajectory for detailed results
                var trajectoryResult = _trajectorySimulator.SimulateTrajectory(
                    bestCharge.MuzzleVelocity,
                    elevationMils / 1000.0, // Convert mils to radians
                    azimuthRad,
                    env,
                    bestCharge,
                    deltaHeight);

                // Calculate corrections
                double elevationCorrection = CalculateElevationCorrection(env);
                double azimuthCorrection = CalculateAzimuthCorrection(env, azimuthRad, horizontalDistance);

                // Populate solution
                solution.Azimuth = azimuthMils;
                solution.Elevation = elevationMils;
                solution.SelectedCharge = bestCharge;
                solution.TimeOfFlight = trajectoryResult.TimeOfFlight;
                solution.Range = horizontalDistance;
                solution.ElevationCorrection = elevationCorrection;
                solution.AzimuthCorrection = azimuthCorrection;
                solution.ImpactVelocity = trajectoryResult.ImpactVelocity;
                solution.ImpactAngle = trajectoryResult.ImpactAngle;
                solution.MaximumHeight = trajectoryResult.MaximumHeight;
                solution.IsValid = true;
            }
            catch (Exception ex)
            {
                solution.IsValid = false;
                throw new Exception($"Ballistic calculation failed: {ex.Message}");
            }

            return solution;
        }

        private Ammunition SelectBestCharge(double distance, List<Ammunition> catalog)
        {
            var sorted = catalog.OrderBy(a => a.MuzzleVelocity).ToList();

            foreach (var ammo in sorted)
            {
                // Check if charge can reach this distance
                if (ammo.MaxRange >= distance * 1.1) // 10% margin
                    return ammo;
            }

            return null;
        }

        private double SolveElevation(
            double muzzleVelocity,
            double targetDistance,
            double deltaHeight,
            EnvironmentalData env,
            Ammunition ammo,
            double azimuthRad)
        {
            // Bisection method with adaptive precision
            double minElevation = 0.001; // ~0.06 degrees
            double maxElevation = 1.5; // ~86 degrees
            double precision = 0.2; // 0.2 meters

            for (int iteration = 0; iteration < 60; iteration++)
            {
                double midElevation = (minElevation + maxElevation) / 2.0;

                var result = _trajectorySimulator.SimulateTrajectory(
                    muzzleVelocity,
                    midElevation,
                    azimuthRad,
                    env,
                    ammo,
                    deltaHeight);

                if (Math.Abs(result.Range - targetDistance) < precision)
                {
                    return midElevation * 1000; // Convert to mils
                }

                if (result.Range < targetDistance)
                {
                    minElevation = midElevation;
                }
                else
                {
                    maxElevation = midElevation;
                }
            }

            return ((minElevation + maxElevation) / 2.0) * 1000;
        }

        private double CalculateElevationCorrection(EnvironmentalData env)
        {
            // Temperature correction: ±0.2 mil per 10°C
            double tempCorrection = (env.Temperature - 15.0) * 0.02;

            // Pressure correction: ±0.1 mil per 10 hPa
            double pressureCorrection = (1013.25 - env.Pressure) * 0.01;

            // Humidity correction: ±0.05 mil per 20%
            double humidityCorrection = (env.Humidity - 50.0) * 0.0025;

            return tempCorrection + pressureCorrection + humidityCorrection;
        }

        private double CalculateAzimuthCorrection(EnvironmentalData env, double azimuthRad, double range)
        {
            // Crosswind deflection
            double windRad = env.WindDirection * Math.PI / 180.0;
            double crosswind = env.WindSpeed * Math.Sin(windRad - azimuthRad);

            // Wind drift: simplified model
            double windDrift = crosswind * 0.2 * Math.Sqrt(range / 1000.0);

            // Coriolis effect (simplified)
            double coriolis = 0;
            if (env.CoriolisEffect > 0)
            {
                double latitude = 45.0 * Math.PI / 180.0; // Example latitude
                double omega = 7.2921e-5; // Earth's rotation rate
                coriolis = -2 * omega * Math.Sin(latitude) * range / (1000 * 1000);
            }

            return windDrift * 1000 + coriolis;
        }
    }
}