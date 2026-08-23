using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Elementos de UI (Barras)")]
    public Image healthBarImage; // Asigna aquí el objeto "Fill" de EstaminaBar

    [Header("Referencias")]
    public PlayerMovement player; // Referencia al jugador

    [Header("Elementos de UI (Texto TMP)")]
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI cookieText;
    public TextMeshProUGUI inhalerText;

    [Header("Elementos de UI opcionales (Barras Sliders)")]
    public Slider healthSlider;  // Opcional
    public Slider inhalerSlider; // Opcional

    void Start()
    {
        // Configurar los rangos máximos de las barras si están asignadas
        if (player != null)
        {
            if (healthSlider != null) healthSlider.maxValue = player.maxHealth;
            if (inhalerSlider != null) inhalerSlider.maxValue = 100f;
        }
    }

    void Update()
    {
        if (player == null) return;

        // 1. Actualizar Texto de Vida / Salud
        if (healthText != null)
        {
            healthText.text = $"Salud: {Mathf.CeilToInt(player.currentHealth)} / {player.maxHealth}";
        }

        // 2. Actualizar Texto de Galletas
        if (cookieText != null)
        {
            cookieText.text = $":{player.cookieCount}";
        }

        // 3. Actualizar Texto de Inhalador
        if (inhalerText != null)
        {
            inhalerText.text = $"Inhalador: {Mathf.CeilToInt(player.inhalerCharge)}%";
        }

        // 4. Actualizar la NUEVA BARRA DE VIDA (Image Filled)
        if (healthBarImage != null && player.maxHealth > 0)
        {
            healthBarImage.fillAmount = player.currentHealth / player.maxHealth;
        }

        // 5. Actualizar Sliders viejos (si los sigues usando)
        if (healthSlider != null) healthSlider.value = player.currentHealth;
        if (inhalerSlider != null) inhalerSlider.value = player.inhalerCharge;
    }

    public void UpdateHealth(float currentHealth, float maxHealth)
    {
        if (healthBarImage != null && maxHealth > 0)
        {
            healthBarImage.fillAmount = currentHealth / maxHealth;
        }
    }
}