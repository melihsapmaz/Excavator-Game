using System.Collections.Generic;
using UnityEngine;
using System;

// This script is part of the _Project namespace and handles the drop zone functionality
namespace _Project.Scripts {
    public class DropZone : MonoBehaviour {
        private readonly HashSet<GameObject> _deliveredLoadsList = new(); // Hashset to keep track of delivered loads
        private readonly HashSet<GameObject> _loadsInZone = new(); // HashSet to track loads currently in the drop zone

        private void OnEnable() {
            // Subscribe to the OnLoadDropped event from AttachmentController
            AttachmentController.OnLoadDropped += HandleLoadDropped;
        }

        private void OnDisable() {
            // Unsubscribe from the OnLoadDropped event to prevent memory leaks
            AttachmentController.OnLoadDropped -= HandleLoadDropped;
        }

        // This method handles the logic when a load is dropped in the drop zone
        private void HandleLoadDropped(GameObject droppedLoad, PlayerInputHandler.PlayerID playerID) {
            if (_loadsInZone.Contains(droppedLoad) && _deliveredLoadsList.Add(droppedLoad)) {
                // Check if the load is in the drop zone and not already delivered
                if (GameManager.Instance) {
                    // deliveredLoadsList.Add returns false if the load was already in the list
                    GameManager.Instance
                        .OnLoadDelivered(playerID); // Notify the GameManager that a load has been delivered
                }
            }
        }

        // Unity's trigger methods to detect when loads enter or exit the drop zone
        private void OnTriggerEnter(Collider other) {
            if (other.CompareTag("Load")) {
                _loadsInZone.Add(other.gameObject);
            }
        }

        private void OnTriggerExit(Collider other) {
            if (other.CompareTag("Load")) {
                _loadsInZone.Remove(other.gameObject);
            }
        }
    }
}