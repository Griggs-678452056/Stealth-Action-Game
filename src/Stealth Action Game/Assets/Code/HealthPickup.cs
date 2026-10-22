using UnityEngine;

namespace Code
{
    public class HealthPickup : MonoBehaviour
    {
        [SerializeField] private int _healthAmount;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Destroy(gameObject);

                other.GetComponent<PlayerHealthController>().Heal(_healthAmount);
            }
        }
    }
}
