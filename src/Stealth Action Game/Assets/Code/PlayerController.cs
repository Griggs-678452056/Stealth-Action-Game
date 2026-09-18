using UnityEngine;
using UnityEngine.InputSystem;

namespace Code
{
    public class PlayerController : MonoBehaviour
    {
        [field: SerializeField] public float MoveSpeed { get; private set; }
        [SerializeField] private InputActionReference _moveAction;

        [SerializeField] private CharacterController _characterController;

        [SerializeField] private Transform _model;

        [SerializeField] private float _turnSpeed;

        private float _ySpeed;

        [SerializeField] private float _gravityScale = 5f;

        private void Start()
        {

        }

        private void Update()
        {
            Vector2 moveInput = _moveAction.action.ReadValue<Vector2>();

            float x = moveInput.x * MoveSpeed;

            float z = moveInput.y * MoveSpeed;

            if (moveInput != Vector2.zero)
            {
                Quaternion lookDirection = Quaternion.LookRotation(new Vector3(moveInput.x, 0, moveInput.y));
                _model.rotation = Quaternion.Slerp(_model.rotation, lookDirection, _turnSpeed * Time.deltaTime);
            }

            if(_characterController.isGrounded)
            {
                _ySpeed = 0f;
            }

            _ySpeed += Physics.gravity.y * Time.deltaTime * _gravityScale;

            _characterController.Move(new Vector3(x, _ySpeed, z) * Time.deltaTime);
        }
    }
}

