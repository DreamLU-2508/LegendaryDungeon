using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;
using Random = System.Random;

namespace DreamLU
{
    public class CardManager : MonoBehaviour, ICardProvider
    {
        [SerializeField] private CardsDataManifest _cardsDataManifest;

        [ShowInInspector, ReadOnly]
        private List<CardData> _cardDatas = new();

        private IGameStateProvider _gameStateProvider;

        public bool CanSelectCard
        {
            get
            {
                return _cardDatas.Count > 0;
            }
        }

        private void Awake()
        {
            _gameStateProvider = CoreLifetimeScope.SharedContainer.Resolve<IGameStateProvider>();

            _gameStateProvider.OnStartGame += OnStartGame;
            _gameStateProvider.OnEndGame += OnEndGame;
        }

        private void OnStartGame()
        {
            _cardDatas = _cardsDataManifest.CloneList();
        }
        
        private void OnEndGame()
        {
            _cardDatas.Clear();
        }
        
        public List<CardData> RandomCard()
        {
            if (_cardDatas.Count <= 0)
            {
                return null;
            }
            
            if (_cardDatas.Count < Settings.maxCardSelect) return _cardDatas;

            Random rand = new Random();
            List<CardData> cards = _cardDatas.OrderBy(x => rand.Next()).Take(Settings.maxCardSelect).ToList();

            return cards;
        }

        public void RemoveCard(CardData cardData)
        {
            if (_cardDatas.Contains(cardData))
            {
                _cardDatas.Remove(cardData);
            }
        }

        private void OnDestroy()
        {
            _gameStateProvider.OnStartGame -= OnStartGame;
            _gameStateProvider.OnEndGame -= OnEndGame;
        }
    }

}