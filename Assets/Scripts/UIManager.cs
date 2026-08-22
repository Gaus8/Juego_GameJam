using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Referencias")]
    public PlayerMovement player; // Referencia al jugador

    [Header("Elementos de UI (Texto TMP)")]
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI cookieText;
    public TextMeshProUGUI inhalerText;

    [Header("Elementos de UI opcionales (Barras)")]
    public Slider healthSlider;  // Opcional: si usas una barra para la vida
    public Slider inhalerSlider; // Opcional: si usas una barra para el inhalador

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

        // 4. Actualizar Barras Sliders (si se están utilizando)
        if (healthSlider != null) healthSlider.value = player.currentHealth;
        if (inhalerSlider != null) inhalerSlider.value = player.inhalerCharge;
    }
}