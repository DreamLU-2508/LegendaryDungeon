using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DreamLU
{
    public class InventoryBox : MonoBehaviour
    {
        [SerializeField] private Image image;

        private ItemData _itemData = null;

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
    }
}
