using UnityEngine;

public enum VehicleType {
    None,
    WheelLoader,
    Telehandler
}

// This script manages game settings such as player count and vehicle types.
namespace _Project.Scripts {
    public class GameSettings : MonoBehaviour {
        public static GameSettings Instance { get; private set; }

        public int playerCount = 1;
        public VehicleType player1Vehicle;
        public VehicleType player2Vehicle;

        private void Awake() {
            if (Instance && Instance != this) {
                Destroy(gameObject);
            }
            else {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        public void ResetSettings() {
            playerCount = 1;
            player1Vehicle = VehicleType.None;
            player2Vehicle = VehicleType.None;
        }
    }
}