using UnityEngine;
using UnityEngine.Audio;
public class AudioManagerScript : MonoBehaviour
{
    public static AudioManagerScript Instance { get; private set; }
    public AudioSource bgmSource;
    public AudioSource sfxSource;
    public AudioClip[] _audioBgm;
    public AudioClip[] _audioSfx;
    public AudioMixer audioMixer;
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
        }
    }

    public void SetMasterVolume(float volume)
    {
        float dB = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20f;
        audioMixer.SetFloat("MainVolume", dB);
    }

    public void SetBGMVolume(float volume)
    {
        float dB = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20f;
        audioMixer.SetFloat("BGMVolume", dB);
    }

    public void SetSFXVolume(float volume)
    {
        float dB = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20f;
        audioMixer.SetFloat("SFXVolume", dB);
    }

    public void PlayBgm(int type)
    {
        switch (type)
        {
            case 0:
                PlayBGMController(_audioBgm[0]);
                break;
            case 1:
                PlayBGMController(_audioBgm[1]);
                break;
            case 2:
                PlayBGMController(_audioBgm[2]);
                break;
            case 3:
                PlayBGMController(_audioBgm[3]);
                break;
        }
    }

    private void PlayBGMController(AudioClip newClip)
    {
        if (bgmSource.clip == newClip && bgmSource.isPlaying)
            return;

        bgmSource.clip = newClip;
        bgmSource.loop = true;
        bgmSource.Play();
    }
}
