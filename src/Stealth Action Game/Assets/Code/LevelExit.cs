using System;
using System.Collections.Generic;
using UnityEngine;

namespace Code
{
    public class LevelExit : MonoBehaviour
    {
        private bool _playerInZone;

        [SerializeField] private float _exitWaitTime = 2f;
        private float _exitCounter;

        [SerializeField] private string _nextLevel;

        private List<EnemyController> _allEnemies = new List<EnemyController>();

        [SerializeField] private float _timeBetweenChecks = 0.2f;
        private float _checkCounter;

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

                UIController.Instance.UpdateExitSlider(_exitCounter, _exitWaitTime);

                _checkCounter -= Time.deltaTime;
                if (_checkCounter <= 0f)
                {
                    _checkCounter = _timeBetweenChecks;

                    if(CheckIfSeen() == true)
                    {
                        _playerInZone = false;
                        UIController.Instance.HideExitSlider();
                    }
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag("Player"))
            {
                _allEnemies.Clear();
                _allEnemies.AddRange(FindObjectsByType<EnemyController>(FindObjectsSortMode.None));

                if (CheckIfSeen() == false)
                {
                    _playerInZone = true;
                    _exitCounter = _exitWaitTime;

                    UIController.Instance.ShowExitSlider();
                    UIController.Instance.UpdateExitSlider(_exitCounter, _exitWaitTime);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if(other.CompareTag("Player"))
            {
                _playerInZone = false;

                UIController.Instance.HideExitSlider();
            }
        }

        private bool CheckIfSeen()
        {
            bool isSeen = false;

            foreach (EnemyController enemy in _allEnemies)
            {
                if (enemy.CurrentState != EnemyController.EnemyState.Normal)
                {
                    isSeen = true;
                }
            }

            return isSeen;
        }
    }
}