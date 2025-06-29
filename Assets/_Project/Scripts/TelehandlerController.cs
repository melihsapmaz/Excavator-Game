using UnityEngine;

// This script defines the TelehandlerController class, which manages the telehandler's arm rotation and extension in a Unity game.
namespace _Project.Scripts {
    public class TelehandlerController : BaseVehicleController {
        [Header("Rotation Arm Control")]
        [Tooltip("Hinge joint for the rotation of the telehandler arm.")]
        [SerializeField]
        private HingeJoint rotationJoint;

        [Tooltip("Speed of the rotation")] [SerializeField]
        private float rotationSpeed = 100f;

        [Tooltip("Max force of the rotation joint motor.")] [SerializeField]
        private float motorForce = 50000f;

        [Header("Telescopic Arm Control (Configurable Joint)")]
        [Tooltip("Configurable joint for the telescopic arm.")]
        [SerializeField]
        private ConfigurableJoint telescopicJoint;

        [Tooltip("Speed of the telescopic arm extension/retraction.")] [SerializeField]
        private float extensionSpeed = 1f;

        [Tooltip("Minimum extension distance of the telescopic arm.")] [SerializeField]
        private float minExtension;

        [Tooltip("Maximum extension distance of the telescopic arm.")] [SerializeField]
        private float maxExtension = 5f;

        [Header("Telescopic Settings")]
        [Tooltip("Spring force to keep the telescopic arm in position(Spring)")]
        [SerializeField]
        private float telescopicDriveSpring = 20000f;

        [Tooltip("Damper force to avoid oscillation(Damper)")] [SerializeField]
        private float telescopicDriveDamper = 200f;

        private float _rotationInput;
        private float _telescopicInput;

        private Vector3 _telescopicTargetPosition;

        private void Start() {
            if (rotationJoint) {
                var motor = rotationJoint.motor;
                motor.force = motorForce;
                rotationJoint.motor = motor;
                rotationJoint.useMotor = true;
            }
            else {
                //Debug.LogError("Rotation joint is not assigned to this object!", gameObject);
            }

            if (telescopicJoint) {
                var linearDrive = new JointDrive {
                    positionSpring = telescopicDriveSpring,
                    positionDamper = telescopicDriveDamper,
                    maximumForce = float.MaxValue
                };
                telescopicJoint.zDrive = linearDrive; // Set the zDrive for telescopic movement
                _telescopicTargetPosition = telescopicJoint.targetPosition;
            }
            else {
                //Debug.LogError("Telescopic joint is not assigned to this object!", gameObject);
            }
        }

        protected override void FixedUpdate() {
            base.FixedUpdate();
            HandleTelescopicMovement();
            HandleRotation();
        }

        // Handles the rotation of the telehandler arm based on user input.
        private void HandleRotation() {
            if (!rotationJoint) {
                //Debug.LogError("Rotation joint is not assigned.");
                return;
            }

            var motor = rotationJoint.motor;
            motor.targetVelocity = _rotationInput * rotationSpeed;
            rotationJoint.motor = motor;
        }

        // Sets the input values for the telehandler arm's rotation and telescopic extension.
        public void SetArmInput(float rotInput, float extInput) {
            _rotationInput = rotInput;
            _telescopicInput = extInput;
        }

        // Handles the telescopic movement of the telehandler arm based on user input.
        private void HandleTelescopicMovement() {
            if (!telescopicJoint) {
                //Debug.LogError("Telescopic joint is not assigned.");
                return;
            }

            if (_telescopicInput == 0f) return;
            _telescopicTargetPosition.z += _telescopicInput * extensionSpeed * Time.fixedDeltaTime;
            _telescopicTargetPosition.z = Mathf.Clamp(_telescopicTargetPosition.z, minExtension, maxExtension);
            telescopicJoint.targetPosition = _telescopicTargetPosition;
        }
    }
}