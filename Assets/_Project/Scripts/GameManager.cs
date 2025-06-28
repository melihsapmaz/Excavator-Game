using System;
using UnityEngine;

namespace _Project.Scripts {
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance{ get; private set; }
        
        [Header("Task Settings")]
        [Tooltip("The total number of loads required to complete the task")]
        [SerializeField] private int totalLoadsToDeliver = 5;
        
        private int _deliveredLoads = 0;

        private void Awake() {
            if(Instance && !Equals(Instance, this)) {
                Debug.LogWarning("Multiple instances of GameManager detected. Destroying the new instance.");
                Destroy(gameObject);
            } else {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        private void Start() {
            UpdateGameStateUI();
        }

        public void OnLoadDelivered() {
            _deliveredLoads++;
            Debug.Log($"Load delivered! Total delivered: {_deliveredLoads}/{totalLoadsToDeliver}");
            
            UpdateGameStateUI();
            CheckForWinCondition();
        }

        private void CheckForWinCondition() {
            if(_deliveredLoads >= totalLoadsToDeliver) {
                Debug.Log("All loads delivered! You win!");
                // Here you can trigger a win event, load a new scene, etc.
            }
        }
        
        private void UpdateGameStateUI() {
            // This method can be used to update the UI with the current game state.
            Debug.Log($"Current Game State: {_deliveredLoads}/{totalLoadsToDeliver} loads delivered.");
        }
    }
}
