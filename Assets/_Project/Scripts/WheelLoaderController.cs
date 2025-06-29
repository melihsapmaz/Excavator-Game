using UnityEngine;

// This script defines the vehicle types available in the game.
namespace _Project.Scripts {
    public class WheelLoaderController : BaseVehicleController {
        [Header("Wheel Loader Arm Controls")]
        [Tooltip("First arm joint that connects the base to the first arm segment.")]
        [SerializeField]
        private HingeJoint arm1Joint;

        [Tooltip("Second arm joint that connects the first arm segment to the second arm segment.")] [SerializeField]
        private HingeJoint arm2Joint;

        [Header("Arm Movement Settings")] [Tooltip("Multiplier for the arm movement speed.")] [SerializeField]
        private float armSpeed = 1f;

        [Tooltip("Maximum motor force to move and keep the arm in position.")] [SerializeField]
        private float motorForce = 50000f;

        private float _arm1Input;
        private float _arm2Input;

        private void Start() {
            if (arm1Joint != null) {
                var motor1 = arm1Joint.motor;
                motor1.force = motorForce;
                arm1Joint.motor = motor1;
                arm1Joint.useMotor = true;
            }
            else {
                //Debug.LogError("Arm 1 Joint is not assigned to this object!!", this.gameObject);
            }

            if (arm2Joint != null) {
                var motor2 = arm2Joint.motor;
                motor2.force = motorForce;
                arm2Joint.motor = motor2;
                arm2Joint.useMotor = true;
            }
            else {
                //Debug.LogError("Arm 2 Joint is not assigned to this object!!", this.gameObject);
            }
        }

        protected override void FixedUpdate() {
            base.FixedUpdate();
            HandleArmMovements();
        }

        // Handles the arm movements based on player input.
        private void HandleArmMovements() {
            if (arm1Joint) {
                var motor1 = arm1Joint.motor;
                motor1.targetVelocity = _arm1Input * armSpeed;
                arm1Joint.motor = motor1;
            }
            else {
                //Debug.LogError("Arm 1 Joint is not assigned to this object!!", this.gameObject);
            }

            if (arm2Joint) {
                var motor2 = arm2Joint.motor;
                motor2.targetVelocity = _arm2Input * armSpeed;
                arm2Joint.motor = motor2;
            }
            else {
                //Debug.LogError("Arm 2 Joint is not assigned to this object!!", this.gameObject);
            }
        }

        // Sets the input values for the arm movements.
        public void SetArmInput(float arm1Input, float arm2Input) {
            _arm1Input = arm1Input;
            _arm2Input = arm2Input;
        }
    }
}