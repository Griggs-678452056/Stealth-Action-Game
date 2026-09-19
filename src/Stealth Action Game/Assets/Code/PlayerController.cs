using UnityEngine;
using UnityEngine.InputSystem;

namespace Code
{
    public class PlayerController : MonoBehaviour
    {
        [field: SerializeField] public float MoveSpeed { get; private set; }
        [SerializeField] private InputActionReference _moveAction;
        [SerializeField] private InputActionReference _walkAction;
        [SerializeField] private float _walkSpeed;
        private float _activeMoveSpeed;
        [SerializeField] private InputActionReference _rollAction;
        [SerializeField] private float _rollSpeed, _rollLength;

        [SerializeField] private CharacterController _characterController;
        [SerializeField] private Transform _model;

        [SerializeField] private float _turnSpeed;

        private float _ySpeed;
        [SerializeField] private float _gravityScale = 5f;

        [SerializeField] private Animator _animator;

        private void Start()
        {
            _activeMoveSpeed = MoveSpeed;
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

            if (_rollAction.action.WasPressedThisFrame())
            {
                _animator.SetTrigger("Roll");
            }

            if (moveInput != Vector2.zero)
            {
                Quaternion lookDirection = Quaternion.LookRotation(new Vector3(moveInput.x, 0, moveInput.y));
                _model.rotation = Quaternion.Slerp(_model.rotation, lookDirection, _turnSpeed * Time.deltaTime);
            }

            if (_characterController.isGrounded)
            {
                _ySpeed = 0f;
            }

            _ySpeed += Physics.gravity.y * Time.deltaTime * _gravityScale;

            _characterController.Move(new Vector3(moveInput.x * _activeMoveSpeed, _ySpeed, moveInput.y * _activeMoveSpeed) * Time.deltaTime);

            _animator.SetFloat("Speed", moveInput.magnitude * _activeMoveSpeed);
        }
    }
}

