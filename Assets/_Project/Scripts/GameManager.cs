using System;
using System.Collections;
using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.SceneManagement;

// This script manages the game state, player vehicles, and UI updates.
namespace _Project.Scripts {
    [RequireComponent(typeof(AudioSource))]
    public class GameManager : MonoBehaviour {
        public static GameManager Instance { get; private set; }
        [Header("Audio")] [SerializeField] private AudioClip gameEndApplauseClip;
        private AudioSource _audioSource;
        public event Action<int> OnPlayer1ScoreChanged; // Event to notify when Player 1's score changes
        public event Action<int> OnPlayer2ScoreChanged; // Event to notify when Player 2's score changes

        public event Action<int, int>
            OnTotalLoadsUpdated; // Event to notify when total loads delivered by both players are updated

        [Header("Level Prefabs & Spawn Points")] [SerializeField]
        private GameObject wheelLoaderPrefab;

        [SerializeField] private GameObject telehandlerPrefab;
        [SerializeField] private Transform player1SpawnPoint;
        [SerializeField] private Transform player2SpawnPoint;

        [Header("Camera Control")] [SerializeField]
        private Camera player1Camera;

        [SerializeField] private CinemachineCamera player1CinemachineCamera;
        [SerializeField] private Camera player2Camera;
        [SerializeField] private CinemachineCamera player2CinemachineCam;
        [SerializeField] private UIManager uiManager;

        private BaseVehicleController _player1Vehicle;
        private BaseVehicleController _player2Vehicle;

        [Header("Task Settings")] [Tooltip("The total number of loads required to complete the task")] [SerializeField]
        private int totalLoadsToDeliver = 10;

        private int _deliveredLoads;
        private int _player1Score;
        private int _player2Score;
        private bool _isGameOver;

        private float _elapsedTime;

        private void Awake() {
            if (Instance && !Equals(Instance, this)) {
                //Debug.LogWarning("Multiple instances of GameManager detected. Destroying the new instance.");
                Destroy(gameObject);
            }
            else {
                Instance = this;
            }

            _audioSource = GetComponent<AudioSource>();
        }

        private void Start() {
            SetupGame();
        }

        private void Update() {
            _elapsedTime += Time.deltaTime;
        }


        // This method initializes the game based on the settings defined in GameSettings.
        private void SetupGame() {
            if (!GameSettings.Instance) {
                //Debug.LogWarning("GameSettings not found. Starting a default single-player game.");
                SpawnPlayer(1, VehicleType.WheelLoader);
                SetupCameras(1);
                return;
            }

            var playerCount = GameSettings.Instance.playerCount;
            var p1Type = GameSettings.Instance.player1Vehicle;
            var p2Type = GameSettings.Instance.player2Vehicle;

            SpawnPlayer(1, p1Type);

            // If there are two players, spawn the second player
            if (playerCount == 2) {
                SpawnPlayer(2, p2Type);
            }

            SetupCameras(playerCount);
            totalLoadsToDeliver = GameObject.FindGameObjectsWithTag("Load").Length; // Get total loads from the scene
            uiManager.InitializeUI(_player1Vehicle, _player2Vehicle, playerCount, totalLoadsToDeliver);
        }

        private void SpawnPlayer(int playerNumber, VehicleType vehicleType) {
            var spawnPoint = (playerNumber == 1) ? player1SpawnPoint : player2SpawnPoint;
            var prefabToSpawn = (vehicleType == VehicleType.WheelLoader) ? wheelLoaderPrefab : telehandlerPrefab;

            if (!prefabToSpawn || !spawnPoint) {
                //Debug.LogError("Player " + playerNumber + " prefab or spawn point is not assigned in GameManager!");
                return;
            }

            var vehicleInstance = Instantiate(prefabToSpawn, spawnPoint.position, spawnPoint.rotation);

            if (playerNumber == 1) {
                _player1Vehicle = vehicleInstance.GetComponent<BaseVehicleController>();
                vehicleInstance.GetComponent<PlayerInputHandler>().Initialize(PlayerInputHandler.PlayerID.Player1);
                player1CinemachineCamera.Follow = vehicleInstance.transform;
                player1CinemachineCamera.LookAt = vehicleInstance.transform;
            }
            else {
                _player2Vehicle = vehicleInstance.GetComponent<BaseVehicleController>();
                vehicleInstance.GetComponent<PlayerInputHandler>().Initialize(PlayerInputHandler.PlayerID.Player2);
                player2CinemachineCam.Follow = vehicleInstance.transform;
                player2CinemachineCam.LookAt = vehicleInstance.transform;
            }
        }

