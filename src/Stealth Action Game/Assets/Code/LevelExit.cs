using UnityEngine;

namespace Code
{
    public class LevelExit : MonoBehaviour
    {
        private bool _playerInZone;

        [SerializeField] private float _exitWaitTime = 2f;
        private float _exitCounter;

        [SerializeField] private string _nextLevel;

        private void Update()
        {
            if (_playerInZone == true)
            {
                _exitCounter -= Time.deltaTime;
                if (_exitCounter <= 0f)
                {
                    _exitCounter = 0;
                    _playerInZone = false;

                    FindFirstObjectByType<PlayerController>().gameObject.SetActive(false);

                    UIController.Instance.ShowLevelCompleteScreen(_nextLevel);
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag("Player"))
            {
                _playerInZone = true;
                _exitCounter = _exitWaitTime;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if(other.CompareTag("Player"))
            {
                _playerInZone = false;
            }
        }
    }
}