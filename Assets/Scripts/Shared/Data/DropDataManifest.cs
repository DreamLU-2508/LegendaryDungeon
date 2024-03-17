using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = System.Random;

namespace DreamLU
{
    public class Droppable: ScriptableObject
    {
        
    }
    
    [CreateAssetMenu(fileName = "DropItemsDataManifest", menuName = "Database/DropItemsDataManifest")]
    public class DropDataManifest : ScriptableObject
    {
        [Header("Chest Drop")] 
        [SerializeField] private List<Droppable> chestItemsDroppable;

        [Button]
        public Droppable RandomChestItemsDroppable(List<Droppable> excludedList)
        {
            Random random = new Random();
            var availableList = chestItemsDroppable;
            if (excludedList != null && excludedList.Count > 0)
            {
                availableList = chestItemsDroppable.Where(num => !excludedList.Contains(num)).ToList();
            }
                
            
            if (availableList.Count == 0)
                return null;
            
            int randomIndex = random.Next(availableList.Count);
            return availableList[randomIndex];
        }
    }
}