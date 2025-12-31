using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointTrigger : MonoBehaviour
{
    public int levelIndex;
    private bool triggered = false;

    [Header("Görsel Kök (isteðe baðlý)")]
    public GameObject visualRoot; // Yalnýzca belirli bir child'ý gizlemek istersen

    private Renderer[] renderers;

    void Awake()
    {
        // visualRoot atanmýþsa onu; yoksa bu objenin tüm alt renderer'larýný al
        var root = visualRoot != null ? visualRoot.transform : transform;
        renderers = root.GetComponentsInChildren<Renderer>(true);
    }

    void HideVisual()
    {
        if (renderers == null) return;
        foreach (var r in renderers)
            r.enabled = false; // sadece görseli gizle
    }

    void ShowVisual()
    {
        if (renderers == null) return;
        foreach (var r in renderers)
            r.enabled = true; // görseli geri getir
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        if (other.CompareTag("Player"))
        {
            // LevelManager'a haber ver
            LevelManager lm = FindObjectOfType<LevelManager>();
            if (lm != null)
                lm.OnCheckpointReached(levelIndex);

            // Oyuncunun respawn noktasýný güncelle
            PlayerController pc = other.GetComponent<PlayerController>();
            if (pc != null)
                pc.respawnPoint = transform;

            // SFX
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlaySfx(SfxType.Checkpoint);

            // Yalnýzca görseli gizle (objeyi kapatmýyoruz)
            HideVisual();

            triggered = true;
        }
    }

    // Oyuncu yeniden doðduðunda (respawn) yýldýzý geri göstermek için
    public void RestoreVisualOnRespawn()
    {
        triggered = false;
        ShowVisual();
    }
}
