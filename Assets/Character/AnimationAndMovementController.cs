using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class AnimationAndMovementController : MonoBehaviour
{
    PlayerInput _playerInput;
    CharacterController _characterController;
    Animator _animator;
    Rigidbody _rigidbody;
    
    int _isWalkingHash;
    int _isRunningHash;

    Vector2 _currentMovementInput;
    Vector3 _currentMovement;
    Vector3 _currentRunMovement;
    bool _isMovementPressed;
    bool _isRunPassed;
    float _rotationFactorPreFrame = 15.0f;
    private float _runMultiplier = 3.0f;
    
    [SerializeField] GameObject stepRayUpper;
    [SerializeField] GameObject stepRayLower;
    [SerializeField] private float stepHeight = 0.3f;
    [SerializeField] private float stepSmooth = 0.1f;
    
    
    void Awake()
    {
        _playerInput = new PlayerInput();
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();

        _isWalkingHash = Animator.StringToHash("IsWalking");
        _isRunningHash = Animator.StringToHash("IsRunning");

        _playerInput.CharacterControls.Move.started += OnMovementInput;
        _playerInput.CharacterControls.Move.canceled += OnMovementInput;
        _playerInput.CharacterControls.Move.performed += OnMovementInput;
        _playerInput.CharacterControls.Run.started += OnRun;
        _playerInput.CharacterControls.Run.canceled += OnRun;

        stepRayUpper.transform.position = new Vector3(stepRayUpper.transform.position.x, stepHeight,
            stepRayUpper.transform.position.z);
    }

    void OnRun(InputAction.CallbackContext context)
    {
        _isRunPassed = context.ReadValueAsButton();
    }
    
    void OnMovementInput(InputAction.CallbackContext context)
    {
        _currentMovementInput = context.ReadValue<Vector2>();
        _currentMovement.x = _currentMovementInput.x;
        _currentMovement.z = _currentMovementInput.y;
        _currentRunMovement.x = _currentMovementInput.x * _runMultiplier;
        _currentRunMovement.z = _currentMovementInput.y * _runMultiplier;
        _isMovementPressed = _currentMovementInput.x != 0 || _currentMovementInput.y != 0;
    }

    void HandleRotation()
    {
        Vector3 positionToLookAt;

        positionToLookAt.x = _currentMovement.x;
        positionToLookAt.y = 0.0f;
        positionToLookAt.z = _currentMovement.z;
        
        Quaternion currentRotation = transform.rotation;
        
        if (_isMovementPressed)
        {
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, _rotationFactorPreFrame * Time.deltaTime);
        }
    }
    
    void HandleAnimation()
    {
        bool isWalking = _animator.GetBool(_isWalkingHash);
        bool isRunning = _animator.GetBool(_isRunningHash);

        if (_isMovementPressed && !isWalking)
        {
            _animator.SetBool(_isWalkingHash, true);
        }
        else if (!_isMovementPressed && isWalking)
        {
            _animator.SetBool(_isWalkingHash, false);
        }

        if ((_isMovementPressed && _isRunPassed) && !isRunning)
        {
            _animator.SetBool(_isRunningHash, true);
        }
        else if ((!_isMovementPressed || !_isRunPassed) && isRunning)
        {
            _animator.SetBool(_isRunningHash, false);
        }
       
    }

    void HandleGravity()
    {
        if (_characterController.isGrounded)
        {
            float groundGravity = -0.05f;
            _currentMovement.y = groundGravity;
            _currentRunMovement.y = groundGravity;
        }
        else
        {
            float gravity = -9.8f;
            _currentMovement.y += gravity;
            _currentRunMovement.y += gravity;
        }
    }
    
    void Update()
    {
        HandleRotation();
        HandleAnimation();
        StepClimb();

        if (_isRunPassed)
        {
            _characterController.Move(_currentRunMovement * Time.deltaTime);
        }
        else
        {
            _characterController.Move(_currentMovement * Time.deltaTime);
        }
    }

    void OnEnable()
    {
        _playerInput.CharacterControls.Enable();
    }

    void OnDisable()
    {
        _playerInput.CharacterControls.Disable();
    }

    void FixedUpdate()
    {
        StepClimb();
    }

    void StepClimb()
    {
        RaycastHit hitLower;
        if (Physics.Raycast(stepRayLower.transform.position, transform.TransformDirection(Vector3.forward), out hitLower, 0.1f))
        {
            RaycastHit hitUpper;
            if (!Physics.Raycast(stepRayUpper.transform.position, transform.TransformDirection(Vector3.forward), out hitUpper, 0.2f))
            {
                _rigidbody.position -= new Vector3(0f, -stepSmooth, 0f);
            }
        }
    }
}
