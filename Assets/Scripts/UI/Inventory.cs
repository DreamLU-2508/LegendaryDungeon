using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DreamLU
{
    public class Inventory : MonoBehaviour
    {
        [SerializeField] private int quantity;
        [SerializeField] private InventoryBox inventoryBox;
        [SerializeField] private Transform _transform;

        private List<InventoryBox> _boxes = new List<InventoryBox>();

        public void ShowGrid(List<ItemData> itemDatas)
        {
            _boxes.Clear();
            HelperUtilities.ClearChildren(_transform, true);
            foreach (var itemData in itemDatas)
            {
                InventoryBox box = Instantiate(inventoryBox, _transform);
                box.Setup(itemData);
                _boxes.Add(box);
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
    }
}