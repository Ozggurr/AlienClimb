using UnityEngine;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour
{
    [Header("UI Refs")]
    public Button btnPause;           // Canvas/BtnPause
    public GameObject panelPause;     // Canvas/PanelPause

    [Header("Groups to disable when paused")]
    public GameObject gameUIGroup;    // Canvas/GameUI (oyun içi butonlar)
    public GameObject joystickRoot;   // Canvas/JoystickRoot (veya joystiðin parent'ý)

    private bool isPaused = false;

    void Start()
    {
        if (panelPause) panelPause.SetActive(false);
        if (btnPause) btnPause.onClick.AddListener(TogglePause);
    }

    void Update()
    {
        // (PC testleri için) ESC ile de toggle
        if (Input.GetKeyDown(KeyCode.Escape))
            TogglePause();
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            if (panelPause) panelPause.SetActive(true);
            if (gameUIGroup) gameUIGroup.SetActive(false);  // oyun butonlarýný kapat
            if (joystickRoot) joystickRoot.SetActive(false); // joystick'i kapat
            Time.timeScale = 0f;                             // oyunu durdur
        }
        else
        {
            if (panelPause) panelPause.SetActive(false);
            if (gameUIGroup) gameUIGroup.SetActive(true);   // oyun butonlarýný aç
            if (joystickRoot) joystickRoot.SetActive(true);  // joystick'i aç
            Time.timeScale = 1f;                             // oyunu sürdür
        }
    }
}
