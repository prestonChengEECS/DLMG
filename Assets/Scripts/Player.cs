using UnityEngine;

public class Player : MonoBehaviour
{
    private Animator animator;
    private bool isGrounded;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f; //determines how far 
    public LayerMask groundLayer;
    public Transform wallCheck; //bascially like ground checking but for the sides of the character. 
    public float wallCheckRadius = 0.2f;
    public LayerMask wallLayer;
    public float moveSpeed = 3f;
    private Rigidbody2D rb;
    private float horizontalInput;
    private float verticalInput;
    public float jumpForce = 10f;
    public bool inventoryOpen = false; //starts off as false because the inventory starts off as hidden. 
    public bool collectablesOpen = false;
    public float springForce = 5.0f;

    // --- STATE MANAGEMENT EDITS START HERE ---
    public enum Playerstate { Idle, Walking, Climbing }
    public Playerstate currentState = Playerstate.Idle;
    // --- STATE MANAGEMENT EDITS END HERE ---

    // Start is called once before the first execution of Update after the MonoBehaviour is created 
    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); //gets the 2D Rigidbody to the GameObject that this script is attached to 
        animator = GetComponent<Animator>(); //gets the animator to the GameObject that this script is attached to 
    }

    // Update is called once per frame 
    void Update()
    {
        inventoryOpen = InventoryUI.instance.inventoryPanel.activeSelf;
        collectablesOpen = InventoryUI.instance.collectiblesPanel.activeSelf;

        if (!inventoryOpen && !collectablesOpen)
        {
            horizontalInput = Input.GetAxis("Horizontal");
            verticalInput = Input.GetAxis("Vertical");
        }
        else
        {
            horizontalInput = 0; //if any of these are open, kill the input immediately and stop moving. 
            verticalInput = 0;
        }

        // Shared Ground Logic (Jumping and OverlapCircle work when Idle or Walking)
        if (currentState == Playerstate.Idle || currentState == Playerstate.Walking)
        {
            animator.SetFloat("Speed", Mathf.Abs(horizontalInput));
            /* * Physics2D.OverlapCircle takes in 3 parameters. * OverlapCircle is used to check for 2D colliders within a circular area in the world space. * * 1. point => the center of the circle world coordinates. Here it is an empty game object and since groundCheck is public, we assign it in the Unity editor. * 2. radius => the distance from the center to the edge of the circle. A large float value would mean we look farther out from the center. * 3. layerMask => Used to filter objects only on specific layers. in this case we filter by only groundLayers. */
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer); //if the inventory is open, then halt all processes... 

            //jumping mechanic. checks to see if on ground first. 
            if (Input.GetKeyDown(KeyCode.W) && isGrounded && !inventoryOpen && !collectablesOpen)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                isGrounded = false;
            }
        }

        // State Machine Handling Transitions & Animations
        if (currentState == Playerstate.Idle)
        {
            //when the player is not running revert back to the idle animation 
            animator.Play("Idle");

            // If player starts moving horizontally, switch to Walking state
            if (horizontalInput != 0 && !inventoryOpen && !collectablesOpen)
            {
                currentState = Playerstate.Walking;
            }
        }
        else if (currentState == Playerstate.Walking)
        {
            //flips the character direction. 
            if (horizontalInput > 0 && !inventoryOpen && !collectablesOpen)
            {
                //if d is pressed face right 
                transform.eulerAngles = new Vector3(0, 0, 0);
                animator.Play("Run");
            }
            else if (horizontalInput < 0 && !inventoryOpen && !collectablesOpen)
            {
                //if a is pressed face left 
                transform.eulerAngles = new Vector3(0, 180, 0);
                animator.Play("Run");
            }

            // If player stops moving horizontally, switch back to Idle state
            if (horizontalInput == 0)
            {
                currentState = Playerstate.Idle;
            }
        }
    }

    void FixedUpdate()
    {
        if (currentState == Playerstate.Idle || currentState == Playerstate.Walking)
        {
            rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }

    //handles bouncing on the SPRING pad. 
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Spring Pad"))
        {
            Debug.Log("boing"); //gets the Spring script that is attached to the spring pad base so you can check to see if it is still bouncing or not. 
            Spring springComponent = collision.gameObject.GetComponent<Spring>();
            if (springComponent != null && !springComponent.isBouncing)
            {
                rb.AddForce(Vector2.up * springForce, ForceMode2D.Impulse);
            }
        }
    }
}
