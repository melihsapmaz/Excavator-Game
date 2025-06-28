using System;
using UnityEngine;

namespace _Project.Scripts {
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    public class AttachmentController : MonoBehaviour
    {
        private Rigidbody _potentialLoadRigidbody;
        
        private FixedJoint _currentJoint;
        
        public static event Action<GameObject> OnLoadDropped;

        public void ToggleAttachment()
        {
            // State 1: An already carried load is being dropped.
            if (_currentJoint)
            {
                
                var droppedLoadObject = _currentJoint.connectedBody.gameObject;
                Destroy(_currentJoint);
                _currentJoint = null;
                _potentialLoadRigidbody = null; 
                
                OnLoadDropped?.Invoke(droppedLoadObject);
            }
            // State 2: A potential load is being picked up.
            else if (_potentialLoadRigidbody)
            {
                _currentJoint = gameObject.AddComponent<FixedJoint>();
                _currentJoint.connectedBody = _potentialLoadRigidbody;
                _currentJoint.breakForce = Mathf.Infinity;
                _currentJoint.breakTorque = Mathf.Infinity;
            }
            else
            {
                Debug.Log("There is no load to pick up or drop.");
            }
        }

        private void OnTriggerEnter(Collider other) {
            if (!other.CompareTag("Load") || _currentJoint ) return;
            Debug.Log("Touched to pickable load : " + other.name);
            _potentialLoadRigidbody = other.GetComponent<Rigidbody>();

        }
        
        private void OnTriggerExit(Collider other) {
            if (other.GetComponent<Rigidbody>() != _potentialLoadRigidbody || !other.CompareTag("Load")) return;
            Debug.Log("Untouched the pickable load: " + other.name);
            _potentialLoadRigidbody = null;
        }
    }
}
