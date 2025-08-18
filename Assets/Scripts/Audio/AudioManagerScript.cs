using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;
public class AudioManagerScript : MonoBehaviour
{
    public static AudioManagerScript Instance { get; private set; }
    public AudioSource bgmSource;
    public AudioSource objectPoolSFX;
    public AudioSource clikeUiSFX;
    public AudioClip[] _audioBgm;
    public AudioClip[] _audioPlayerSfx;
    public AudioClip[] _audioEnemySfx;
    public AudioClip _audioUISfx;
    public AudioMixer audioMixer;
    public GameObject obj;
    private Queue<AudioSource> audioPool = new Queue<AudioSource>();
    private int poolSize = 5;
    private bool isBulletPlaying = false;

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

    void Start()
    {
        for (int i = 0; i < poolSize; i++)
        {
            obj.transform.SetParent(this.transform);
            AudioSource source = obj.AddComponent<AudioSource>();
            source.spatialBlend = 0f;
            source.playOnAwake = false;
            audioPool.Enqueue(source);
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
            case 4:
                PlayBGMController(_audioBgm[4]);
                break;
            case 5:
                PlayBGMController(_audioBgm[5]);
                break;
            case 6:
                PlayBGMController(_audioBgm[6]);
                break;
        }
    }

    public void EnemySFX(int type)
    {
        switch (type)
        {
            case 0:
                PlaySFX(_audioEnemySfx[0]);
                break;
            case 1:
                PlaySFX(_audioEnemySfx[1]);
                break;
        }
    }

    public void PlayerSFX(int type)
    {
        switch (type)
        {
            case 0:
                PlaySFX(_audioPlayerSfx[0]);
                break;
            case 1:
                PlaySFX(_audioPlayerSfx[1]);
                break;
            case 2:
                PlayBulletSFX(_audioPlayerSfx[2]);
                break;
        }
    }

    public void PlayUISource()
    {
        clikeUiSFX.PlayOneShot(_audioUISfx);
    }

    private void PlayBGMController(AudioClip newClip)
    {
        if (bgmSource.clip == newClip && bgmSource.isPlaying)
            return;

        bgmSource.clip = newClip;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        if (audioPool.Count > 0)
        {
            AudioSource source = audioPool.Dequeue();
            source.clip = clip;
            source.Play();

            StartCoroutine(ReturnToPool(source, clip.length));
        }
    }
    public void PlayBulletSFX(AudioClip clip)
    {
        if (isBulletPlaying) return;

        if (audioPool.Count > 0)
        {
            AudioSource source = audioPool.Dequeue();
            source.clip = clip;
            source.Play();

            isBulletPlaying = true;

            StartCoroutine(ReturnToBulletPool(source, clip.length));
        }
    }

    private IEnumerator ReturnToPool(AudioSource source, float delay)
    {
        yield return new WaitForSeconds(delay);
        source.Stop();
        audioPool.Enqueue(source);
    }

    private IEnumerator ReturnToBulletPool(AudioSource source, float delay)
    {
        yield return new WaitForSeconds(delay);
        source.Stop();
        audioPool.Enqueue(source);

        isBulletPlaying = false;
    }
}
