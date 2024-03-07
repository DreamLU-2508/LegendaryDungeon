using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VContainer;

namespace DreamLU.UI
{
    public class CardItem : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI textName;
        [SerializeField] private TextMeshProUGUI textDes;
        [SerializeField] private Sprite defaultIcon;

        private CardData _cardData;

        private ICharacterActor _characterActor;
        private ICardProvider _cardProvider;
        private IGameStateProvider _gameStateProvider;

        private void Awake()
        {
            _characterActor = CoreLifetimeScope.SharedContainer.Resolve<ICharacterActor>();
            _cardProvider = CoreLifetimeScope.SharedContainer.Resolve<ICardProvider>();
            _gameStateProvider = CoreLifetimeScope.SharedContainer.Resolve<IGameStateProvider>();
        }

        public void Setup(CardData cardData)
        {
            if (cardData != null)
            {
                this._cardData = cardData;

                Sprite sprite = cardData.icon == null ? defaultIcon : cardData.icon;
                icon.sprite = sprite;

                textName.text = cardData.nameCard;
                textDes.text = cardData.descriptionCard;
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _characterActor.AddCard(_cardData);
            _cardProvider.RemoveCard(_cardData);
            _gameStateProvider.ProceedToEndLevel();
        }
    }

}