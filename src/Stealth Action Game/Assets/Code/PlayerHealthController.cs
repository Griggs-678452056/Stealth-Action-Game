using UnityEngine;

namespace Code
{
    public class PlayerHealthController : MonoBehaviour
    {
        [SerializeField] private int _startingHealth = 5;
        private int _currentHealth;

        private void Start()
        {
            _currentHealth = _startingHealth;

            UIController.Instance.UpdateHealth(_currentHealth, _startingHealth);
        }

        public void TakeDamage()
        {
            _currentHealth --;
            if (_currentHealth <= 0)
            {
                GetComponent<PlayerController>().GetKilled();
            }

            UIController.Instance.UpdateHealth(_currentHealth, _startingHealth);
        }
    }
}
