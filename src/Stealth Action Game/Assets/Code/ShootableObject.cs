using UnityEngine;

public class ShootableObject : MonoBehaviour
{
    [SerializeField] private GameObject _destroyEffect;

    [SerializeField] private GameObject _ammoToDrop;
    [Range(0f, 100f)]
    [SerializeField] private float _ammoChance;

    public void DestroyObject()
    {
        Destroy(gameObject);
        Instantiate(_destroyEffect, transform.position, transform.rotation);

        if (_ammoToDrop != null)
        {
            if (Random.Range(0f, 100f) < _ammoChance)
            {
                Instantiate(_ammoToDrop, transform.position, _ammoToDrop.transform.rotation);
            }
        }
    }
}
