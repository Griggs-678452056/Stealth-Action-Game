using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _moveSpeed;
    [SerializeField] private InputActionReference _moveAction;

    [SerializeField] private CharacterController _characterController;

    [SerializeField] private Transform _model;

    private void Start()
    {

    }

    private void Update()
    {
        Vector2 moveInput = _moveAction.action.ReadValue<Vector2>();

        Quaternion lookDirection = Quaternion.LookRotation(new Vector3(moveInput.x, 0, moveInput.y));
        _model.rotation = lookDirection;

        _characterController.Move(new Vector3(moveInput.x, 0, moveInput.y) * _moveSpeed * Time.deltaTime);
    }
}
