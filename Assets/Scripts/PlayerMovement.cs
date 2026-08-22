using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movimiento")]
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public float rotationSpeed = 10f;

    [Header("Inventario / Galletas")]
    public int cookieCount = 0;

    [Header("Sistema de Asma / Salud")]
    public float maxHealth = 100f;
    public float currentHealth = 100f;
    public float passiveDrainRate = 1.5f;
    public float runningDrainRate = 5.0f;

    [Header("Inhalador")]
    public float inhalerCharge = 100f;
    public float healAmount = 30f;
    public float inhalerCostPerUse = 25f;

    [Header("Referencias")]
    public Animator animator;

    private float currentSpeed;
    private bool isRunning;
    private float x, y;

    void Start()
    {
        currentHealth = maxHealth;
    }

    void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        x = 0f;
        y = 0f;

        if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) x -= 1f;
        if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) x += 1f;
        if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) y -= 1f;
        if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) y += 1f;

        Vector3 moveDirection = new Vector3(x, 0f, y).normalized;
        isRunning = (keyboard.leftShiftKey.isPressed || keyboard.rightShiftKey.isPressed) && moveDirection.magnitude > 0f;

        if (keyboard.eKey.wasPressedThisFrame)
        {
            UseInhaler();
        }

        currentSpeed = isRunning ? runSpeed : walkSpeed;

        if (moveDirection.magnitude > 0f)
        {
            transform.Translate(moveDirection * currentSpeed * Time.deltaTime, Space.World);
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }

        float currentDrain = isRunning ? runningDrainRate : passiveDrainRate;
        DeductHealth(currentDrain * Time.deltaTime);

        float movementMagnitude = moveDirection.magnitude;
        float animSpeed = isRunning ? movementMagnitude * 2f : movementMagnitude;

        if (animator != null)
        {
            animator.SetFloat("speedY", animSpeed);
        }
    }

    public void AddCookies(int amount)
    {
        cookieCount += amount;
        Debug.Log($"Galletas recolectadas: {cookieCount}");
    }

    public bool UseCookie()
    {
        if (cookieCount > 0)
        {
            cookieCount--;
            Debug.Log($"Galleta entregada. Restantes: {cookieCount}");
            return true;
        }
        return false;
    }

    public void DeductHealth(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    public void UseInhaler()
    {
        if (inhalerCharge >= inhalerCostPerUse && currentHealth < maxHealth)
        {
            inhalerCharge -= inhalerCostPerUse;
            currentHealth = Mathf.Min(currentHealth + healAmount, maxHealth);
        }
    }

    private void Die()
    {
        Debug.Log("El jugador se ha quedado sin aire.");
    }
}