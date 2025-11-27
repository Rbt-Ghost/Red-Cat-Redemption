using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{   
    private bool facingRight=true;
     [SerializeField]
    private float movementThreshold = 0.01f; //Used for detecting movement.
    [SerializeField]
    private float moveSpeed = 5f; //The speed of movement
    [SerializeField]
    private SpriteRenderer playerSpriteRenderer; //Player's sprite
     [SerializeField]
    private PlayerStats playerStats;
    [SerializeField]
    private Animator playerAnimator; //Player's animator
    private Vector3 lastPosition; //To track last position for movement detection
     private Rigidbody2D rb; // Reference to the Rigidbody2D component (used for movement)

    [SerializeField]
    private DialogueManager dialogueManager;
    [SerializeField]
    private AudioSource footstepAudioSource; // Audio source for footstep sounds
    private float pitchRange;

    void Awake(){
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D not found on Player! Movement will not work correctly.");
        }
         if (playerStats == null)
        {
           
            playerStats = GetComponent<PlayerStats>();
        }
        if (playerStats == null)
        {
            Debug.LogError("PlayerStats script not found on PlayerMovement's GameObject! Movement will not stop on death.");
        }
        lastPosition = transform.position;
    }
    void FixedUpdate(){
        if (dialogueManager != null && dialogueManager.IsDialogueActive())
        {
            rb.linearVelocity = Vector2.zero;

            if (playerAnimator != null)
            {
                playerAnimator.SetBool("isMoving", false);
            }

            return;
        }

         if (playerStats != null && !playerStats.IsAlive())
        {
            rb.linearVelocity = Vector2.zero; 
         
            if (playerAnimator != null)
            {
                playerAnimator.SetBool("isMoving", false);
               
            }
            return;
        }
         // Get input for horizontal and vertical movement
         float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");   // W/S or Up/Down Arrow keys

        // Create a direction vector based on input
        Vector2 movementDirection = new Vector2(horizontalInput, verticalInput);

        // Normalize the vector to ensure consistent speed in all directions (including diagonals)
        // If not normalized, moving diagonally would be faster than moving horizontally/vertically
        movementDirection.Normalize();

        // Calculate the movement amount for this frame
        rb.linearVelocity = movementDirection * moveSpeed;
    }
    void Update()
    {
       if (playerStats != null && !playerStats.IsAlive())
        {
            return; 
        }

       if (dialogueManager != null && dialogueManager.IsDialogueActive())
        {
            playerAnimator.SetBool("isMoving", false);
            return;
        }

        // Handle footstep sound
        if (IsMoving() && playerStats.IsAlive() &&
            (dialogueManager == null || !dialogueManager.IsDialogueActive()))
        {
            if (!footstepAudioSource.isPlaying)
            {
                pitchRange = Random.Range(0.8f, 1.2f);
                footstepAudioSource.pitch = pitchRange;
                footstepAudioSource.Play();
            }
        }
        else
        {
            if (footstepAudioSource.isPlaying)
            {
                footstepAudioSource.Stop();
            }
        }


        float horizontalInput = Input.GetAxisRaw("Horizontal");
       //Animation. We use the horizontal input to determine the facing direction.
        if (playerAnimator != null)
        {
            if (horizontalInput > 0.01f) //To the right
            {
                playerSpriteRenderer.flipX = false; 
                facingRight = true;
            }
            else if (horizontalInput < -0.01f) //To the left
            {
                playerSpriteRenderer.flipX = true; 
                facingRight = false;
            }
           //If horizontal input is near zero, we don't change the flipX value (keep facing the same direction).
        }
        playerAnimator.SetBool("isMoving", IsMoving()); //Constantly update the isMoving parameter based on movement status.
        lastPosition = transform.position; //Update last position at the end of Update.
        
    }

    public bool IsFacingRight()
    {
        return facingRight;
    }

    public bool IsMoving()
    {
        //Calculate movement based on Rigidbody2D velocity for more accurate detection
        if (rb != null)
        {
            return rb.linearVelocity.magnitude > movementThreshold;
        }
        // Fallback if there is no rigidbody
        return Vector3.Distance(transform.position, lastPosition) > movementThreshold;
    }
}
