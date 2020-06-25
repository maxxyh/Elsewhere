using UnityEngine;
using System.Collections;
using System.Xml.Serialization;
using UnityEngine.PlayerLoop;
using System;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;
    public static AudioManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<AudioManager>();
                if (instance == null)
                {
                    instance = new GameObject("Spawned AudioManager", typeof(AudioManager)).GetComponent<AudioManager>();
                }
            }
            return instance;
        }
        private set
        {
            instance = value;
        }
    }


    #region Fields
    private AudioSource musicSource;
    private AudioSource musicSource2;
    private AudioSource sfxSource;
    private AudioSource loopingSFXSource;

    [SerializeField] private AudioClip hitSFX;
    [SerializeField] private AudioClip spellSFX;
    [SerializeField] private AudioClip crystalCollectedSFX;
    [SerializeField] private AudioClip walkingSFX;
    [SerializeField] private AudioClip buttonHoverSFX;
    [SerializeField] private AudioClip buttonClickSFX;


    private bool firstMusicSourceIsPlaying;
    private bool loopingSFXisPlaying;
    #endregion

    private void Awake()
    {
        // Make sure we don't destroy this instance
        DontDestroyOnLoad(this.gameObject);

        // Create audio sources, save as references
        musicSource = this.gameObject.AddComponent<AudioSource>();
        musicSource2 = this.gameObject.AddComponent<AudioSource>();
        sfxSource = this.gameObject.AddComponent<AudioSource>();
        loopingSFXSource = this.gameObject.AddComponent<AudioSource>();

        // Loop the music tracks
        musicSource.loop = true;
        musicSource2.loop = true;
        loopingSFXSource.loop = true;

        Unit.OnCrystalCollected += PlayCollectCrystalSound;
    }

    public void PlayMusic(AudioClip musicClip)
    {
        AudioSource activeSource = firstMusicSourceIsPlaying ? musicSource : musicSource2;
        activeSource.clip = musicClip;
        activeSource.volume = 1;
        activeSource.Play();

    }

    public void PlayMusicWithFade(AudioClip newClip, float transitionTime = 1.0f)
    {
        // Determine which source is active
        AudioSource activeSource = firstMusicSourceIsPlaying ? musicSource : musicSource2;

        StartCoroutine(UpdateMusicWithFade(activeSource, newClip, transitionTime));
    }

    public void PlayMusicWithCrossFade(AudioClip musicClip, float transitionTime = 1.0f)
    {
        // Determine which source is active
        AudioSource activeSource = firstMusicSourceIsPlaying ? musicSource : musicSource2;
        AudioSource newSource = firstMusicSourceIsPlaying ? musicSource2 : musicSource;

        // Swap the source
        firstMusicSourceIsPlaying = !firstMusicSourceIsPlaying;

        // Set the fields of the audio source, then start the Coroutine to crossfade
        newSource.clip = musicClip;
        newSource.Play();

        StartCoroutine(UpdateMusicWithCrossFade(activeSource, newSource, transitionTime));
    }

    private IEnumerator UpdateMusicWithCrossFade(AudioSource original, AudioSource newSource, float transitionTime)
    {
        float t = 0.0f;
        for (t = 0.0f; t <= transitionTime; t+= Time.deltaTime)
        {
            original.volume = 1 - (t / transitionTime);
            newSource.volume = t / transitionTime;
            yield return null;
        }
        original.Stop();
    }

    private IEnumerator UpdateMusicWithFade(AudioSource activeSource, AudioClip newClip, float transitionTime)
    {
        if (!activeSource.isPlaying)
        {
            activeSource.Play();
        }

        float t = 0.0f;

        // Fade Out
        for (t = 0.0f; t <= transitionTime; t += Time.deltaTime) 
        {
            activeSource.volume = (1 - (t / transitionTime)); 
            yield return null;
        }

        activeSource.Stop();
        activeSource.clip = newClip;
        activeSource.Play();

        // Fade In
        for (t = 0; t < transitionTime; t += Time.deltaTime)
        {
            activeSource.volume = (t / transitionTime);
            yield return null;
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    public void PlaySFX(AudioClip clip, float volume)
    {
        sfxSource.volume = volume;
        sfxSource.PlayOneShot(clip);
    }

    public void PlayLoopingSFX(AudioClip clip, float volume=1)
    {
        loopingSFXSource.clip = clip;
        loopingSFXSource.volume = volume;
        loopingSFXSource.Play();
        loopingSFXisPlaying = true;
}

    public void StopLoopingSFX()
    {
        loopingSFXSource.Stop();
        loopingSFXisPlaying = false;
}

    public void PlayHitSound(float volume=1)
    {
        PlaySFX(hitSFX, volume);
    }

    public void PlaySpellSound(float volume = 1)
    {
        PlaySFX(spellSFX, volume);
    }

    public void PlayWalkingSound(float volume = 1)
    {
        if (!loopingSFXisPlaying)
        {
            PlayLoopingSFX(walkingSFX, volume);
        }
    }

    public void PlayCollectCrystalSound()
    {
        PlaySFX(crystalCollectedSFX, 1);
    }

    public void PlayButtonHoverSFX()
    {
        PlaySFX(buttonHoverSFX);
    }
    public void PlayButtonClickSFX()
    {
        PlaySFX(buttonClickSFX);
    }


    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = volume;
    }

    public void SetMusicVolume(float volume)
    {
        musicSource.volume = volume;
        musicSource2.volume = volume;
    }
}
