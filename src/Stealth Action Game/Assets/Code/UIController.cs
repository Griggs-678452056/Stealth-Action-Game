using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Code
{
    public class UIController : MonoBehaviour
    {
        public static UIController Instance { get; private set; }

        [SerializeField] private TMP_Text _ammoText;

        [SerializeField] private Slider _healthSlider;

        [SerializeField] private GameObject _gameOverPanel;

        private void Awake()
        {
            Instance = this;
        }

        public void UpdateAmmoText(int currentAmmo)
        {
            _ammoText.text = $"{currentAmmo}";
        }

        public void UpdateAmmoText(int currentAmmo, int totalAmmo)
        {
            _ammoText.text = $"{currentAmmo}/{totalAmmo}";
        }

        public void UpdateHealth(int currentHealth, int maxHealth)
        {
            _healthSlider.maxValue = maxHealth;
            _healthSlider.value = currentHealth;
        }

        public void ShowGameOverPanel()
        {
            _gameOverPanel.SetActive(true);
        }

        public void RestartGame()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}
