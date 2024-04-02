using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPSController : MonoBehaviour
{
    private CharacterController _controller;
    private Transform _camera;
    private float _horizontal;
    private float _vertical;
    [SerializeField] private float _playerSpeed = 5;
    [SerializeField] private float _jumpHeight = 1;


    //Crouch
    [SerializeField] private GameObject HeadPosition;
    [SerializeField] public bool _crouch = false;
    [SerializeField] private bool _canStand;

    private float _gravity = -9.81f;
    private Vector3 _playerGravity;

    private float turnSmoothVelocity;
    [SerializeField] float turnSmoothTime = 0.1f;

    [SerializeField] private Transform _sensorPosition;
    [SerializeField] private float _sensorRadius = 0.2f;
    [SerializeField] private LayerMask _groundLayer;
    private bool _isGrounded;
    private Animator _animator;

    [SerializeField] private float _throwForce = 10;
    public GameObject objectToGrab;
    private GameObject grabedObject;
    [SerializeField] private Transform _interactionZone;
    [SerializeField] private float _pushForce = 5;

    //Escalada
    public float velocidadElevacion = 5f; 
    private bool activarElevacion = false;
    

    //Disparo
    [SerializeField] Transform gunPosition;
    [SerializeField] int ammo;
    public GameObject bullet;
    
 void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _camera = Camera.main.transform;
        _animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        _horizontal = Input.GetAxisRaw("Horizontal");
        _vertical = Input.GetAxisRaw("Vertical");
        Movement();
        Jump();
        if(Input.GetKeyDown(KeyCode.E))
        {
            GrabObject();
        }
        if(Input.GetButtonDown("Fire1") && grabedObject != null)
        {
            ThrowObject();
        }
        Crouch();
        //Disparo
        if (Input.GetMouseButtonDown(0))
        {
            if(ammo > 0)
            {
                Instantiate(bullet, gunPosition.position, gunPosition.rotation);
                ammo = ammo -1;
            }
        }
        if (activarElevacion && Input.GetKey(KeyCode.L))
        {
            transform.Translate(Vector3.up * velocidadElevacion * Time.deltaTime);
        }
    }
    public void ActivarElevacion(bool activar)
    {
        activarElevacion = activar;
    }
    
    void Movement()
    {
        _crouch = false;
        Vector3 direction = new Vector3(_horizontal, 0, _vertical);
        _animator.SetFloat("VelX", 0);
        _animator.SetFloat("VelZ", direction.magnitude);

        if(direction != Vector3.zero)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + _camera.eulerAngles.y;
            float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0, smoothAngle, 0);
            Vector3 moveDirection = Quaternion.Euler(0,targetAngle, 0) * Vector3.forward;
            _controller.Move(moveDirection.normalized * _playerSpeed * Time.deltaTime);
        }
    }
    void Crouch()
    {
        if (Physics.Raycast(HeadPosition.transform.position, Vector3.up, 0.5f))
        {
            _canStand = false;
            Debug.DrawRay(HeadPosition.transform.position, Vector3.up, Color.red);
        }
        else
        {
            _canStand = true;
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            if(_crouch == true && _canStand == true)
            {
                _crouch = false;
                _animator.SetBool("IsCrouching", false);
                _jumpHeight = 1;
                _controller.height = 2f;
                _playerSpeed = 5;
                _controller.center = new Vector3(0f, 0f, 0f);
                
            }
            else
            {
                _crouch = true;
                _animator.SetBool("IsCrouching", true);
                _jumpHeight = 0;
                _controller.height = 1.5f;
                _playerSpeed = 2;
                _controller.center = new Vector3(0f, -0.2f, 0f);
            }
        }
    }

    void Jump()
    {
        _isGrounded = Physics.CheckSphere(_sensorPosition.position, _sensorRadius, _groundLayer);
        _animator.SetBool("IsJumping", !_isGrounded);

        if(_isGrounded && _playerGravity.y < 0)
        {
            _playerGravity.y = -2;
        }
        if(_isGrounded && Input.GetButtonDown("Jump"))
        {
            _playerGravity.y = Mathf.Sqrt(_jumpHeight * -2 * _gravity);
        }
        _playerGravity.y += _gravity * Time.deltaTime;
        _controller.Move(_playerGravity * Time.deltaTime);
    }
    void GrabObject()
    {
        if(objectToGrab != null && grabedObject == null)
        {
            grabedObject = objectToGrab;
            grabedObject.transform.SetParent(_interactionZone);
            grabedObject.transform.position = _interactionZone.position;
            grabedObject.GetComponent<Rigidbody>().isKinematic = true;
        }
        else if(grabedObject != null)
        {
            grabedObject.GetComponent<Rigidbody>().isKinematic = false;
            grabedObject.transform.SetParent(null);
            grabedObject = null;
        }
    }

    void ThrowObject()
    {
        Rigidbody grabedBody = grabedObject.GetComponent<Rigidbody>();

        grabedBody.isKinematic = false;
        grabedObject.transform.SetParent(null);
        grabedBody.AddForce(_controller.transform.forward * _throwForce, ForceMode.Impulse);
        grabedObject = null;
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;

        if(body == null|| body.isKinematic)
        {
            return;
        }

        if(hit.moveDirection.y < -0.2f)
        {
            return;
        }
        Vector3 pushDirection = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
        body.velocity = pushDirection * _pushForce / body.mass;
    }
 
}
