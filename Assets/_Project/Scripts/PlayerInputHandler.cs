using System;
using UnityEngine;
using UnityEngine.InputSystem;

// This script handles player input for a vehicle controller in a Unity game.
namespace _Project.Scripts {
    public class PlayerInputHandler : MonoBehaviour {
        // Enum to identify players
        public enum PlayerID {
            Player1,
            Player2
        }

        private PlayerID _playerID;

        // References to various vehicle controllers and input systems
        private BaseVehicleController _baseVehicleController;
        private WheelLoaderController _wheelLoaderController;
        private TelehandlerController _telehandlerController;
        private AttachmentController _attachmentController;
        private PlayerInputs _playerInputs;

        // Input values for movement and arm controls
        private Vector2 _moveInput;
        private float _arm1Input;
        private float _arm2Input;

        private void Awake() {
            _attachmentController = GetComponentInChildren<AttachmentController>();
            _baseVehicleController = GetComponent<BaseVehicleController>();
            _wheelLoaderController = GetComponent<WheelLoaderController>();
            _telehandlerController = GetComponent<TelehandlerController>();
            if (!_baseVehicleController) {
                //Debug.LogError("BaseVehicleController component not found on this GameObject.");
                return;
            }

            _playerInputs = new PlayerInputs();
        }

        private void FixedUpdate() {
            if (_baseVehicleController) {
                // Apply movement input to the base vehicle controller
                _baseVehicleController.SetMovementInput(_moveInput);
            }

            if (_wheelLoaderController) {
                // Apply arm inputs to the wheel loader controller
                _wheelLoaderController.SetArmInput(_arm1Input, _arm2Input);
            }

            if (_telehandlerController) {
                // Apply arm inputs to the telehandler controller
                _telehandlerController.SetArmInput(_arm1Input, _arm2Input);
            }
        }

        // Disable method to clean up input actions
        private void OnDisable() {
            _playerInputs.P1.Disable();
            _playerInputs.P2.Disable();
            if (_playerInputs == null) return;
            _playerInputs.P1.PickLoad.performed -= OnPickLoad;
            _playerInputs.P2.PickLoad.performed -= OnPickLoad;
        }

        // Method to handle the 'PickLoad' input action
        private void OnPickLoad(InputAction.CallbackContext obj) {
            if (obj.phase != InputActionPhase.Performed) return;
            //Debug.Log(_playerID + " 'PickLoad' button is pressed!");
            if (_attachmentController) {
                _attachmentController.ToggleAttachment();
            }
        }

        // Method to handle movement input
        private void OnMove(InputAction.CallbackContext obj) {
            var moveVector = obj.ReadValue<Vector2>();
            _moveInput = moveVector;
        }

        // Method to initialize the player input handler with a specific player ID
        public void Initialize(PlayerID player) {
            _playerID = player;
            switch (_playerID) {
                case PlayerID.Player1:
                    var p1Actions = _playerInputs.P1;
                    p1Actions.Enable();

                    p1Actions.Move.performed += OnMove;
                    p1Actions.Move.canceled += OnMove;
                    p1Actions.PickLoad.performed += OnPickLoad;
                    p1Actions.PickLoad.canceled += OnPickLoad;

                    p1Actions.Arm1Control.performed += context => _arm1Input = context.ReadValue<float>();
                    p1Actions.Arm1Control.canceled += _ => _arm1Input = 0f;
                    p1Actions.Arm2Control.performed += context => _arm2Input = context.ReadValue<float>();
                    p1Actions.Arm2Control.canceled += _ => _arm2Input = 0f;
                    break;
                case PlayerID.Player2:
                    var p2Actions = _playerInputs.P2;
                    p2Actions.Enable();

                    p2Actions.Move.performed += OnMove;
                    p2Actions.Move.canceled += OnMove;
                    p2Actions.PickLoad.performed += OnPickLoad;
                    p2Actions.PickLoad.canceled += OnPickLoad;

                    p2Actions.Arm1Control.performed += context => _arm1Input = context.ReadValue<float>();
                    p2Actions.Arm1Control.canceled += _ => _arm1Input = 0f;
                    p2Actions.Arm2Control.performed += context => _arm2Input = context.ReadValue<float>();
                    p2Actions.Arm2Control.canceled += _ => _arm2Input = 0f;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (_attachmentController) {
                _attachmentController.Initialize(_playerID);
            }
        }
    }
}