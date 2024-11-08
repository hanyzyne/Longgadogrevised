using UnityEngine;
using UnityEngine.UI; // Include this for UI components

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float sprintMultiplier = 2f;
    public float knockbackForce = 10f;
    public float knockbackDuration = 0.2f; // Duration of the knockback effect
    public float stunDuration = 2f; // Duration of the stun effect
    public string inputButtonX;
    public string inputButtonY;
    public KeyCode sprintKey;

    public float maxStamina = 100f; // Maximum stamina
    public float currentStamina; // Current stamina
    public float sprintStaminaCost = 5f; // Stamina cost per second while sprinting
    public float staminaRegenRate = 5f; // Stamina regeneration per second

    public Slider staminaSlider; // Reference to the stamina slider

    private Rigidbody2D rb;
    private Vector2 movement;
    private bool isSprinting;
    private bool isKnockedBack = false; // Track knockback state
    private bool isStunned = false; // Track stun state

    private PlayerInteraction playerInteraction; // Reference to PlayerInteraction component

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerInteraction = GetComponent<PlayerInteraction>(); // Get the PlayerInteraction component
        currentStamina = maxStamina; // Initialize stamina
        UpdateStaminaUI(); // Initialize stamina UI
    }

    void Update()
    {
        if (!isKnockedBack && !isStunned) // Only handle input if not knocked back or stunned
        {
            HandleInput();
        }

        // Regenerate stamina over time
        if (currentStamina < maxStamina)
        {
            currentStamina += staminaRegenRate * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina); // Ensure it doesn't exceed max stamina
            UpdateStaminaUI(); // Update UI when stamina changes
        }
    }

    void FixedUpdate()
    {
        if (!isKnockedBack && !isStunned) // Only move if not knocked back or stunned
        {
            Move();
        }
    }

    private void HandleInput()
    {
        movement.x = Input.GetAxisRaw(inputButtonX);
        movement.y = Input.GetAxisRaw(inputButtonY);

        // Check if the player is sprinting and has enough stamina
        isSprinting = Input.GetKey(sprintKey) && currentStamina > 1;

        if (isSprinting)
        {
            // Deduct stamina while sprinting in whole numbers
            currentStamina -= sprintStaminaCost * Time.deltaTime; // Deduct stamina based on the time spent sprinting
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina); // Ensure stamina doesn't go negative
            currentStamina = Mathf.Floor(currentStamina); // Round down to the nearest whole number
            UpdateStaminaUI(); // Update UI when stamina changes
        }
    }

    private void Move()
    {
        Vector2 moveDir = movement.normalized * moveSpeed * (isSprinting ? sprintMultiplier : 1f);
        rb.MovePosition(rb.position + moveDir * Time.fixedDeltaTime);
    }

    public void Knockback(Vector2 direction)
    {
        isKnockedBack = true;
        rb.AddForce(direction * knockbackForce, ForceMode2D.Impulse);

        // Drop held item if there is one
        if (playerInteraction != null)
        {
            playerInteraction.DropItem();
        }

        Invoke(nameof(ResetKnockback), knockbackDuration); // Reset knockback after a short duration
    }

    public void Stun()
    {
        isStunned = true;
        Invoke(nameof(ResetStun), stunDuration); // Reset stun after the stun duration
    }

    private void ResetKnockback()
    {
        isKnockedBack = false;
    }

    private void ResetStun()
    {
        isStunned = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && isSprinting)
        {
            Vector2 knockbackDirection = collision.transform.position - transform.position;
            knockbackDirection.Normalize();

            // Apply knockback and stun to the other player
            PlayerMovement hitPlayer = collision.gameObject.GetComponent<PlayerMovement>();
            if (hitPlayer != null)
            {
                hitPlayer.Knockback(knockbackDirection);
                hitPlayer.Stun(); // Stun the hit player for 2 seconds
            }
        }
    }

    private void UpdateStaminaUI()
    {
        if (staminaSlider != null)
        {
            staminaSlider.value = currentStamina; // Update the slider's value
        }
    }
}
