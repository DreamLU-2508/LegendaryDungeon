using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DreamLU
{
    public interface ICardProvider
    {
        public bool CanSelectCard { get; }

        public List<CardData> RandomCard();

        public void RemoveCard(CardData cardData);
    }
}
