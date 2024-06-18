using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DreamLU
{
    public enum ItemType
    {
        None,
        Blueprint
    }
    
    [CreateAssetMenu(fileName = "ACC_", menuName = "Database/Item/Item Data")]
    public class ItemData : Droppable
    {
        public ItemID itemID;
        public string itemName;
        public Sprite itemSprite;
        public int price;
        public ItemRarity tier;

        public GameObject itemDropPrefab;
        public ItemType itemType;
        public int quantityLimit;
    }
}
