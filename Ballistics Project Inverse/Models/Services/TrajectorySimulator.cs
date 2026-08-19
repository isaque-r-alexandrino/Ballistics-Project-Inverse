using System;
using BallisticCalculator.Models;

namespace BallisticCalculator.Services
{
    public class TrajectorySimulator
    {
        private const double GRAVITY = 9.80665;
        private const double DT = 0.005; // Time step in seconds

        public TrajectoryResult SimulateTrajectory(
            double muzzleVelocity,
            double elevationRad,
            double azimuthRad,
            EnvironmentalData env,
            Ammunition ammo,
            double deltaHeight)
        {
            var atmos = new AtmosphericCalculator();
            double airDensity = atmos.CalculateAirDensity(env);

            // Initial velocities
            double vx0 = muzzleVelocity * Math.Cos(elevationRad);
            double vy0 = muzzleVelocity * Math.Sin(elevationRad);

            // Convert wind to trajectory coordinates
            double windRad = env.WindDirection * Math.PI / 180.0;
            double windEast = env.WindSpeed * Math.Sin(windRad);
            double windNorth = env.WindSpeed * Math.Cos(windRad);

            // Rotate wind to local trajectory coordinates
            double windX = windNorth * Math.Cos(azimuthRad) + windEast * Math.Sin(azimuthRad);
            double windY = -windNorth * Math.Sin(azimuthRad) + windEast * Math.Cos(azimuthRad);

            // State variables
            double time = 0;
            double x = 0, y = 0, z = deltaHeight; // Start at gun height
            double vx = vx0, vy = 0, vz = vy0;

            double maxHeight = z;
            double impactVelocity = 0;
            double impactAngle = 0;

            // Projectile properties
            double mass = ammo.ProjectileMass;
            double crossSection = Math.PI * Math.Pow(ammo.Caliber / 2.0, 2);
            double cd = ammo.DragCoefficient;

            // Simulate until impact
            while (z > 0 && time < 120) // Max 120 seconds
            {
                // Relative velocity (including wind)
                double vRelX = vx - windX;
                double vRelY = vy - windY;
                double vRelZ = vz;
                double vRel = Math.Sqrt(vRelX * vRelX + vRelY * vRelY + vRelZ * vRelZ);

                // Drag force
                double dragForce = 0.5 * airDensity * cd * crossSection * vRel * vRel;

                // Accelerations
                double ax = -dragForce / mass * vRelX / (vRel + 1e-10);
                double ay = -dragForce / mass * vRelY / (vRel + 1e-10);
                double az = -dragForce / mass * vRelZ / (vRel + 1e-10) - GRAVITY;

                // Update velocities
                vx += ax * DT;
                vy += ay * DT;
                vz += az * DT;

                // Update positions
                x += vx * DT;
                y += vy * DT;
                z += vz * DT;

                time += DT;

                // Track maximum height
                if (z > maxHeight)
                    maxHeight = z;

                // If projectile goes below ground, interpolate impact
                if (z < 0)
                {
                    double fraction = (z - vz * DT) / (vz * DT);
                    x -= vx * DT * fraction;
                    y -= vy * DT * fraction;
                    impactVelocity = Math.Sqrt(vx * vx + vy * vy + vz * vz);
                    impactAngle = Math.Atan2(vz, Math.Sqrt(vx * vx + vy * vy)) * 180.0 / Math.PI;
                    break;
                }
            }

            // Calculate horizontal range
            double range = Math.Sqrt(x * x + y * y);

            return new TrajectoryResult
            {
                Range = range,
                TimeOfFlight = time,
                MaximumHeight = maxHeight,
                ImpactVelocity = impactVelocity,
                ImpactAngle = impactAngle,
                FinalX = x,
                FinalY = y,
                FinalZ = z
            };
        }

        public class TrajectoryResult
        {
            public double Range { get; set; }
            public double TimeOfFlight { get; set; }
            public double MaximumHeight { get; set; }
            public double ImpactVelocity { get; set; }
            public double ImpactAngle { get; set; }
            public double FinalX { get; set; }
            public double FinalY { get; set; }
            public double FinalZ { get; set; }
        }
    }
}