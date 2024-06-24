using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace DreamLU
{
    public class UISettings : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI musicLevelText;
        [SerializeField] private TextMeshProUGUI soundsLevelText;
        
        private void Awake()
        {
            HideSelf();
        }

        public void HideSelf()
        {
            // eventHideUISettings?.Invoke();
            gameObject.SetActive(false);
        }

        public void ShowSelf()
        {
            gameObject.SetActive(true);
            musicLevelText.SetText(MusicManager.Instance.MusicVolume.ToString());
            soundsLevelText.SetText(SoundEffectManager.Instance.SoundsVolume.ToString());
        }
        
        public bool IsShown()
        {
            return gameObject.activeSelf;
        }

        public void ActionBack()
        {
            LDGameManager.Instance.HideUISettings();
        }
        
        public void IncreaseMusicVolume()
        {
            MusicManager.Instance.IncreaseMusicVolume();
            musicLevelText.SetText(MusicManager.Instance.MusicVolume.ToString());
        }
        
        public void DecreaseMusicVolume()
        {
            MusicManager.Instance.DecreaseMusicVolume();
            musicLevelText.SetText(MusicManager.Instance.MusicVolume.ToString());
        }
        
        public void IncreaseSoundsVolume()
        {
            SoundEffectManager.Instance.IncreaseSoundsVolume();
            soundsLevelText.SetText(SoundEffectManager.Instance.SoundsVolume.ToString());
        }
        
        public void DecreaseSoundsVolume()
        {
            SoundEffectManager.Instance.DecreaseSoundsVolume();
            soundsLevelText.SetText(SoundEffectManager.Instance.SoundsVolume.ToString());
        }
    }

}