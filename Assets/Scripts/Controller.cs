using System;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public Camera mainCamera;
    
    public Transform cameraPosition;
    
    [Header("Control Settings")]
    public float mouseSensitivity = 100.0f;
    public float playerSpeed = 5.0f;
    public float runningSpeed = 7.0f;
    public float jumpSpeed = 5.0f;
    
    float _verticalSpeed = 0.0f;
    
    float _verticalAngle, _horizontalAngle;
    public float speed { get; private set; } = 0.0f;

    public bool lockControl { get; set; } = false;

    public bool grounded => _grounded;

    CharacterController _CharacterController;

    bool _grounded;
    float _groundedTimer;
    float _speedAtJump = 0.0f;
    Vector3 move;
    float actualSpeed;
    Vector3 currentAngles;


    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        _grounded = true;
        
        mainCamera.transform.SetParent(cameraPosition, false);
        mainCamera.transform.localPosition = Vector3.zero;
        mainCamera.transform.localRotation = Quaternion.identity;
        _CharacterController = GetComponent<CharacterController>();
        
        _verticalAngle = 0.0f;
        _horizontalAngle = transform.localEulerAngles.y;
    }

    void Update()
    {
        bool wasGrounded = _grounded;
        bool loosedGrounding = false;
        
        //we define our own grounded and not use the Character controller one as the character controller can flicker
        //between grounded/not grounded on small step and the like. So we actually make the controller "not grounded" only
        //if the character controller reported not being grounded for at least .5 second;
        if (!_CharacterController.isGrounded)
        {
            if (_grounded)
            {
                _groundedTimer += Time.deltaTime;
                if (_groundedTimer >= 0.5f)
                {
                    loosedGrounding = true;
                    _grounded = false;
                }
            }
        }
        else
        {
            _groundedTimer = 0.0f;
            _grounded = true;
        }

        speed = 0;
        move = Vector3.zero;
        if (!lockControl)
        {
            // Jump (we do it first as 
            if (_grounded && Input.GetButtonDown("Jump"))
            {
                _verticalSpeed = jumpSpeed;
                _grounded = false;
                loosedGrounding = true;
            }
            
            bool running = Input.GetButton("Run");
            actualSpeed = running ? runningSpeed : playerSpeed;

            if (loosedGrounding)
            {
                _speedAtJump = actualSpeed;
            }

            // Move around with WASD
            MovePlayer();

            // Turn player
            TurnPlayer();

            // Camera look up/down
            CameraLookUpDown();
                
            speed = move.magnitude / (playerSpeed * Time.deltaTime);
        }

        // Fall down / gravity
        _verticalSpeed = _verticalSpeed - 10.0f * Time.deltaTime;
        if (_verticalSpeed < -10.0f)
            _verticalSpeed = -10.0f; // max fall speed
        var verticalMove = new Vector3(0, _verticalSpeed * Time.deltaTime, 0);
        var flag = _CharacterController.Move(verticalMove);
        if ((flag & CollisionFlags.Below) != 0)
            _verticalSpeed = 0;

    }

    private void MovePlayer()
    {
        move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        if (move.sqrMagnitude > 1.0f)
            move.Normalize();

        float usedSpeed = _grounded ? actualSpeed : _speedAtJump;

        move = move * usedSpeed * Time.deltaTime;

        move = transform.TransformDirection(move);
        _CharacterController.Move(move);
    }

    private void TurnPlayer()
    {
        float turnPlayer = Input.GetAxis("Mouse X") * mouseSensitivity;
        _horizontalAngle = _horizontalAngle + turnPlayer;

        if (_horizontalAngle > 360) _horizontalAngle -= 360.0f;
        if (_horizontalAngle < 0) _horizontalAngle += 360.0f;

        currentAngles = transform.localEulerAngles;
        currentAngles.y = _horizontalAngle;
        transform.localEulerAngles = currentAngles;
    }

    private void CameraLookUpDown()
    {
        var turnCam = -Input.GetAxis("Mouse Y");
        turnCam = turnCam * mouseSensitivity;
        _verticalAngle = Mathf.Clamp(turnCam + _verticalAngle, -89.0f, 89.0f);
        currentAngles = cameraPosition.transform.localEulerAngles;
        currentAngles.x = _verticalAngle;
        cameraPosition.transform.localEulerAngles = currentAngles;
    }
}
