using UnityEngine;

public class EnemyController : MonoBehaviour
{   
    public void TakeDamage()
    {
        Destroy(gameObject);
    }
}
