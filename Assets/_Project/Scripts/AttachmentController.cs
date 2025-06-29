using System;
using UnityEngine;

// This script is part of the _Project namespace and handles the attachment and detachment of loads in a game.
namespace _Project.Scripts {
    [RequireComponent(typeof(Collider), typeof(AudioSource))]
    public class AttachmentController : MonoBehaviour {
        private Rigidbody _potentialLoadRigidbody; // The rigidbody of the load that can be picked up.
        private FixedJoint _currentJoint; // The joint that connects the attachment area to the load.

        [Header("Audio")] [SerializeField] private AudioClip dropSoundClip;
        private AudioSource _audioSource;

        public static event Action<GameObject, PlayerInputHandler.PlayerID>
            OnLoadDropped; // Event to notify when a load is dropped.

        private PlayerInputHandler.PlayerID _ownerPlayerID;

        private void Awake() {
            _audioSource = GetComponent<AudioSource>();
        }

        // Initialize the attachment controller with the owner player ID.
        // We need this to notify who dropped the load
        public void Initialize(PlayerInputHandler.PlayerID ownerID) {
            _ownerPlayerID = ownerID;
        }

        // Toggle the attachment
        public void ToggleAttachment() {
            // State 1: Carried load is dropped.
            if (_currentJoint) {
                var droppedLoadObject = _currentJoint.connectedBody.gameObject;
                Destroy(_currentJoint); // Destroy the joint to drop the load.
                _currentJoint = null;
                _potentialLoadRigidbody.useGravity = true; // Re-enable gravity for the dropped load.
                _potentialLoadRigidbody = null;
                OnLoadDropped?.Invoke(droppedLoadObject, _ownerPlayerID); // Notify that the load has been dropped.
                if (dropSoundClip) {
                    _audioSource.PlayOneShot(dropSoundClip); // Play the drop sound if available.
                }
            }
            // State 2: A load is picked up.
            else if (_potentialLoadRigidbody) {
                _currentJoint = gameObject.AddComponent<FixedJoint>(); // Create a new FixedJoint to attach the load.
                _currentJoint.connectedBody =
                    _potentialLoadRigidbody; // Connect the joint to the potential load's rigidbody.
                _potentialLoadRigidbody.useGravity = false; // Disable gravity for the load while it is attached.
                _currentJoint.breakForce = Mathf.Infinity; // Set the break force and break torque to infinity
                _currentJoint.breakTorque =
                    Mathf.Infinity; // to prevent the joint from breaking under normal conditions.
            }
            else {
                //Debug.Log("There is no load to pick up or drop.");
            }
        }

        // Handle the trigger events for detecting pickable loads.
        private void OnTriggerEnter(Collider other) {
            if (!other.CompareTag("Load") || _currentJoint)
                return; // If the object is not a load or already attached, ignore it.
            //Debug.Log("Touched to pickable load : " + other.name);
            _potentialLoadRigidbody = other.GetComponent<Rigidbody>();
        }

        // Handle the exit event for when the player stops touching a pickable load.
        private void OnTriggerExit(Collider other) {
            if (other.GetComponent<Rigidbody>() != _potentialLoadRigidbody || !other.CompareTag("Load"))
                return; // If the exiting object is not the potential load, ignore it.
            //Debug.Log("Untouched the pickable load: " + other.name);
            _potentialLoadRigidbody = null;
        }
    }
}