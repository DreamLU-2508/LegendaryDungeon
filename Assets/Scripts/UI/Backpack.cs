using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DreamLU
{
    public class Backpack : MonoBehaviour
    {
        [SerializeField] private Transform root;
        [SerializeField] private Inventory _inventory;
        [SerializeField] private ItemDataManifest _itemDataManifest;

        private void Awake()
        {
            _inventory.HideGird();
            root.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) && _inventory.ItemData == null)
            {
                if (root.gameObject.activeSelf)
                {
                    _inventory.HideGird();
                    root.gameObject.SetActive(false);
                    LDGameManager.Instance.IsShowInventory = false;
                }
            }
        }

        public void Show()
        {
            LDGameManager.Instance.IsShowInventory = true;
            root.gameObject.SetActive(true);
            List<ItemData> itemDatas = new List<ItemData>();
            foreach (var itemDataInventory in DataManager.Instance.ItemsInventory)
            {
                var data = _itemDataManifest.GetItem(itemDataInventory.itemID);
                if (data != null && itemDataInventory.isShow)
                {
                    itemDatas.Add(data);
                }
            }
            _inventory.ShowGrid(itemDatas);
        }
    }
}