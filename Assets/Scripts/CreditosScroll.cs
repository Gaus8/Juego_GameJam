using UnityEngine;

public class CreditosScroll : MonoBehaviour
{
    [SerializeField] private float velocidad = 40f;          // Velocidad del desplazamiento
    [SerializeField] private float posicionInicialY = -300f; // Punto de inicio (abajo)
    [SerializeField] private float posicionFinalY = 600f;    // Límite para reiniciar (arriba)

    private RectTransform rectTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void OnEnable()
    {
        ResetearPosicion();
    }

    void Update()
    {
        // Desplaza el texto hacia arriba
        rectTransform.anchoredPosition += new Vector2(0, velocidad * Time.deltaTime);

        // Reinicia en bucle al llegar arriba
        if (rectTransform.anchoredPosition.y >= posicionFinalY)
        {
            ResetearPosicion();
        }
    }

    public void ResetearPosicion()
    {
        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, posicionInicialY);
        }
    }
}