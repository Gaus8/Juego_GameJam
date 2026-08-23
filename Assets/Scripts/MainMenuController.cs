using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainMenuController : MonoBehaviour
{
    [Header("Paneles del Menu")]
    public GameObject mainMenuPanel;
    public GameObject panelInstructions;
    public GameObject panelCredits;

    [Header("Configuraci�n de Escena")]
#if UNITY_EDITOR
    [Tooltip("Arrastra aqu� el archivo de la escena a la que quieres cambiar")]
    public SceneAsset escenaDelJuego;
#endif

    // Almacena el nombre internamente para la compilaci�n final (Build)
    [SerializeField, HideInInspector]
    private string nombreEscenaGuardado;

    private void OnValidate()
    {
#if UNITY_EDITOR
        // Actualiza autom�ticamente el nombre si arrastras una escena diferente en el Inspector
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
    
        panelInstructions.SetActive(true);
    }

    public void MostrarCreditos()
    {
    
        panelCredits.SetActive(true);
    }

    public void VolverAlMenuPrincipal()
    {
        panelInstructions.SetActive(false);
        panelCredits.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void SalirDelJuego()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }

    public void CerrarModalInstrucciones()
    {
         panelInstructions.SetActive(false);
    }

     public void CerrarModalCreditos()
    {
         panelCredits.SetActive(false);
    }
}