using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

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

        public enum EnemyState
        {
            Normal,
            Alerted,
            Chase,
            Searching
        }
        [SerializeField] private EnemyState _currentState;

        [SerializeField] private float _waitToChase, _waitToSearch;
        private float _chaseCounter, _searchCounter;
        [SerializeField] private float _searchLength;
        private Vector3 _searchPosition;

        private float _lookAroundCounter, _lookAroundAngle;
        [SerializeField] private float _searchTurnSpeed;

        [SerializeField] private float _chaseSpeed, _walkSpeed;
        private float _chaseAdjustCounter;

        [SerializeField] private float _timeToShoot, _moveAfterShootTime;
        private float _shootWaitCounter, _shootMoveCounter, _waitToShoot;
        [SerializeField] private Transform _shootPoint;
        [SerializeField] private GameObject _projectile;


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

            switch (_currentState)
            {
                case EnemyState.Normal:
                    Patrol();
                    break;

                case EnemyState.Alerted:

                    if (_canSeePlayer == true)
                    {
                        _searchPosition = _player.position;

                        transform.LookAt(new Vector3(_player.position.x, transform.position.y, _player.position.z));

                        _chaseCounter -= Time.deltaTime;

                        if (_chaseCounter <= 0 || Vector3.Distance(transform.position, _player.position) < _playerViewRange / 2)
                        {
                            StartChasing();
                        }
                    }
                    else
                    {
                        _searchCounter -= Time.deltaTime;

                        if (_searchCounter <= 0)
                        {
                            StartSearching();
                        }
                    }

                    break;

                case EnemyState.Chase:

                    if (_shootWaitCounter > 0)
                    {
                        if (_canSeePlayer == true)
                        {
                            _chaseAdjustCounter -= Time.deltaTime;

                            if (_chaseAdjustCounter <= 0)
                            {
                                _agent.SetDestination(_player.position);

                                _chaseAdjustCounter = 0.2f;
                            }
                        }

                        _shootWaitCounter -= Time.deltaTime;
                        if (_shootWaitCounter <= 0)
                        {
                            _shootMoveCounter = _moveAfterShootTime;
                            _agent.isStopped = true;

                            _animator.SetTrigger("Aiming");
                            _animator.SetBool("Chase", false);

                            _waitToShoot = _moveAfterShootTime * 0.25f;
                        }
                    }
                    else
                    {
                        _shootMoveCounter -= Time.deltaTime;
                        if (_shootMoveCounter <= 0)
                        {
                            _shootWaitCounter = _timeToShoot;

                            _agent.isStopped = false;
                            _agent.SetDestination(_player.position);

                            _animator.SetBool("Chase", true);
                            _chaseAdjustCounter = 0.2f;
                        }

                        if (_waitToShoot > 0)
                        {
                            _waitToShoot -= Time.deltaTime;
                            if (_waitToShoot <= 0)
                            {
                                _animator.SetTrigger("Shoot");

                                Instantiate(_projectile, _shootPoint.position, _shootPoint.rotation);
                            }
                        }

                        transform.LookAt(new Vector3(_player.position.x, transform.position.y, _player.position.z));
                    }

                    if (_canSeePlayer == false)
                    {
                        _searchPosition = _player.position;

                        _animator.SetBool("Chase", true);

                        StartSearching();
                    }

                    break;

                case EnemyState.Searching:

                    SearchingProcess();

                    if (_canSeePlayer == true)
                    {
                        StartChasing();
                    }

                    break;
            }
        }

        private void SearchingProcess()
        {
            if (Vector3.Distance(transform.position, _searchPosition) < .5f)
            {
                _agent.isStopped = true;

                _animator.SetBool("Walking", false);
                _animator.SetBool("Chase", false);

                _searchCounter -= Time.deltaTime;

                if (_searchCounter <= 0)
                {
                    _currentState = EnemyState.Normal;

                    _agent.isStopped = false;
                    _agent.SetDestination(_patrolPoints[_currentPatrolPoint].position);

                    _animator.SetBool("Walking", true);
                    _agent.speed = _walkSpeed;
                }

                _lookAroundCounter -= Time.deltaTime;

                if (_lookAroundCounter <= 0)
                {
                    _lookAroundCounter = Random.Range(_searchLength * .25f, _searchLength * .5f);

                    _lookAroundAngle = Random.Range(-180, 180);
                }

                transform.rotation = Quaternion.RotateTowards(transform.rotation,
                    Quaternion.Euler(0, _lookAroundAngle, 0),
                    _searchTurnSpeed * Time.deltaTime);
            }
        }

        private void StartChasing()
        {
            _currentState = EnemyState.Chase;
            _animator.SetBool("Chase", true);
            _animator.SetBool("Walking", false);

            _agent.SetDestination(_player.position);
            _agent.isStopped = false;

            _agent.speed = _chaseSpeed;

            _chaseAdjustCounter = 0.2f;

            _shootWaitCounter = _timeToShoot;
        }

        private void StartSearching()
        {
            _currentState = EnemyState.Searching;

            _searchCounter = _searchLength;

            _agent.SetDestination(_searchPosition);
            _agent.isStopped = false;
            _agent.speed = _walkSpeed;

            _animator.SetBool("Chase", false);
            _animator.SetBool("Walking", true);

            _lookAroundCounter = 0;
        }

        private void Patrol()
        {
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

                if (_canSeePlayer == true)
                {
                    _currentState = EnemyState.Alerted;

                    _animator.SetBool("Walking", false);

                    _agent.isStopped = true;

                    _chaseCounter = _waitToChase;

                    _searchCounter = _waitToSearch;

                    _searchPosition = _player.position;
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
                    if (Physics.Raycast(transform.position + new Vector3(0, 1.5f, 0), directionToTarget, out RaycastHit hit, _playerViewRange))
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
