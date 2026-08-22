using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movimiento")]
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public float rotatingSpeed = 70f;

    [Header("Sistema de Asma / Salud")]
    public float maxHealth = 100f;
    public float currentHealth = 100f;
    public float passiveDrainRate = 1.5f;   // Consumo por segundo al caminar/estar quieto
    public float runningDrainRate = 5.0f;   // Consumo por segundo al correr
    public float scareDamage = 25f;          // Daño instantáneo por un susto

    [Header("Inhalador")]
    public float inhalerCharge = 100f;       // Porcentaje total restante del inhalador (0 - 100)
    public float healAmount = 30f;           // Cuánta salud recupera una dosis
    public float inhalerCostPerUse = 25f;    // Porcentaje de batería/dosis consumido por uso

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

        // 1. Lectura de Entradas
        x = 0f;
        y = 0f;

        if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) x -= 1f;
        if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) x += 1f;
        if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) y -= 1f;
        if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) y += 1f;

        // Detectar si está corriendo (Shift Izquierdo o Derecho)
        isRunning = (keyboard.leftShiftKey.isPressed || keyboard.rightShiftKey.isPressed) && y > 0f;

        // Usar Inhalador con la tecla E
        if (keyboard.eKey.wasPressedThisFrame)
        {
            UseInhaler();
        }

        // 2. Control de Movimiento
        currentSpeed = isRunning ? runSpeed : walkSpeed;

        transform.Rotate(0, x * Time.deltaTime * rotatingSpeed, 0);
        transform.Translate(0, 0, y * Time.deltaTime * currentSpeed);

        // 3. Sistema de Consumo de Asma continuo
        float currentDrain = isRunning ? runningDrainRate : passiveDrainRate;
        DeductHealth(currentDrain * Time.deltaTime);

        // 4. Parámetros de Animación
        animator.SetFloat("speedX", x);
        animator.SetFloat("speedY", isRunning ? y * 2f : y); // Pasa 2 a la animación si corre
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
            Debug.Log($"Inhalador usado. Carga restante: {inhalerCharge}%. Salud: {currentHealth}");
        }
        else if (inhalerCharge < inhalerCostPerUse)
        {
            Debug.Log("¡El inhalador está vacío!");
        }
    }

    // Trigger para detectar objetos que asustan (Susto de golpe)
  

    private void Die()
    {
        Debug.Log("El jugador se ha quedado sin aire.");
        // Lógica de Game Over aquí
    }
}