        // This method sets up the cameras based on the number of players.
        private void SetupCameras(int playerCount) {
            if (playerCount == 1) {
                player1Camera.rect = new Rect(0, 0, 1, 1);
                player2Camera.gameObject.SetActive(false);
            }
            else {
                player1Camera.rect = new Rect(0, 0, 0.5f, 1);
                player2Camera.gameObject.SetActive(true);
                player2Camera.rect = new Rect(0.5f, 0, 0.5f, 1);
            }
        }

        // This method is called when a load is delivered by either player.
        public void OnLoadDelivered(PlayerInputHandler.PlayerID playerID) {
            _deliveredLoads++;
            switch (playerID) {
                case PlayerInputHandler.PlayerID.Player1:
                    _player1Score++;
                    OnPlayer1ScoreChanged?.Invoke(_player1Score);
                    break;
                case PlayerInputHandler.PlayerID.Player2:
                    _player2Score++;
                    OnPlayer2ScoreChanged?.Invoke(_player2Score);
                    break;
                default:
                    //Debug.LogWarning($"Player {playerID} has not been implemented yet.");
                    break;
            }

            OnTotalLoadsUpdated?.Invoke(_player1Score, _player2Score);
            //Debug.Log($"Load delivered! Total delivered: {_deliveredLoads}/{totalLoadsToDeliver}");

            UpdateGameStateUI();
            CheckForWinCondition();
        }

        private void CheckForWinCondition() {
            if (_isGameOver) return;
            if (_deliveredLoads < totalLoadsToDeliver) return;
            _isGameOver = true;
            StartCoroutine(EndGameSequence());
        }

        // This method handles the end of the game, displaying the winner and their scores.
        private void EndGame() {
            //Debug.Log("--- GAME OVER ---");
            Time.timeScale = 0f;

            if (gameEndApplauseClip != null) {
                _audioSource.PlayOneShot(gameEndApplauseClip);
            }

            var winnerText = "";
            var detailsText = "";

            if (_player1Score > _player2Score) {
                winnerText = "Winner: Player 1";
                detailsText = $"{_player1Vehicle.GetVehicleName()} - Score: {_player1Score}";
            }

            else if (_player2Score > _player1Score) {
                winnerText = "Winner: Player 2";
                detailsText = $"{_player2Vehicle.GetVehicleName()} - Score: {_player2Score}";
            }
            else {
                winnerText = "It's a Draw!";
                detailsText = $"Both players scored: {_player1Score}";
            }

            if (uiManager) {
                uiManager.ShowGameEndScreen(winnerText, detailsText);
            }
        }

        // This coroutine handles the end game sequence, including a delay before showing the end game screen.
        private IEnumerator EndGameSequence() {
            _isGameOver = true;
            yield return new WaitForSeconds(2f);
            EndGame();
        }

        // This method restarts the game by resetting the timescale and loading the main menu scene.
        public static void RestartGame() {
            Time.timeScale = 1f;

            if (GameSettings.Instance) {
                GameSettings.Instance.ResetSettings();
            }

            SceneManager.LoadScene("MainMenuScene");
        }

        // This method updates the game state UI with the current number of loads delivered.
        private void UpdateGameStateUI() {
            // This method can be used to update the UI with the current game state.
            //Debug.Log($"Current Game State: {_deliveredLoads}/{totalLoadsToDeliver} loads delivered.");
        }

        public float GetElapsedTime() => _elapsedTime;

        public int GetTotalLoadsToDeliver() => totalLoadsToDeliver;

        public int GetDeliveredLoads() => _deliveredLoads;

        public BaseVehicleController GetPlayer1Vehicle() => _player1Vehicle;

        public BaseVehicleController GetPlayer2Vehicle() => _player2Vehicle;
        public int GetPlayer1Score() => _player1Score;
        public int GetPlayer2Score() => _player2Score;
    }
}