using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts {
    public class DropZone : MonoBehaviour {
        private readonly List<GameObject> _deliveredLoadsList = new();
        private readonly HashSet<GameObject> loadsInZone = new HashSet<GameObject>();

        private void OnEnable() {
            AttachmentController.OnLoadDropped += HandleLoadDropped;
        }

        private void OnDisable() {
            AttachmentController.OnLoadDropped -= HandleLoadDropped;
        }
        
        private void HandleLoadDropped(GameObject droppedLoad) {
            if (loadsInZone.Contains(droppedLoad) && !_deliveredLoadsList.Contains(droppedLoad)) {
                _deliveredLoadsList.Add(droppedLoad);
                GameManager.Instance.OnLoadDelivered();
            }
            else {
                Debug.Log("<color=red>FAILURE:</color> The dropped load is not in the DropZone or has already been delivered.");
            }
        }

        private void OnTriggerEnter(Collider other) {
            if (other.CompareTag("Load"))
            {
                loadsInZone.Add(other.gameObject);
            }
        }
        
        private void OnTriggerExit(Collider other) {
            if (other.CompareTag("Load"))
            {
                loadsInZone.Remove(other.gameObject);
            }
        }
        
        
    }
}