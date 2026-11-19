using UnityEngine;
using UnityEngine.SceneManagement;

namespace Code
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private string _firstLevel;

        public void StartGame()
        {
            SceneManager.LoadScene(_firstLevel);
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}