using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DreamLU
{
    [CreateAssetMenu(fileName = "ItemsDataManifest", menuName = "Database/ItemsDataManifest")]
    public class ItemDataManifest : ScriptableObject
    {
        public List<ItemData> itemDatas;

        public ItemData GetItem(ItemID itemID)
        {
            return itemDatas.Find(x => x.itemID == itemID);
        }
    }

}