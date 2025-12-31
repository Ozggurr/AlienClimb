using UnityEngine;
using UnityEngine.UI;

public class SoundToggleMusic : MonoBehaviour
{
    public GameObject iconOn;
    public GameObject iconOff;

    void Start()
    {
        bool muted = AudioManager.Instance.MusicMuted;
        SetIcons(muted);

        GetComponent<Button>().onClick.AddListener(() =>
        {
            bool nowMuted = AudioManager.Instance.ToggleMusic();
            SetIcons(nowMuted);
         //denbug
            AudioManager.Instance.DebugDump();
        });
    }

    void SetIcons(bool muted)
    {
        if (iconOn) iconOn.SetActive(!muted);
        if (iconOff) iconOff.SetActive(muted);
    }
}