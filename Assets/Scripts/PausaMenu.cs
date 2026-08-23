using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem; // Si usas el New Input System

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI; // Arrastra tu MenuPausaPanel aqu�
    public GameObject panelInstucciones;

    public static bool isPaused = false;

    void Update()
    {
        // Detecta la tecla Escape (Nuevo Input System)
        var keyboard = Keyboard.current;
        if (keyboard != null && keyboard.escapeKey.wasPressedThisFrame)
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f; // Reanuda la f�sica y el tiempo
        isPaused = false;

        // Oculta y bloquea el cursor para el juego
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f; // Congela la f�sica y la simulaci�n
        isPaused = true;

        // Libera el cursor para poder hacer clic en los botones
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; // Asegura reactivar el tiempo antes de recargar
        isPaused = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Time.timeScale = 1f; // Reestablece la velocidad del juego
        isPaused = false;

        // Muestra el cursor para usar el men� principal
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Carga la escena del men� principal
        SceneManager.LoadScene("mainMenu");
    }

    public void OpenPanelInstructions()
    {
        panelInstucciones.SetActive(true);
    }
      public void ClosePanelInstructions()
    {
        panelInstucciones.SetActive(false);
    }
}