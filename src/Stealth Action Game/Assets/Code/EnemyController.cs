using UnityEngine;
using UnityEngine.AI;

namespace Code
{
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

        private Transform _player;
        private bool _canSeePlayer;
        [SerializeField] private float _playerViewAngle, _playerViewRange;

        private void Start()
        {
            _patrolPointsHolder.SetParent(null);

            if (_patrolPoints.Length > 0)
            {
                _agent.SetDestination(_patrolPoints[0].position);

                _waitCounter = _pointWaitTime;

                _animator.SetBool("Walking", true);
            }

            _player = FindFirstObjectByType<PlayerController>().transform;
        }

        private void Update()
        {
            CheckForPlayer();

            if (_patrolPoints.Length > 0)
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

        private void CheckForPlayer()
        {
            _canSeePlayer = false;

            if (Vector3.Distance(transform.position, _player.position) < _playerViewRange)
            {
                Vector3 directionToTarget = (_player.position - transform.position).normalized;

                if (Vector3.Angle(transform.forward, directionToTarget) < _playerViewAngle / 2)
                {
                    if (Physics.Raycast(transform.position + new Vector3 (0, 1.5f, 0), directionToTarget, out RaycastHit hit, _playerViewRange))
                    {
                        if (hit.collider.CompareTag("Player"))
                        {
                            _canSeePlayer = true;
                        }
                    }

                    Debug.DrawRay(transform.position + new Vector3(0, 1.5f, 0), directionToTarget * _playerViewRange, Color.purple);
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            
            Vector3 endPosition = Vector3.zero;

            endPosition.x = Mathf.Sin(Mathf.Deg2Rad * ((_playerViewAngle / 2) + transform.rotation.eulerAngles.y)) * _playerViewRange;
            endPosition.z = Mathf.Cos(Mathf.Deg2Rad * ((_playerViewAngle / 2) + transform.rotation.eulerAngles.y)) * _playerViewRange;

            Gizmos.DrawLine(transform.position, endPosition + transform.position);

            endPosition.x = Mathf.Sin(Mathf.Deg2Rad * ((-_playerViewAngle / 2) + transform.rotation.eulerAngles.y)) * _playerViewRange;
            endPosition.z = Mathf.Cos(Mathf.Deg2Rad * ((-_playerViewAngle / 2) + transform.rotation.eulerAngles.y)) * _playerViewRange;

            Gizmos.DrawLine(transform.position, endPosition + transform.position);
        }
    }
}
