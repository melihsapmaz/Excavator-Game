using UnityEngine;
using TMPro;
using UnityEngine.UI;

// This script manages the UI elements for the game, including player scores, fuel levels, and game end screen.
namespace _Project.Scripts {
    public class UIManager : MonoBehaviour {
        [Header("Shared UI Elements")] [SerializeField]
        private TextMeshProUGUI timeText;

        [SerializeField] private TextMeshProUGUI taskStatusText;

        [SerializeField] private GameObject seperatorImage;

        [Header("Player 1 UI Elements")] [SerializeField]
        private GameObject p1UIPanel;

        [SerializeField] private TextMeshProUGUI p1VehicleNameText;
        [SerializeField] private TextMeshProUGUI p1FuelText;
        [SerializeField] private TextMeshProUGUI p1LoadDeliveredText;

        [Header("Player 2 UI Elements")] [SerializeField]
        private GameObject p2UIPanel;

        [SerializeField] private TextMeshProUGUI p2VehicleNameText;
        [SerializeField] private TextMeshProUGUI p2FuelText;
        [SerializeField] private TextMeshProUGUI p2LoadDeliveredText;

        [Header("Game End UI")] [SerializeField]
        private GameObject gameEndPanel;

        [SerializeField] private TextMeshProUGUI winnerInfoText;
        [SerializeField] private TextMeshProUGUI detailsText;
        [SerializeField] private Button restartButton;

        private void Start() {
            SubscribeToGameManagerEvents();
            if (restartButton) {
                restartButton.onClick.AddListener(OnRestartButtonPressed);
            }

            gameEndPanel.SetActive(false);
        }

        private void Update() {
            if (GameManager.Instance) {
                UpdateTimeText(GameManager.Instance.GetElapsedTime());
            }
        }

        private void OnDestroy() {
            UnsubscribeFromGameManagerEvents();
        }

        // Subscribes to events from the GameManager to update UI elements.
        private void SubscribeToGameManagerEvents() {
            if (!GameManager.Instance) return;
            GameManager.Instance.OnPlayer1ScoreChanged += UpdatePlayer1Score;
            GameManager.Instance.OnPlayer2ScoreChanged += UpdatePlayer2Score;
            GameManager.Instance.OnTotalLoadsUpdated += UpdateTaskStatusText;

            if (GameManager.Instance.GetPlayer1Vehicle()) {
                GameManager.Instance.GetPlayer1Vehicle().OnFuelChanged += UpdatePlayer1Fuel;
            }

            if (GameManager.Instance.GetPlayer2Vehicle()) {
                GameManager.Instance.GetPlayer2Vehicle().OnFuelChanged += UpdatePlayer2Fuel;
            }
        }

        // Unsubscribes from events to prevent memory leaks and ensure proper cleanup.
        private void UnsubscribeFromGameManagerEvents() {
            if (!GameManager.Instance) return;
            GameManager.Instance.OnPlayer1ScoreChanged -= UpdatePlayer1Score;
            GameManager.Instance.OnPlayer2ScoreChanged -= UpdatePlayer2Score;
            GameManager.Instance.OnTotalLoadsUpdated -= UpdateTaskStatusText;

            if (GameManager.Instance.GetPlayer1Vehicle()) {
                GameManager.Instance.GetPlayer1Vehicle().OnFuelChanged -= UpdatePlayer1Fuel;
            }

            if (GameManager.Instance.GetPlayer2Vehicle()) {
                GameManager.Instance.GetPlayer2Vehicle().OnFuelChanged -= UpdatePlayer2Fuel;
            }
        }

        // Initializes the UI with player vehicle information, scores, and task status.
        public void InitializeUI(BaseVehicleController player1Vehicle, BaseVehicleController player2Vehicle,
            int playerCount, int totalLoadsToDeliver) {
            taskStatusText.text = totalLoadsToDeliver.ToString();
            if (player1Vehicle) {
                p1VehicleNameText.text = player1Vehicle.GetVehicleName();
                UpdatePlayer1Fuel(player1Vehicle.GetFuelPercentage());
            }

            seperatorImage.SetActive(playerCount == 2); // Show seperator line between players if there are two players.

            switch (playerCount) {
                case 1:
                    UpdatePlayer1Score(GameManager.Instance.GetPlayer1Score());
                    UpdateTaskStatusText(GameManager.Instance.GetPlayer1Score(), 0);
                    SetPlayer2UIVisible(false);
                    break;
                case 2:
                    SetPlayer2UIVisible(true);
                    UpdatePlayer1Score(GameManager.Instance.GetPlayer1Score());
                    UpdatePlayer2Score(GameManager.Instance.GetPlayer2Score());
                    UpdateTaskStatusText(GameManager.Instance.GetPlayer1Score(),
                        GameManager.Instance.GetPlayer2Score());
                    if (player2Vehicle) {
                        p2VehicleNameText.text = player2Vehicle.GetVehicleName();
                        UpdatePlayer2Fuel(player2Vehicle.GetFuelPercentage());
                    }

                    break;
            }
        }

        private void UpdatePlayer1Score(int score) => p1LoadDeliveredText.text = "Load Delivered: " + score;
        private void UpdatePlayer2Score(int score) => p2LoadDeliveredText.text = "Load Delivered: " + score;
        private void UpdatePlayer1Fuel(float fuel) => p1FuelText.text = "Fuel: " + fuel.ToString("F0") + "%";
        private void UpdatePlayer2Fuel(float fuel) => p2FuelText.text = "Fuel: " + fuel.ToString("F0") + "%";


        // Updates the time text in the UI with the elapsed time in minutes and seconds format.
        private void UpdateTimeText(float timeInSeconds) {
            var minutes = Mathf.FloorToInt(timeInSeconds / 60);
            var seconds = Mathf.FloorToInt(timeInSeconds % 60);
            timeText.text = "Time: " + $"{minutes:00}:{seconds:00}";
        }

        // Updates the delivery info text with the current scores of both players.
        private void UpdateTaskStatusText(int player1Score, int player2Score) {
            //Debug.Log("Updating task status text with scores: " + player1Score + ", " + player2Score);
            p1LoadDeliveredText.text = "Delivered Loads: " + player1Score;
            p2LoadDeliveredText.text = "Delivered Loads: " + player2Score;
            taskStatusText.text =
                $"Task Status: {GameManager.Instance.GetDeliveredLoads()}/{GameManager.Instance.GetTotalLoadsToDeliver()}";
        }

        // Sets the visibility of Player 2's UI panel
        private void SetPlayer2UIVisible(bool isVisible) {
            p2UIPanel.SetActive(isVisible);
        }

        // Displays the game end screen with the winner's information and details.
        public void ShowGameEndScreen(string winnerInfo, string details) {
            gameEndPanel.SetActive(true);
            winnerInfoText.text = winnerInfo;
            detailsText.text = details;
        }

        // Handles the restart button press event to restart the game.
        private static void OnRestartButtonPressed() {
            if (GameManager.Instance) {
                GameManager.RestartGame();
            }
        }
    }
}