using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public enum SfxType { Jump, Button, Checkpoint, Lose }

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Mixer")]
    public AudioMixer mixer;               // GameMixer asset
    [Tooltip("AudioMixer'da expose ettiğin Müzik Vol param ismi")]
    public string musicParam = "MusicVol"; // GameMixer/Music -> Exposed: MusicVol
    [Tooltip("AudioMixer'da expose ettiğin SFX Vol param ismi")]
    public string sfxParam = "SFXVol";   // GameMixer/SFX   -> Exposed: SFXVol

    [Header("SFX Source")]
    public AudioSource sfxSource;          // Output -> GameMixer/SFX

    [Header("SFX Clips")]
    public AudioClip jumpClip;
    public AudioClip buttonClip;
    public AudioClip checkpointClip;
    public AudioClip loseClip;

    // PlayerPrefs Keys
    const string KEY_MUSIC = "musicMuted";
    const string KEY_SFX = "sfxMuted";

    // Durumlar
    public bool MusicMuted { get; private set; }
    public bool SfxMuted { get; private set; }

    Dictionary<SfxType, AudioClip> map;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        map = new Dictionary<SfxType, AudioClip>
        {
            { SfxType.Jump,       jumpClip },
            { SfxType.Button,     buttonClip },
            { SfxType.Checkpoint, checkpointClip },
            { SfxType.Lose,       loseClip }
        };

        MusicMuted = PlayerPrefs.GetInt(KEY_MUSIC, 0) == 1;
        SfxMuted = PlayerPrefs.GetInt(KEY_SFX, 0) == 1;

        ApplyMixer();
    }

    void ApplyMixer()
    {
        // 0 dB = açık, -80 dB ≈ kapalı
        if (mixer != null)
        {
            mixer.SetFloat(musicParam, MusicMuted ? -80f : 0f);
            mixer.SetFloat(sfxParam, SfxMuted ? -80f : 0f);
        }
    }

    public bool ToggleMusic()
    {
        MusicMuted = !MusicMuted;
        PlayerPrefs.SetInt(KEY_MUSIC, MusicMuted ? 1 : 0);
        PlayerPrefs.Save();
        ApplyMixer();
        return MusicMuted;
    }

    public bool ToggleSFX()
    {
        SfxMuted = !SfxMuted;
        PlayerPrefs.SetInt(KEY_SFX, SfxMuted ? 1 : 0);
        PlayerPrefs.Save();
        ApplyMixer();
        return SfxMuted;
    }

    public void PlaySfx(SfxType type, float volume = 1f)
    {
        if (!sfxSource) return;
        if (!map.TryGetValue(type, out var clip) || clip == null) return;
        sfxSource.PlayOneShot(clip, volume);
    }

    public void DebugDump()
    {
        if (mixer == null) return;
        float mv, sv;
        mixer.GetFloat(musicParam, out mv);
        mixer.GetFloat(sfxParam, out sv);
        Debug.Log($"[AudioManager] MusicVol={mv} dB, SFXVol={sv} dB | MusicMuted={MusicMuted}, SfxMuted={SfxMuted}");
    }

}