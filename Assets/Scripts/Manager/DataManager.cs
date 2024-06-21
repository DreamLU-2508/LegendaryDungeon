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
        [SerializeField] private int maxAggregateData = 5;
        
        static DataManager _Instance = null;
        public static DataManager Instance => _Instance;
        
        private const string saveGameFileName = "SaveGame.es3";
        private const string saveKeyGameData = "gameData";
        private const string saveKeyItemInventory = "itemsInventory";
        private const string saveKeyAggregateData = "aggregateData";
        
        private GlobalGameData _globalGameData;
        private List<GlobalItemDataInventory> _itemsInventory = new List<GlobalItemDataInventory>();
        private List<AggregateData> _aggregateDatas = new List<AggregateData>();

        public GlobalGameData GlobalGameData => _globalGameData;
        public List<GlobalItemDataInventory> ItemsInventory => _itemsInventory;

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
                _itemsInventory = ES3.Load<List<GlobalItemDataInventory>>(saveKeyItemInventory, new List<GlobalItemDataInventory>());
                _aggregateDatas = ES3.Load<List<AggregateData>>(saveKeyAggregateData, new List<AggregateData>());
            }
            catch (Exception e)
            {
                _globalGameData = new GlobalGameData();
                _itemsInventory = new List<GlobalItemDataInventory>();
                _aggregateDatas = new List<AggregateData>();
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

        public bool TryGetGlobalItemDataInventory(ItemID itemID, out GlobalItemDataInventory itemDataInventory)
        {
            itemDataInventory = null;
            if (_itemsInventory != null && _itemsInventory.Count > 0)
            {
                var index = _itemsInventory.FindIndex(x => x.itemID == itemID);
                if (index != -1)
                {
                    itemDataInventory = _itemsInventory[index];
                    return true;
                }
            }

            return false;
        }
        
        public bool TryGetGlobalItemDataInventory(ItemID itemID, out int itemDataInventoryIndex)
        {
            itemDataInventoryIndex = -1;
            if (_itemsInventory != null && _itemsInventory.Count > 0)
            {
                var index = _itemsInventory.FindIndex(x => x.itemID == itemID);
                if (index != -1)
                {
                    itemDataInventoryIndex = index;
                    return true;
                }
            }

            return false;
        }

        public void AddItemInventory(GlobalItemDataInventory itemDataInventory)
        {
            if(itemDataInventory == null) return;
            
            if(itemDataInventory.itemID == ItemID.None) return;
            
            TryGetGlobalItemDataInventory(itemDataInventory.itemID, out int index);
            if (index != -1)
            {
                _itemsInventory[index].quantity += 1;
            }
            else
            {
                _itemsInventory.Add(new GlobalItemDataInventory()
                {
                    itemID = itemDataInventory.itemID,
                    quantity = 1,
                });
            }
            ES3.Save(saveKeyItemInventory, _itemsInventory);
        }
        
        public void RemoveItemInventory(ItemID itemID, int quantity)
        {
            if(_itemsInventory.Count <= 0) return;
            
            TryGetGlobalItemDataInventory(itemID, out int index);
            if (index != -1)
            {
                _itemsInventory[index].quantity -= quantity;
                if (_itemsInventory[index].quantity <= 0)
                {
                    _itemsInventory.RemoveAt(index);
                }
                ES3.Save(saveKeyItemInventory, _itemsInventory);
            }
        }

        public void SaveAggregateData(AggregateData aggregateData)
        {
            if (_aggregateDatas.Count < maxAggregateData)
            {
                _aggregateDatas.Add(aggregateData);
            }
            else
            {
                _aggregateDatas.RemoveAt(0);
                _aggregateDatas.Add(aggregateData);
            }
            
            ES3.Save(saveKeyAggregateData, _aggregateDatas);
        }
    }
}
