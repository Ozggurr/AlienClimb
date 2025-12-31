using UnityEngine;
using UnityEngine.SceneManagement;

public class LosePanelButtons : MonoBehaviour
{
    public PlayerController player; // Inspector’dan sürükle

    public void OnClickRetry()
    {
        // Lose paneli kapat
        if (LosePanelController.Instance)
            LosePanelController.Instance.HideLose();

        // Oyuncuyu yeniden dogur
        if (player != null)
        {
            player.Respawn();        // Respawn sistemi PlayerController'daki mantýkla çalýþýr
            player.ResetLoseFlag();  // Panel tekrar açýlabilsin diye
        }
    }

    public void OnClickExitToMenu()
    {
        if (LosePanelController.Instance)
            LosePanelController.Instance.HideLose();

        SceneManager.LoadScene("MainMenu");
    }

    public void OnClickStore()
    {
        Debug.Log("Magaza acilacak...");
        // Buraya maðaza paneli veya sahne çaðrýsý gelecek
    }
}