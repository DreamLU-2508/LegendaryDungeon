using System;
using System.Collections;
using System.Collections.Generic;
using DreamLU.UI;
using UnityEngine;
using VContainer;

namespace DreamLU
{
    public class UICardSelect : MonoBehaviour
    {
        [SerializeField] private Transform root;
        [SerializeField] private Transform container;
        [SerializeField] private CardItem cardItemPrefab;

        private ICardProvider _cardProvider;
        private IGameStateProvider _gameStateProvider;
        private ICharacterActor _characterActor;
        private GameStateMachine _gameStateMachine;
        
        private void Awake()
        {
            _cardProvider = CoreLifetimeScope.SharedContainer.Resolve<ICardProvider>();
            _gameStateProvider = CoreLifetimeScope.SharedContainer.Resolve<IGameStateProvider>();
            _characterActor = CoreLifetimeScope.SharedContainer.Resolve<ICharacterActor>();
            _gameStateMachine = GameStateMachine.Instance;

            root.gameObject.SetActive(false);
            _gameStateMachine.onStateEnter += OnStatChange;
        }

        private void OnStatChange(StateID stateID)
        {
            if (stateID == StateID.SelectCard)
            {
                root.gameObject.SetActive(true);
                _characterActor.IsMovementLocked = true;
                _characterActor.IsActionLocked = true;
                Setup();
            }
            else
            {
                root.gameObject.SetActive(false);
                _characterActor.IsMovementLocked = false;
                _characterActor.IsActionLocked = false;
            }
        }

        private void Setup()
        {
            HelperUtilities.ClearChildren(container, false);
            List<CardData> _cardDatas = _cardProvider.RandomCard();
            if (_cardDatas != null && _cardDatas.Count > 0)
            {
                foreach (var cardData in _cardDatas)
                {
                    var cardItem = Instantiate(cardItemPrefab, container);
                    cardItem.Setup(cardData);
                }
            }
            else
            {
                _gameStateProvider.ProceedToEndLevel();
            }
        }

        private void OnDestroy()
        {
            _gameStateMachine.onStateEnter -= OnStatChange;
        }
    }

}