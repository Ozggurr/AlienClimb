
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RoundedProgressBar : MonoBehaviour
{
    [Header("Refs")]
    public RectTransform progressBarBG;  // ProgressBar_BG (RectTransform)
    public RectTransform fill;           // Fill (RectTransform)
    public TMP_Text percentText;         // PercentText (opsiyonel)

    [Header("Padding")]
    [Range(0f, 20f)] public float horizontalPadding = 2f;
    // Fill’in iki yandan biraz içerde baþlamasý için (oval kenara deðmesin)

    [Header("Checkpoint Progress (optional)")]
    [Min(1)] public int totalCheckpoints = 5; // Inspector’dan el ile ayarlarsýn
    public bool useDebugDriver = true;        // debugPercent ile elde sürmek istiyorsan açýk kalsýn
    private int reached = 0;                  // benzersiz alýnan checkpoint sayýsý

    // --- Mevcut bar güncelleme fonksiyonun (hiç bozmadým) ---
    public void SetPercent01(float t)
    {
        t = Mathf.Clamp01(t);

        // BG geniþliði
        float bgWidth = progressBarBG.rect.width;

        // Hedef geniþlik (kenarlardan azcýk içerde dursun)
        float target = Mathf.Max(0f, (bgWidth - horizontalPadding * 2f) * t);

        // Fill’ý sola sabitlediðimiz için sadece width’i artýrmak/düþürmek yeterli
        var size = fill.sizeDelta;
        size.x = target;
        size.y = progressBarBG.rect.height; // ayný yükseklikte tut
        fill.sizeDelta = size;

        if (percentText != null)
        {
            int pct = Mathf.RoundToInt(t * 100f);
            percentText.text = pct + "%";
        }
    }

    // --- Yeni: Checkpoint entegrasyonu için küçük API ---
    public void ResetProgress(int total)
    {
        totalCheckpoints = Mathf.Max(1, total);
        reached = 0;
        SetPercent01(0f);
    }

    public void OnCheckpointReachedOnce()
    {
        reached = Mathf.Clamp(reached + 1, 0, totalCheckpoints);
        float t = (float)reached / totalCheckpoints;
        SetPercent01(t);
    }

    // Test amaçlý: Play modda inspector’dan kaydýrmak için
    [Range(0f, 1f)] public float debugPercent = 0f;
    void Update()
    {
        // Sahnede testi kolaylaþtýrmak için. Kapamak istersen useDebugDriver=false yap.
        if (useDebugDriver)
            SetPercent01(debugPercent);
    }
}