using UnityEngine;
using TMPro;
using UnityEngine.Serialization;

namespace _Project.Scripts {
    public class UIManager : MonoBehaviour {
        [Header("Shared UI Elements")]
        [SerializeField] private TextMeshProUGUI timeText;

        [SerializeField] private TextMeshProUGUI taskStatusText;

        [Header("Player 1 UI Elements")]
        [SerializeField] private GameObject p1UIPanel;

        [SerializeField] private TextMeshProUGUI p1VehicleNameText;
        [SerializeField] private TextMeshProUGUI p1FuelText;

        [Header("Player 2 UI Elements")]
        [SerializeField] private GameObject p2UIPanel;

        [SerializeField] private TextMeshProUGUI p2VehicleNameText;
        [SerializeField] private TextMeshProUGUI p2FuelText;

        public void UpdateTimeText(float timeInSeconds) {
            var minutes = Mathf.FloorToInt(timeInSeconds / 60);
            var seconds = Mathf.FloorToInt(timeInSeconds % 60);
            timeText.text = "Time: " + $"{minutes:00}:{seconds:00}";
        }
        
        public void UpdateTaskStatusText(int deliveredLoads, int totalLoads) {
            taskStatusText.text = $"Tasks: {totalLoads - deliveredLoads}/{totalLoads}";
        }
        
        public void UpdatePlayer1UI(string vehicleName, float fuel)
        {
            if (!p1UIPanel.activeSelf) p1UIPanel.SetActive(true);
        
            p1VehicleNameText.text = vehicleName;
            p1FuelText.text = "Fuel: " + fuel.ToString("F0") + "%";
        }
        
        public void UpdatePlayer2UI(string vehicleName, float fuel)
        {
            if (!p2UIPanel.activeSelf) p2UIPanel.SetActive(true);
        
            p2VehicleNameText.text = vehicleName;
            p2FuelText.text = "Fuel: " + fuel.ToString("F0") + "%";
        }
        
        public void SetPlayer2UIVisible(bool isVisible)
        {
            p2UIPanel.SetActive(isVisible);
        }
    }
}