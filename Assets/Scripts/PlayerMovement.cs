using UnityEngine;
using UnityEngine.InputSystem; // 1. Add this namespace

public class PlayerMovement : MonoBehaviour
{
    public float runningSpeed = 4f;
    public float rotatingSpeed = 70f;

    public Animator animator;

    private float x, y;

    void Update()
    {

        // ACTUALMENT EL Input.GetAxis no está funcionando, era otro método

        // 2. Read inputs using the new Input System
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        x = 0f;
        y = 0f;

        if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) x -= 1f;
        if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) x += 1f;
        if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) y -= 1f;
        if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) y += 1f;

        // 3. Fixed swap: rotatingSpeed for rotation, runningSpeed for translation
        transform.Rotate(0, x * Time.deltaTime * rotatingSpeed, 0);
        transform.Translate(0, 0, y * Time.deltaTime * runningSpeed);

        animator.SetFloat("speedX", x);
        animator.SetFloat("speedY", y);
    }
}