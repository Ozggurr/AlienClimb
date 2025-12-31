using UnityEngine;
using UnityEngine.UI;
using System.Linq;
public class UIButtonClickSFX : MonoBehaviour
{
    [Tooltip("Butona basýldýðýnda çalacak SFX tipi")]
    public SfxType type = SfxType.Button;

    void Start()
    {
        Button btn = GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.AddListener(() =>
            {
                if (AudioManager.Instance != null)
                    AudioManager.Instance.PlaySfx(type);
            });
        }
    }
}