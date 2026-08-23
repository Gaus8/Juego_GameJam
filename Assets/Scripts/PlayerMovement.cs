using System.Collections; // Necesario para Coroutines (IEnumerator)
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement; // Necesario para reiniciar la escena

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

    [Header("Muerte")]
    [Tooltip("Tiempo en segundos que espera antes de reiniciar la escena.")]
    public float restartDelay = 3.0f;

    [Header("Referencias")]
    public Animator animator;

    private float currentSpeed;
    private bool isRunning;
    private bool isDead = false;
    private float x, y;

    void Start()
    {
        currentHealth = maxHealth;
    }

    void Update()
    {
        // Si está muerto, bloquea las acciones y el movimiento
        if (isDead) return;

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

        float targetAnimSpeed = 0f;
        if (moveDirection.magnitude > 0f)
        {
            targetAnimSpeed = isRunning ? 2f : 1f;
        }

        if (animator != null)
        {
            float currentAnimSpeed = animator.GetFloat("speedY");
            float smoothedSpeed = Mathf.MoveTowards(currentAnimSpeed, targetAnimSpeed, Time.deltaTime * 5f);
            animator.SetFloat("speedY", smoothedSpeed);
        }
    }

    public void GetScared()
    {
        if (isDead) return;
        if (animator != null)
        {
            animator.SetTrigger("isScared");
        }
    }

    public void Interact()
    {
        if (isDead) return;
        if (animator != null)
        {
            animator.SetTrigger("interact");
        }
    }

    public void AddCookies(int amount)
    {
        if (isDead) return;
        cookieCount += amount;
        Interact();
        Debug.Log($"Galletas recolectadas: {cookieCount}");
    }

    public bool UseCookie()
    {
        if (isDead) return false;
        if (cookieCount > 0)
        {
            cookieCount--;
            Interact();
            Debug.Log($"Galleta entregada. Restantes: {cookieCount}");
            return true;
        }
        return false;
    }

    public void DeductHealth(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    public void UseInhaler()
    {
        if (isDead) return;

        if (inhalerCharge >= inhalerCostPerUse && currentHealth < maxHealth)
        {
            inhalerCharge -= inhalerCostPerUse;
            currentHealth = Mathf.Min(currentHealth + healAmount, maxHealth);

            if (animator != null)
            {
                animator.SetTrigger("useInhaler");
            }
        }
    }

    private void Die()
    {
        isDead = true;
        Debug.Log("El jugador se ha quedado sin aire.");

        if (animator != null)
        {
            animator.SetTrigger("die");
        }

        // Inicia la cuenta regresiva para reiniciar la escena
        StartCoroutine(DieRoutine());
    }

    private IEnumerator DieRoutine()
    {
        // Espera los segundos asignados en restartDelay antes de recargar
        yield return new WaitForSeconds(restartDelay);

        // Carga de nuevo la escena que está activa en este momento
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
    // Agrega este método dentro de PlayerMovement.cs
    public void RechargeInhaler(float amount)
    {
        if (isDead) return;

        inhalerCharge += amount;
        inhalerCharge = Mathf.Clamp(inhalerCharge, 0f, 100f); // Evita sobrepasar el 100%
        Debug.Log($"Inhalador recargado en {amount}%. Carga actual: {inhalerCharge}%");
    }
}