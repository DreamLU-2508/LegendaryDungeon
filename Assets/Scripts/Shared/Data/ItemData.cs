using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DreamLU
{
    public class ItemData : Droppable
    {
        public ItemID itemID;
        public string itemName;
        public Sprite itemSprite;
        public int price;
        public ItemRarity tier;

        public GameObject itemDropPrefab;
    }
}
