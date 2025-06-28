using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

// This script is part of the _Project namespace and serves as a base class for vehicle controllers.
namespace _Project.Scripts {
    public abstract class BaseVehicleController : MonoBehaviour {
        
        [Header("Vehicle Physics Parts")]
        [Tooltip("List of wheel colliders that provide drive power to the vehicle.")]
        [SerializeField] protected List<WheelCollider> driveWheels;
        
        [Tooltip("List of wheel colliders that provide steering control to the vehicle.")]
        [SerializeField] protected List<WheelCollider> steerWheels;
        
        [FormerlySerializedAs("wheelVisuals")]
        [Tooltip("List of wheel transforms that represent the visual appearance of the wheels.")]
        [SerializeField] protected List<Transform> wheelContainers;
        
        [Tooltip("Maximum force applied to the vehicle's wheels.")]
        [SerializeField] protected float maxMotorForce = 2000f;
        
        [Tooltip("Maximum force applied to the vehicle's brakes.")]
        [SerializeField] protected float brakeForce = 3000f;
        
        [Tooltip("Maximum steering angle for the vehicle's wheels.")]
        [SerializeField] protected float maxSteeringAngle = 30f;
        
        [Tooltip("The object that represents the center of mass of the vehicle, used for stability.")]
        [SerializeField] private Transform centerOfMassObject;
        
        private Rigidbody _rb;
        
        // Input Values
        private float _verticalInput;
        private float _horizontalInput;

        private void Awake() {
            _rb = GetComponent<Rigidbody>();
            if (_rb) {
                _rb.centerOfMass = transform.InverseTransformPoint(centerOfMassObject.position);
            }
        }
        protected virtual void FixedUpdate() {
            HandleMotor();
            HandleSteering();
            UpdateWheelsVisuals();
        }

        private void UpdateWheelsVisuals() {
            
            if (wheelContainers.Count != driveWheels.Count) return;

            for (var i = 0; i < wheelContainers.Count; i++)
            {
                driveWheels[i].GetWorldPose(out var pos, out var rot);
                wheelContainers[i].SetPositionAndRotation(pos, rot);
            }
        }

        private void HandleSteering() {
            var steeringAngle = _horizontalInput * maxSteeringAngle;
            foreach (var wheel in steerWheels) {
                wheel.steerAngle = steeringAngle;
            }
        }

        private void HandleMotor() {
            var currentMotorForce = _verticalInput * maxMotorForce;
            foreach (var wheel in driveWheels) {
                if (_verticalInput != 0) {
                    wheel.motorTorque = currentMotorForce;
                    wheel.brakeTorque = 0f;
                }
                else {
                    wheel.motorTorque = 0f;
                    wheel.brakeTorque = brakeForce;
                }
            }
        }

        public string GetVehicleName() => GetType().Name;
        
        public void SetMovementInput(Vector2 movement) {
            _horizontalInput = movement.x;
            _verticalInput = movement.y;
        }
    }
}