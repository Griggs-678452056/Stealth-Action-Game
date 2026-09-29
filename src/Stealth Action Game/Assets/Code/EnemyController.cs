using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private NavMeshAgent _agent;

    [SerializeField] private Transform _patrolPointsHolder;
    [SerializeField] private Transform[] _patrolPoints;
    private int _currentPatrolPoint;

    [SerializeField] private float _pointWaitTime;
    private float _waitCounter;

    [SerializeField] private Animator _animator;

    [SerializeField] private int _health = 3;

    private void Start()
    {
        _patrolPointsHolder.SetParent(null);

        _agent.SetDestination(_patrolPoints[0].position);

        _waitCounter = _pointWaitTime;

        _animator.SetBool("Walking", true);
    }

    private void Update()
    {
        if (Vector3.Distance(transform.position,
            new Vector3(_patrolPoints[_currentPatrolPoint].position.x,
            transform.position.y,
            _patrolPoints[_currentPatrolPoint].position.z)) < .1f)
        {
            _animator.SetBool("Walking", false);

            _waitCounter -= Time.deltaTime;

            if (_waitCounter <= 0)
            {
                _waitCounter = _pointWaitTime;

                _currentPatrolPoint++;

                if (_currentPatrolPoint >= _patrolPoints.Length)
                {
                    _currentPatrolPoint = 0;
                }

                _agent.SetDestination(_patrolPoints[_currentPatrolPoint].position);

                _animator.SetBool("Walking", true);
            }
        }
    }

    public void TakeDamage()
    {
        _health--;

        if (_health <= 0)
        {
            _animator.SetTrigger("Dead");

            _agent.enabled = false;
            enabled = false;

            GetComponent<Collider>().enabled = false;
        }
    }
}
