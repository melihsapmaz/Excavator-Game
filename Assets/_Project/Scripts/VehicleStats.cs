// VehicleStats.cs

using UnityEngine;

// This script defines a ScriptableObject for vehicle statistics, which can be used to configure various properties of vehicles in the game.
namespace _Project.Scripts {
    [CreateAssetMenu(fileName = "NewVehicleStats", menuName = "Vehicle/New Vehicle Stats", order = 1)]
    public class VehicleStats : ScriptableObject
    {
        [Header("Engine & Movement")]
        public float motorForce = 2000f;
        public float brakeForce = 3000f;
        public float maxSteerAngle = 30f;

        [Header("Fuel")]
        public float maxFuel = 100f;
        public float fuelConsumptionRate = 0.5f;

        
    }
}