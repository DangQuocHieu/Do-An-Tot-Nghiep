using UnityEngine;
using UnityEngine.InputSystem;

public class FirstPersonController : MonoBehaviour
{
    [Header("Base Properties")]
    //Move
    [SerializeField] private float _moveSpeed = 4.0f;
    [SerializeField] private float _speedChangeRate = 10f;

    //Look
    [Space(10)]
    private const float _threshold = 0.01f;
    [SerializeField] private float _rotationSpeed = 1.0f;
    [SerializeField] private float _topClamp = 90.0f;
    [SerializeField] private float _bottomClamp = -90f;

    [Header("Runtime Properties")]
    //Move
    private float _currentSpeed;
    private float _rotationVelocity;

    //Look
    [SerializeField] private float _targetPitch;

    private bool IsCurrentDeviceMouse
    {
        get
        {
            return _playerInput.currentControlScheme == "KeyboardMouse";
        }
    }


    [Header("References")]
    [SerializeField] private CharacterController _controller;
    [SerializeField] private PlayerInput _playerInput;
    [SerializeField] private PlayerInputs _playerInputs;
    [SerializeField] private Transform _camera;
    [SerializeField] private Rigidbody _playerRb;

    private void Update()
    {
        Move();
    }

    private void LateUpdate()
    {
        HandleLook();
    }
    private void Move()
    {
        float targetSpeed = _moveSpeed;
        if(_playerInputs.Move == Vector2.zero ) targetSpeed = 0.0f;
        float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;
        //float inputMagnitude = _playerInputs.AnalogMovement ? _playerInputs.Move.magnitude : 1f;  
        if (currentHorizontalSpeed < targetSpeed - 0.1f || currentHorizontalSpeed > targetSpeed + 0.1f)
        {
            _currentSpeed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed, Time.deltaTime * _speedChangeRate);
            _currentSpeed = Mathf.Round(_currentSpeed * 1000f) / 1000f;
        }
        else
        {
            _currentSpeed = targetSpeed;    
        }
        Vector3 inputDirection = new Vector3(_playerInputs.Move.x, 0.0f, _playerInputs.Move.y).normalized;
        if(_playerInputs.Move != Vector2.zero)
        {
            inputDirection = transform.right * _playerInputs.Move.x + transform.forward * _playerInputs.Move.y;
        }
        _playerRb.linearVelocity = inputDirection * (_currentSpeed * Time.deltaTime);
        _controller.Move(inputDirection * (_currentSpeed * Time.deltaTime));
    }

    private void HandleLook()
    {
        if(_playerInputs.Look.sqrMagnitude >= _threshold)
        {
            _targetPitch += _playerInputs.Look.y * _rotationSpeed;
            _rotationVelocity = _playerInputs.Look.x * _rotationSpeed;
            _targetPitch = ClampAngle(_targetPitch, _bottomClamp, _topClamp);
            _camera.localRotation = Quaternion.Euler(-_targetPitch, 0.0f, 0.0f);
            transform.Rotate(Vector3.up * _rotationVelocity);

        }
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
}
