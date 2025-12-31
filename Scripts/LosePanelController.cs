using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LosePanelController : MonoBehaviour
{
    public static LosePanelController Instance;

    [Header("UI")]
    public GameObject losePanel;   // Inspector: Kaybetme paneli (kapalý baþlasýn)
    public bool pauseTime = true;  // Panel açýlýnca oyunu durdurmak istersen

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        if (losePanel) losePanel.SetActive(false);
    }

    public void ShowLose()
    {
        // SFX (opsiyonel)
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySfx(SfxType.Lose);

        if (losePanel) losePanel.SetActive(true);
        if (pauseTime) Time.timeScale = 0f;
    }

    public void HideLose()
    {
        if (losePanel) losePanel.SetActive(false);
        if (pauseTime) Time.timeScale = 1f;
    }
}
