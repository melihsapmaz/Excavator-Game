using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// This script manages the main menu, allowing players to select vehicles and start the game.
namespace _Project.Scripts {
    public class MenuManager : MonoBehaviour {
        [Header("UI Panels")] [SerializeField] private GameObject p1SelectionPanel;
        [SerializeField] private GameObject p2SelectionPanel;
        [SerializeField] private GameObject p2JoinText;
        [SerializeField] private Button startButton;

        private int _playerCount = 1;
        private VehicleType _p1Selection = VehicleType.None;
        private VehicleType _p2Selection = VehicleType.None;

        private void Update() {
            if (_playerCount == 1 && Input.GetKeyDown(KeyCode.KeypadEnter)) {
                JoinPlayer2();
            }
        }

        private void JoinPlayer2() {
            _playerCount = 2;
            p2JoinText.SetActive(false);
            p2SelectionPanel.SetActive(true);
            //Debug.Log("Player 2 has joined the game!");
            UpdateStartButtonState();
        }

        private void SelectVehicle(int playerNumber, VehicleType selectedVehicle) {
            switch (playerNumber) {
                case 1:
                    _p1Selection = selectedVehicle;
                    //Debug.Log("Player 1 selected: " + _p1Selection);
                    break;
                case 2:
                    _p2Selection = selectedVehicle;
                    //Debug.Log("Player 2 selected: " + _p2Selection);
                    break;
            }

            UpdateStartButtonState();
        }

        public void StartGame() {
            GameSettings.Instance.playerCount = _playerCount;
            GameSettings.Instance.player1Vehicle = _p1Selection;
            GameSettings.Instance.player2Vehicle = _p2Selection;

            SceneManager.LoadScene("GameScene");
        }

        private void UpdateStartButtonState() {
            if (_playerCount == 1) {
                startButton.interactable = (_p1Selection != VehicleType.None);
            }
            else {
                startButton.interactable = (_p1Selection != VehicleType.None && _p2Selection != VehicleType.None);
            }
        }

        public void P1_Selects_WheelLoader() {
            SelectVehicle(1, VehicleType.WheelLoader);
        }

        public void P1_Selects_Telehandler() {
            SelectVehicle(1, VehicleType.Telehandler);
        }

        public void P2_Selects_WheelLoader() {
            SelectVehicle(2, VehicleType.WheelLoader);
        }

        public void P2_Selects_Telehandler() {
            SelectVehicle(2, VehicleType.Telehandler);
        }
    }
}