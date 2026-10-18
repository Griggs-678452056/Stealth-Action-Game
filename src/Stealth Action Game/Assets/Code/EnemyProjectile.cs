using UnityEngine;

namespace Code
{
    public class EnemyProjectile : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed;

        private void Start()
        {
            Transform player = FindFirstObjectByType<PlayerController>().transform;

            transform.LookAt(player.position + Vector3.up);

            Destroy(gameObject, 5f);
        }

        private void Update()
        {
            transform.position += transform.forward * (_moveSpeed * Time.deltaTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Debug.Log("Player hit by enemy projectile!");
            }

            Destroy(gameObject);
        }
    }
}
