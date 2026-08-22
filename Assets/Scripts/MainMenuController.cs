using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainMenuController : MonoBehaviour
{
    [Header("Paneles del Menú")]
    public GameObject mainMenuPanel;
    public GameObject instruccionesPanel;
    public GameObject creditosPanel;

    [Header("Configuración de Escena")]
#if UNITY_EDITOR
    [Tooltip("Arrastra aquí el archivo de la escena a la que quieres cambiar")]
    public SceneAsset escenaDelJuego;
#endif

    // Almacena el nombre internamente para la compilación final (Build)
    [SerializeField, HideInInspector]
    private string nombreEscenaGuardado;

    private void OnValidate()
    {
#if UNITY_EDITOR
        // Actualiza automáticamente el nombre si arrastras una escena diferente en el Inspector
        if (escenaDelJuego != null)
        {
            nombreEscenaGuardado = escenaDelJuego.name;
        }
#endif
    }

    public void Jugar()
    {
        if (!string.IsNullOrEmpty(nombreEscenaGuardado))
        {
            SceneManager.LoadScene(nombreEscenaGuardado);
        }
        else
        {
            Debug.LogError("No se ha asignado ninguna escena en el Inspector del MainMenuController.");
        }
    }

    public void MostrarInstrucciones()
    {
        mainMenuPanel.SetActive(false);
        instruccionesPanel.SetActive(true);
    }

    public void MostrarCreditos()
    {
        mainMenuPanel.SetActive(false);
        creditosPanel.SetActive(true);
    }

    public void VolverAlMenuPrincipal()
    {
        instruccionesPanel.SetActive(false);
        creditosPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void SalirDelJuego()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }
}