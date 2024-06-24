using System;
using System.Collections;
using DreamLU;
using UnityEngine;
using UnityEngine.Audio;

[DisallowMultipleComponent]
public class SoundEffectManager : MonoBehaviour
{
    public static SoundEffectManager Instance;
    [Header("Sounds")] [SerializeField] private AudioMixerGroup soundsMasterMixerGroup;

    private int soundsVolume = 10;

    public int SoundsVolume
    {
        get => soundsVolume;
        set
        {
            soundsVolume = value;
            SetSoundsVolume(value);
            PlayerPrefs.SetInt("musicVolume", soundsVolume);
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        if (PlayerPrefs.HasKey("soundsVolume"))
        {
            soundsVolume = PlayerPrefs.GetInt("soundsVolume");
        }
       
        SetSoundsVolume(soundsVolume);
        PoolManager.Instance.OnDestroyAllPools += () =>
        {
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
                _coroutine = null;
            }
        };
    }

    private void OnDisable()
    {
        // Save volume settings in playerprefs
        PlayerPrefs.SetInt("soundsVolume", soundsVolume);
    }

    private Coroutine _coroutine;

    /// <summary>
    /// Play the sound effect
    /// </summary>
    public void PlaySoundEffect(SoundEffectSO soundEffect)
    {
        // Play sound using a sound gameobject and component from the object pool
        SoundEffect sound = PoolManager.GetPool(soundEffect.soundPrefab).RetrieveObject( Vector3.zero, Quaternion.identity, PoolManager.Instance.CurrentTransform).GetComponent<SoundEffect>();
        sound.SetSound(soundEffect);
        sound.gameObject.SetActive(true);
        _coroutine = StartCoroutine(DisableSound(sound, soundEffect.soundEffectClip.length));

    }

    

    /// <summary>
    /// Disable sound effect object after it has played thus returning it to the object pool
    /// </summary>
    private IEnumerator DisableSound(SoundEffect sound, float soundDuration)
    {
        yield return new WaitForSeconds(soundDuration);
        sound.gameObject.SetActive(false);
    }

    //
    /// <summary>
    /// Increase sounds volume
    /// </summary>
    public void IncreaseSoundsVolume()
    {
        int maxSoundsVolume = 20;

        if (soundsVolume >= maxSoundsVolume) return;

        soundsVolume += 1;

        SetSoundsVolume(soundsVolume); ;
    }

    /// <summary>
    /// Decrease sounds volume
    /// </summary>
    public void DecreaseSoundsVolume()
    {
        if (soundsVolume == 0) return;

        soundsVolume -= 1;

        SetSoundsVolume(soundsVolume);
    }

    /// <summary>
    /// Set sounds volume
    /// </summary>
    private void SetSoundsVolume(int soundsVolume)
    {
        float muteDecibels = -80f;

        if (soundsVolume == 0)
        {
            soundsMasterMixerGroup.audioMixer.SetFloat("soundsVolume", muteDecibels);
        }
        else
        {
            soundsMasterMixerGroup.audioMixer.SetFloat("soundsVolume", HelperUtilities.LinearToDecibels(soundsVolume));
        }
    }
}