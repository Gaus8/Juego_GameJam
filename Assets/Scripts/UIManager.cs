using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Elementos de UI (Barras Images)")]
    public Image healthBarImage;  // Asigna el Fill de EstaminaBar
    public Image inhalerBarImage; // Asigna el Fill de InhaladorBar
    
    [Header("Referencias")]
    public PlayerMovement player; 

    [Header("Elementos de UI (Texto TMP)")]
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI cookieText;
    public TextMeshProUGUI inhalerText;

    [Header("Elementos de UI opcionales (Barras Sliders)")]
    public Slider healthSlider;  
    public Slider inhalerSlider; 

    void Start()
    {
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

        // 4. Actualizar BARRA DE VIDA (Image Filled)
        if (healthBarImage != null && player.maxHealth > 0)
        {
            healthBarImage.fillAmount = player.currentHealth / player.maxHealth;
        }

        // 5. Actualizar BARRA DE INHALADOR (Image Filled)
        if (inhalerBarImage != null)
        {
            inhalerBarImage.fillAmount = player.inhalerCharge / 100f; // Asumiendo que la carga máxima es 100
        }

        // 6. Actualizar Sliders viejos (opcional)
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