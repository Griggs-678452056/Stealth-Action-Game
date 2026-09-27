using Code;
using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    [SerializeField] private int _amountToAdd;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>().GetAmmo(_amountToAdd);

            Destroy(gameObject);
        }
    }
}
