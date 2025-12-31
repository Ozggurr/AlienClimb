using UnityEngine;
using UnityEngine.UI;

public class SoundToggleSFX : MonoBehaviour
{
    public GameObject iconOn;
    public GameObject iconOff;

    void Start()
    {
        bool muted = AudioManager.Instance.SfxMuted;
        SetIcons(muted);

        GetComponent<Button>().onClick.AddListener(() =>
        {
            bool nowMuted = AudioManager.Instance.ToggleSFX();
            SetIcons(nowMuted);
        });
    }

    void SetIcons(bool muted)
    {
        if (iconOn) iconOn.SetActive(!muted);
        if (iconOff) iconOff.SetActive(muted);
    }
}