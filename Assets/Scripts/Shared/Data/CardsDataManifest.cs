using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = System.Random;

namespace DreamLU
{
    [CreateAssetMenu(menuName = "Database/Card System/CardsDataManifest")]
    public class CardsDataManifest : ScriptableObject
    {
        public List<CardData> _cardDatas;

        public List<CardData> CloneList()
        {
            List<CardData> list = new List<CardData>();
            foreach (var cardData in _cardDatas)
            {
                list.Add(cardData);
            }

            return list;
        }
    }

}