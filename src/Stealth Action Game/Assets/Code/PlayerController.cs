using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Code
{// TODO: Refactor into separate components for movement, aiming, rolling and weapon logic.
    public class PlayerController : MonoBehaviour
    {
        [field: SerializeField] public float MoveSpeed { get; private set; }
        [SerializeField] private InputActionReference _moveAction;
        [SerializeField] private InputActionReference _walkAction;
        [SerializeField] private float _walkSpeed;
        private float _activeMoveSpeed;

        [SerializeField] private InputActionReference _rollAction;
        [SerializeField] private float _rollSpeed, _rollLength;
        private float _rollCounter;

        [SerializeField] private InputActionReference _aimAction, _lookAction;
        private Vector2 _lastMousePosition;

        [SerializeField] private GameObject _weapon;
        [SerializeField] private InputActionReference _shootAction;
        [SerializeField] private Transform _shootPoint;
        [SerializeField] private GameObject _shootEffect, _impactEffect;
        [SerializeField] private float _range;

        [SerializeField] private CharacterController _characterController;
        [SerializeField] private Transform _model;
        [SerializeField] private float _turnSpeed;
        private float _ySpeed;
        [SerializeField] private float _gravityScale = 5f;

        [SerializeField] private Animator _animator;

        [SerializeField] private int _currentAmmo, _remainingAmmo, _clipSize;
        [SerializeField] private InputActionReference _reloadAction;
        [SerializeField] private float _reloadTime = 2.3f;

        private UIController _uiController;

        private void Start()
        {
            _activeMoveSpeed = MoveSpeed;

            _uiController = FindFirstObjectByType<UIController>();

            _uiController.UpdateAmmoText(_currentAmmo, _remainingAmmo);

            if (_weapon.activeSelf == true)
            {
                _weapon.SetActive(false);
            }

            if (_animator.GetBool("Reloading") == true)
            {
                _animator.SetBool("Reloading", false);
            }
        }

        private void Update()
        {
            if (_walkAction.action.IsPressed())
            {
                _activeMoveSpeed = _walkSpeed;
            }
            else
            {
                _activeMoveSpeed = MoveSpeed;
            }

            Vector2 moveInput = _moveAction.action.ReadValue<Vector2>();

            if (_rollAction.action.WasPressedThisFrame() && _aimAction.action.IsPressed() == false)
            {
                if (_rollCounter <= 0)
                {
                    _animator.SetTrigger("Roll");

                    _rollCounter = _rollLength;
                }
            }

            if (moveInput != Vector2.zero && _aimAction.action.IsPressed() == false)
            {
                Quaternion lookDirection = Quaternion.LookRotation(new Vector3(moveInput.x, 0, moveInput.y));
                _model.rotation = Quaternion.Slerp(_model.rotation, lookDirection, _turnSpeed * Time.deltaTime);
            }

            if (_aimAction.action.IsPressed() && _animator.GetBool("Reloading") == false)
            {
                moveInput = Vector2.zero;

                _rollCounter = 0f;

                _animator.SetBool("Aiming", true);

                _weapon.SetActive(true);

                Vector3 aimDirection = Vector3.zero;

                if (_lastMousePosition != Mouse.current.position.value)
                {

                    Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(
                        Mouse.current.position.value.x,
                        Mouse.current.position.value.y,
                        Camera.main.transform.position.y - _model.position.y));

                    mousePosition.y = _model.position.y;

                    Debug.Log(mousePosition);

                    aimDirection = mousePosition - _model.position;
                }

                _lastMousePosition = Mouse.current.position.value;

                Vector2 lookInput = _lookAction.action.ReadValue<Vector2>();
                if (lookInput != Vector2.zero)
                {
                    aimDirection = new Vector3(lookInput.x, 0, lookInput.y);
                }

                if (aimDirection != Vector3.zero)
                {
                    _model.rotation = Quaternion.LookRotation(aimDirection);
                }

                if (_shootAction.action.WasPressedThisFrame())
                {
                    if (_currentAmmo > 0)
                    {
                        _animator.SetTrigger("Shoot");

                        Instantiate(_shootEffect, _shootEffect.transform.position, _shootEffect.transform.rotation).SetActive(true);

                        RaycastHit hit;

                        if (Physics.Raycast(_shootPoint.position, _shootPoint.forward, out hit, _range))
                        {
                            Debug.Log("Выстрел: " + hit.transform.name);

                            Instantiate(_impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                        }

                        _currentAmmo--;

                        _uiController.UpdateAmmoText(_currentAmmo, _remainingAmmo);
                    }
                    else
                    {
                        StartCoroutine(Reload());
                    }
                }
            }
            else
            {
                _animator.SetBool("Aiming", false);

                if (_animator.GetBool("Reloading") == false)
                {
                    _weapon.SetActive(false);
                }
            }

            if (_reloadAction.action.WasPressedThisFrame())
            {
                StartCoroutine(Reload());
            }

            if (_characterController.isGrounded)
            {
                _ySpeed = 0f;
            }

            _ySpeed += Physics.gravity.y * Time.deltaTime * _gravityScale;

            if (_rollCounter > 0)
            {
                _rollCounter -= Time.deltaTime;
                _activeMoveSpeed = _rollSpeed;

                Vector3 rollDirection = _model.forward;
                moveInput.x = rollDirection.x;
                moveInput.y = rollDirection.z;
            }

            _characterController.Move(new Vector3(moveInput.x * _activeMoveSpeed, _ySpeed, moveInput.y * _activeMoveSpeed) * Time.deltaTime);

            _animator.SetFloat("Speed", moveInput.magnitude * _activeMoveSpeed);

            _lastMousePosition = Mouse.current.position.value;
        }

        private IEnumerator Reload()
        {
            if (_currentAmmo == _clipSize || _remainingAmmo <= 0)
            {
                yield break;
            }

            _weapon.SetActive(true);

            int refillAmount = _clipSize - _currentAmmo;

            if (_remainingAmmo >= refillAmount)
            {
                _remainingAmmo -= refillAmount;

            }
            else
            {
                refillAmount = _remainingAmmo;
                _remainingAmmo = 0;
            }

            _currentAmmo += refillAmount;

            _animator.SetBool("Aiming", false);

            _animator.SetBool("Reloading", true);

            yield return new WaitForSeconds(_reloadTime);

            _animator.SetBool("Reloading", false);

            if (_aimAction.action.IsPressed())
            {
                _animator.SetBool("Aiming", true);
            }

            _uiController.UpdateAmmoText(_currentAmmo, _remainingAmmo);
        }
    }
}
