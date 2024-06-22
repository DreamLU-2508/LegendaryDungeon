using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using Button = UnityEngine.UI.Button;

namespace DreamLU
{
    public class Inventory : MonoBehaviour
    {
        [SerializeField] private int quantity;
        [SerializeField] private InventoryBox inventoryBox;
        [SerializeField] private Transform _transform;
        [SerializeField] private Button _button;
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private ItemDataManifest _itemDataManifest;

        [ShowInInspector]
        private ItemData _itemDataSelected = null;
        [ShowInInspector]
        private InventoryBox _inventoryBox = null;
        private List<InventoryBox> _boxes = new List<InventoryBox>();

        public ItemData ItemData => _itemDataSelected;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) && _itemDataSelected)
            {
                _itemDataSelected = null;
                _button.gameObject.SetActive(false);
                if (_inventoryBox)
                {
                    _inventoryBox.IsLock = false;
                    _inventoryBox.Hover.gameObject.SetActive(false);
                }
            }
        }

        public void ShowGrid(List<ItemData> itemDatas)
        {
            _boxes.Clear();
            HelperUtilities.ClearChildren(_transform, true);
            foreach (var itemData in itemDatas)
            {
                InventoryBox box = Instantiate(inventoryBox, _transform);
                box.Setup(itemData);
                _boxes.Add(box);
                box.OnClick += OnSelect;
            }

            int count = quantity - _boxes.Count;
            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    InventoryBox box = Instantiate(inventoryBox, _transform);
                    box.Setup(null);
                    _boxes.Add(box);
                }
            }
            this.gameObject.SetActive(true);
        }

        public void HideGird()
        {
            _boxes.Clear();
            this.gameObject.SetActive(false);
        }

        public void OnSelect(ItemData itemData, InventoryBox inventoryBox)
        {
            if (_itemDataSelected == null || (_itemDataSelected != null && _itemDataSelected.itemID != itemData.itemID))
            {
                _itemDataSelected = itemData;
                _button.gameObject.SetActive(false);
                _inventoryBox = inventoryBox;
                _inventoryBox.IsLock = true;
                if (itemData.itemType == ItemType.Blueprint)
                {
                    if (LDGameManager.Instance.stateMachine.IsState(StateID.GameStart))
                    {
                        _button.gameObject.SetActive(true);
                        _button.interactable = true;
                        if (itemData.price > DataManager.Instance.GlobalGameData.gold)
                        {
                            _button.interactable = false;
                        }
                        text.text = $"Use - {itemData.price}g";
                    }
                }
            }
        }

        public void OnButtonClick()
        {
            if (_itemDataSelected != null)
            {
                if (_itemDataSelected.itemType == ItemType.Blueprint)
                {
                    DataManager.Instance.HideItemInventory(_itemDataSelected.itemID);
                    DataManager.Instance.SubMaterial(_itemDataSelected.price);
                    DataManager.Instance.UnlockWeapon(_itemDataSelected.weaponLink);
                    
                    List<ItemData> itemDatas = new List<ItemData>();
                    foreach (var itemDataInventory in DataManager.Instance.ItemsInventory)
                    {
                        var data = _itemDataManifest.GetItem(itemDataInventory.itemID);
                        if (data != null && itemDataInventory.isShow)
                        {
                            itemDatas.Add(data);
                        }
                    }
                    ShowGrid(itemDatas);
                    _button.gameObject.SetActive(false);
                    _itemDataSelected = null;
                    _inventoryBox = null;
                }
            }
        }
    }
}