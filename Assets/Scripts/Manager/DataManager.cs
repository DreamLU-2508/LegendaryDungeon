using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace DreamLU
{
    [DefaultExecutionOrder(-103)]
    public class DataManager : MonoBehaviour
    {
        static DataManager _Instance = null;
        public static DataManager Instance => _Instance;
        
        private const string saveGameFileName = "SaveGame.es3";
        private const string saveKeyGameData = "gameData";
        
        private GlobalGameData _globalGameData;
        
        public GlobalGameData GlobalGameData => _globalGameData;
        
        public event System.Action OnChangeGlobalData;

        private void Awake()
        {
            Debug.Log("[DataManager] Awake");
            if (_Instance == null)
            {
                _Instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                Destroy(this.gameObject);
                return;
            }
            
            LoadData(true);
        }

        private void OnDestroy()
        {
            
        }
        
        void LoadData(bool migrate)
        {
            // Sample Loading Code
            try
            {
                _globalGameData = ES3.Load<GlobalGameData>(saveKeyGameData, new GlobalGameData());
            }
            catch (Exception e)
            {
                _globalGameData = new GlobalGameData();
            }
            // LoadDifficultyCharacterData();
        }
        
        [Button]
        public void AddGlobalGameData(int gold)
        {
            _globalGameData.gold += gold;
            ES3.Save(saveKeyGameData, _globalGameData);
            
            OnChangeGlobalData?.Invoke();
            Debug.Log("Global Game Data => " + JsonUtility.ToJson(_globalGameData));
        }
        
        [Button]
        public void SubMaterial(int gold)
        {
            _globalGameData.gold -= gold;
            if(_globalGameData.gold < 0) return;
            ES3.Save(saveKeyGameData, _globalGameData);
            
            OnChangeGlobalData?.Invoke();
            Debug.Log("Global Game Data => " + JsonUtility.ToJson(_globalGameData));
        }
    }
}
