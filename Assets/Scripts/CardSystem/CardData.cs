using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DreamLU
{
    public abstract class CardData : ScriptableObject
    {
        public CardType cardType;
        public string nameCard;
        public string descriptionCard;

        public PowerUp powerUp;

        [Header("Icon")] 
        public Sprite icon;

        public virtual float Resolve()
        {
            return powerUp.value;
        }
    }
}
