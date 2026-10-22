using UnityEngine;

namespace Code
{
    public class ShootableObject : MonoBehaviour
    {
        [SerializeField] private GameObject _destroyEffect;

        [SerializeField] private GameObject _ammoToDrop;
        [Range(0f, 100f)]
        [SerializeField] private float _ammoChance;

        [SerializeField] private GameObject _healthPickupToDrop;
        [Range(0f, 100f)]
        [SerializeField] private float _healthPickupChance;

        public void DestroyObject()
        {
            Destroy(gameObject);
            Instantiate(_destroyEffect, transform.position, transform.rotation);

            if (_ammoToDrop != null)
            {
                if (Random.Range(0f, 100f) < _ammoChance)
                {
                    Instantiate(_ammoToDrop, transform.position + new Vector3 (-.25f, 0f, 0f), _ammoToDrop.transform.rotation);
                }
            }
            
            if (_healthPickupChance > 0f && _healthPickupToDrop != null)
            {
                if (Random.Range(0f, 100f) < _healthPickupChance)
                {
                    Instantiate(_healthPickupToDrop, transform.position + new Vector3 (.25f, 0f, 0f), _healthPickupToDrop.transform.rotation);
                }
            }
        }
    }
}
