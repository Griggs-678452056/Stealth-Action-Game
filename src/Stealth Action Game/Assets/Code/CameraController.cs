using UnityEngine;

namespace Code
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Transform _player;

        private Vector3 _offset;

        private float _moveSpeed;

        [SerializeField] private bool _snapToPosition;

        private PlayerController _playerController;

        private void Start()
        {
            _offset = transform.position - _player.position;
            _playerController = FindAnyObjectByType<PlayerController>();
            _moveSpeed = _playerController.MoveSpeed;
        }

        private void LateUpdate()
        {
            if (_snapToPosition == false)
            {
                transform.position = Vector3.Lerp(transform.position, _player.position + _offset, _moveSpeed * Time.deltaTime);
            }
            else
            {
                transform.position = _player.position + _offset;
            }
        }
    }
}
