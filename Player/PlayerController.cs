using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


//PlayerController is the central nervous system of the player. It handles almost all of the components in one way or another.
//As such, all of the player-centric components (which is to say - most of the game) will thus be in contact with the player controller.
//I'll be limiting dependencies by doing this - most of the scripts will depend on the character controller, but little else.
//my goal is to have about 3 or 4 dependencies per script, components notwithstanding. 
public class PlayerController : MonoBehaviour
{
    //reminder: add headers

    //maybe add keys/control bindings here?

    //variables
    [Header("Locomotion Variables")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float runSpeed;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float animationBlendTime = .2f;
    [SerializeField] private float jumpForce = 4f;
    private Vector3 moveDirection;

    [Header("Gravity & Jumping Handler")]
    [SerializeField] private float gravityForce = -9.81f;
    [SerializeField] private float groundedGravityForce = -2f;

    //components
    [Header("Components")]
    [SerializeField] private CharacterController myCharacterController;
    public Animator playerAnimator;
    private Rigidbody playerRigidBody;
    public Camera mainCamera;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheckTransform;
    [SerializeField] private float groundCheckLength;
    [SerializeField] private LayerMask groundLayer;
    private bool isGrounded;

    [Header("Player Climber")]
    [SerializeField] private float climbSpeed = 1f;
    [SerializeField] private float climbingCheckLength = .5f;
    [SerializeField] private Transform climbingCheckTransform;
    [SerializeField] private LayerMask climbableLayer;
    [SerializeField] private bool isClimbing = false;

    [Header("Debug")]
    [SerializeField] private bool debug; //only used for debugging

    //light prefab and variables
    [Header("Lantern")]
    [SerializeField] private GameObject playerLantern;
    private bool lanternOn = false;

    //private variables


    private void Awake()
    {
        playerRigidBody = GetComponent<Rigidbody>();
    }
    void Start()
    {
        playerLantern.SetActive(false);

        if (myCharacterController == null)
        {
            myCharacterController = GetComponent<CharacterController>();
        }
        if (playerAnimator == null)
        {
            playerAnimator = GetComponentInChildren<Animator>();
        }
        moveSpeed = walkSpeed;
    }

    void Update()
    {

        CheckIsGrounded();
        HandleGravity();

        Movement();
        HandleLocomotionAnimations();
        Jump();
        TryEnterClimbMode();
        ToggleLantern();

        if (IsRunning())
        {
            moveSpeed = runSpeed;
        }
        else if (!IsRunning())
        {
            moveSpeed = walkSpeed;
        }
    }

    private void Movement()
    {
        //basic movement handling
        float xInput = Input.GetAxis("Horizontal");
        float yInput = Input.GetAxis("Vertical");

        if(!isClimbing) 
        {
            moveDirection = ((transform.right * xInput) + (transform.forward * yInput)).normalized * Time.deltaTime;
            myCharacterController.Move(moveDirection * moveSpeed);
        }
        //handles climbing - I might need to refactor it later
        else if(isClimbing) 
        {
            moveDirection = ((transform.right * xInput) + (transform.up * yInput)).normalized * Time.deltaTime;
            myCharacterController.Move(moveDirection * climbSpeed);
        }
    }

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Debug.Log("Is Jumping");
            myCharacterController.Move(Vector3.up * jumpForce);
        }
    }

    private bool IsRunning()
    {
        if (Input.GetKey(KeyCode.LeftShift) && isGrounded)
        {
            return true;
        }
        else return false;
    }

    private void HandleLocomotionAnimations()
    {
        playerAnimator.SetFloat("ForwardMoveSpeed", myCharacterController.velocity.magnitude, animationBlendTime, Time.deltaTime);
    }

    private void HandleGravity()
    {
        if (isGrounded)
        {
            moveDirection.y = groundedGravityForce;
        }
        else if (!isGrounded && !isClimbing)
        {
            moveDirection.y += gravityForce;
            myCharacterController.Move(moveDirection * Time.deltaTime);
        }
        else if(isClimbing) 
        {
            moveDirection.y = 0f;
        }
    }

    private void CheckIsGrounded()
    {
        if (Physics.Raycast(groundCheckTransform.position, -groundCheckTransform.up, groundCheckLength, groundLayer))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    //climbing functionality

    private void TryEnterClimbMode() 
    {
        if (Input.GetKeyDown(KeyCode.C) && !isClimbing)
        {
            if(Physics.Raycast(climbingCheckTransform.position, climbingCheckTransform.forward, climbingCheckLength, climbableLayer)) 
            {
                isClimbing = true;
            }
        }
        else if(Input.GetKeyDown(KeyCode.C) && isClimbing) 
        {
            isClimbing = false;
            return;
        }

        if(isClimbing && !Physics.Raycast(climbingCheckTransform.position, climbingCheckTransform.forward, climbingCheckLength, climbableLayer)) 
        {
            isClimbing = false;
        }
        else if(isClimbing && Input.GetKeyDown(KeyCode.Space))  
        {
            myCharacterController.Move(transform.up * jumpForce * 2.5f); //the magic number should become wall jump multiplier up in the variables section
        }
    }

    private void ToggleLantern() 
    {
        if(Input.GetKeyDown(KeyCode.L) && !lanternOn) 
        {
            playerLantern.SetActive(true);
            lanternOn = true;
        }
        else if(Input.GetKeyDown(KeyCode.L) && lanternOn) 
        {
            playerLantern.SetActive(false);
            lanternOn = false;
        }
    }

    private bool ClimbingCheckDebug() 
    {
        return Physics.Raycast(climbingCheckTransform.position, climbingCheckTransform.forward, climbingCheckLength, climbableLayer);
    }
}
 
