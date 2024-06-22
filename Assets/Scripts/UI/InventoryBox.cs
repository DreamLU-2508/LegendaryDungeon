using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DreamLU
{
    public class InventoryBox : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
    {
        [SerializeField] private Image image;
        [SerializeField] private Image hover;

        private ItemData _itemData = null;
        private bool isLock; 
        
        public event Action<ItemData, InventoryBox> OnClick;

        public bool IsLock
        {
            get => isLock;
            set => isLock = value;
        }

        public Image Hover => hover;

        public void Setup(ItemData itemData)
        {
            if (itemData == null)
            {
                image.enabled = false;
            }
            else
            {
                image.sprite = itemData.itemSprite;
            }
            _itemData = itemData;
        }


        public void OnPointerEnter(PointerEventData eventData)
        {
            if(isLock) return;
            
            hover.gameObject.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if(isLock) return;
            
            hover.gameObject.SetActive(false);
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            OnClick?.Invoke(_itemData, this);
        }
    }
}
