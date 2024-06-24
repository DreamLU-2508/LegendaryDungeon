using System.Collections;
using DreamLU;
using UnityEngine;
using UnityEngine.Audio;

[DisallowMultipleComponent]
public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [SerializeField] private AudioMixerGroup musicMasterMixerGroup;
    [SerializeField] private AudioMixerSnapshot musicOffSnapshot;
    [SerializeField] private AudioMixerSnapshot musicLowSnapshot;
    [SerializeField] private AudioMixerSnapshot musicOnFullSnapshot;
    
    private AudioSource musicAudioSource = null;
    private AudioClip currentAudioClip = null;
    private Coroutine fadeOutMusicCoroutine;
    private Coroutine fadeInMusicCoroutine;
    private int musicVolume = 10;

    public int MusicVolume
    {
        get => musicVolume;
        set
        {
            musicVolume = value;
            SetMusicVolume(value);
            PlayerPrefs.SetInt("musicVolume", musicVolume);
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        // Load components
        musicAudioSource = GetComponent<AudioSource>();

        // Start with music off
        musicOffSnapshot.TransitionTo(0f);

    }

    private void Start()
    {
        // Check if volume levels have been saved in playerprefs - if so retrieve and set them
        if (PlayerPrefs.HasKey("musicVolume"))
        {
            musicVolume = PlayerPrefs.GetInt("musicVolume");
        }

        SetMusicVolume(musicVolume);
    }

    private void OnDisable()
    {
        // Save volume settings in playerprefs
        PlayerPrefs.SetInt("musicVolume", musicVolume);
    }



    public void PlayMusic(MusicTrackSO musicTrack, float fadeOutTime = 0.5f, float fadeInTime = 0.5f)
    {
        // Play music track
        StartCoroutine(PlayMusicRoutine(musicTrack, fadeOutTime, fadeInTime));
    }

    /// <summary>
    /// Play music for room routine
    /// </summary>
    private IEnumerator PlayMusicRoutine(MusicTrackSO musicTrack, float fadeOutTime, float fadeInTime)
    {
        // if fade out routine already running then stop it
        if (fadeOutMusicCoroutine != null)
        {
            StopCoroutine(fadeOutMusicCoroutine);
        }

        // if fade in routine already running then stop it
        if (fadeInMusicCoroutine != null)
        {
            StopCoroutine(fadeInMusicCoroutine);
        }

        // If the music track has changed then play new music track
        if (musicTrack.musicClip != currentAudioClip)
        {
            currentAudioClip = musicTrack.musicClip;

            yield return fadeOutMusicCoroutine = StartCoroutine(FadeOutMusic(fadeOutTime));

            yield return fadeInMusicCoroutine = StartCoroutine(FadeInMusic(musicTrack, fadeInTime));
        }

        yield return null;
    }

    /// <summary>
    /// Fade out music routine
    /// </summary>
    private IEnumerator FadeOutMusic(float fadeOutTime)
    {
        musicLowSnapshot.TransitionTo(fadeOutTime);

        yield return new WaitForSeconds(fadeOutTime);
    }

    /// <summary>
    /// Fade in music routine
    /// </summary>
    private IEnumerator FadeInMusic(MusicTrackSO musicTrack, float fadeInTime)
    {
        // Set clip & play
        musicAudioSource.clip = musicTrack.musicClip;
        musicAudioSource.volume = musicTrack.musicVolume;
        musicAudioSource.Play();

        musicOnFullSnapshot.TransitionTo(fadeInTime);

        yield return new WaitForSeconds(fadeInTime);
    }

    /// <summary>
    /// Increase music volume
    /// </summary>
    public void IncreaseMusicVolume()
    {
        int maxMusicVolume = 20;

        if (musicVolume >= maxMusicVolume) return;

        musicVolume += 1;

        SetMusicVolume(musicVolume);
    }

    /// <summary>
    /// Decrease music volume
    /// </summary>
    public void DecreaseMusicVolume()
    {
        if (musicVolume == 0) return;

        musicVolume -= 1;

        SetMusicVolume(musicVolume);
    }

    /// <summary>
    /// Set music volume
    /// </summary>
    public void SetMusicVolume(int musicVolume)
    {
        float muteDecibels = -80f;

        if (musicVolume == 0)
        {
            musicMasterMixerGroup.audioMixer.SetFloat("musicVolume", muteDecibels);
        }
        else
        {
            musicMasterMixerGroup.audioMixer.SetFloat("musicVolume", HelperUtilities.LinearToDecibels(musicVolume));
        }
    }
}