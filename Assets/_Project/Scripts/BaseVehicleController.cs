using System.Collections.Generic;
using UnityEngine;
using System;

// This script is part of the _Project namespace and serves as a base class for vehicle controllers.
namespace _Project.Scripts {
    [RequireComponent(typeof(Rigidbody), typeof(AudioSource))]
    public abstract class BaseVehicleController : MonoBehaviour {
        public event Action<float> OnFuelChanged; // Event to notify when the fuel level changes.
        [Header("Vehicle Stats Asset")]
        [SerializeField] private VehicleStats vehicleStats;

        [Header("Vehicle Physics Parts")]
        [Tooltip("List of wheel colliders that provide drive power to the vehicle.")]
        [SerializeField]
        protected List<WheelCollider> driveWheels;

        [Tooltip("List of wheel colliders that provide steering control to the vehicle.")] [SerializeField]
        protected List<WheelCollider> steerWheels;

        [Tooltip("List of wheel transforms that represent the visual appearance of the wheels.")] [SerializeField]
        protected List<Transform> wheelContainers;

        [Tooltip("The object that represents the center of mass of the vehicle, used for stability.")] [SerializeField]
        private Transform centerOfMassObject;

        private float _currentFuel; // Current fuel level of the vehicle.

        [Header("Audio Clips")] [SerializeField]
        private AudioClip engineIdleClip;

        [SerializeField] private AudioClip engineDrivingClip;
        [SerializeField] private AudioClip engineReversingClip;
        private AudioSource _audioSource;

        private Rigidbody _rb;

        // Input Values
        private float _verticalInput;
        private float _horizontalInput;
        [SerializeField] private float minPitch = 1f;
        [SerializeField] private float maxPitch = 1.5f;
        [SerializeField] private float maxSpeed = 20f;

        private void Awake() {
            _audioSource = GetComponent<AudioSource>();
            _audioSource.clip = engineIdleClip;
            _audioSource.loop = true; // Set the audio source to loop for continuous engine sound.
            _audioSource.Play(); // Start playing the idle engine sound.
            _currentFuel = vehicleStats.maxFuel; // Initialize the current fuel to the maximum capacity.
            _rb = GetComponent<Rigidbody>();
            if (_rb) {
                _rb.centerOfMass =
                    transform.InverseTransformPoint(centerOfMassObject
                        .position); // Set the center of mass of the vehicle to improve stability.
            }
        }

        protected virtual void FixedUpdate() {
            HandleMotor();
            HandleSteering();
            UpdateWheelsVisuals();
        }

        private void Update() {
            HandleEngineSound();
        }

        private void HandleEngineSound() {
            if (!_rb || !_audioSource) return;
            var forwardSpeed = Vector3.Dot(transform.forward, _rb.linearVelocity);
            Debug.Log("Forward Speed: " + forwardSpeed);
            var speedThreshold = 0.5f;
            if (Mathf.Abs(forwardSpeed) < speedThreshold) {
                if (_audioSource.clip != engineIdleClip) {
                    _audioSource.clip = engineIdleClip;
                    _audioSource.loop = true;
                    _audioSource.Play();
                }
                _audioSource.pitch = Mathf.Lerp(_audioSource.pitch, minPitch, Time.deltaTime * 5f);
            }
            else if (forwardSpeed > speedThreshold)
            {
                if (_audioSource.clip != engineDrivingClip)
                {
                    _audioSource.clip = engineDrivingClip;
                    _audioSource.loop = true;
                    _audioSource.Play();
                }
                var speedRatio = Mathf.Clamp01(Mathf.Abs(forwardSpeed) / maxSpeed);
                _audioSource.pitch = Mathf.Lerp(minPitch, maxPitch, speedRatio);
            }
            else
            {
                if (_audioSource.clip != engineReversingClip)
                {
                    _audioSource.clip = engineReversingClip;
                    _audioSource.loop = true;
                    _audioSource.Play();
                }
                _audioSource.pitch = minPitch;
            }
        }

        // Update the visual representation of the wheels based on their current world pose.
        private void UpdateWheelsVisuals() {
            if (wheelContainers.Count != driveWheels.Count) return;

            for (var i = 0; i < wheelContainers.Count; i++) {
                driveWheels[i].GetWorldPose(out var pos, out var rot);
                wheelContainers[i].SetPositionAndRotation(pos, rot);
            }
        }

        // Handle the steering of the vehicle based on horizontal input.
        private void HandleSteering() {
            var steeringAngle = _horizontalInput * vehicleStats.maxSteerAngle;
            foreach (var wheel in steerWheels) {
                wheel.steerAngle = steeringAngle;
            }
        }

        // Handle the motor force applied to the vehicle based on vertical input.
        private void HandleMotor() {
            var currentMotorForce = _verticalInput * vehicleStats.motorForce;
            foreach (var wheel in driveWheels) {
                if (_verticalInput != 0) {
                    ConsumeFuel(Time.fixedDeltaTime * vehicleStats.fuelConsumptionRate * currentMotorForce);
                    wheel.motorTorque = currentMotorForce;
                    wheel.brakeTorque = 0f;
                }
                else {
                    wheel.motorTorque = 0f;
                    wheel.brakeTorque = vehicleStats.brakeForce;
                }
            }
        }

        public string GetVehicleName() => GetType().Name.Replace("Controller", " Controller").Trim();

        public float GetFuelPercentage() {
            return (_currentFuel / vehicleStats.maxFuel) * 100f;
        }

        public void SetMovementInput(Vector2 movement) {
            _horizontalInput = movement.x;
            _verticalInput = movement.y;
        }

        private void ConsumeFuel(float amount) {
            if (amount <= 0) return;
            _currentFuel = Mathf.Max(_currentFuel - amount, 0);
            OnFuelChanged?.Invoke(GetFuelPercentage());
        }
    }
}