using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace DreamLU
{
    public class UIGlobalGameData : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI gold;

        private DataManager _dataManager;
        private GameStateMachine _gameStateMachine;
        private IGameStateProvider _gameStateProvider;
        
        private void Awake()
        {
            _dataManager = DataManager.Instance;
            _gameStateMachine = GameStateMachine.Instance;
            
            _dataManager.OnChangeGlobalData += UpdateGold;
            _gameStateMachine.onStateEnter += OnStateEnter;
        }

        private void Start()
        {
            Show();
        }

        void OnStateEnter(StateID stateID)
        {
            if(stateID == StateID.GameStart)
            {
                Show();
            }
            else
            {
                Hide();
            }
        }
        
        private void UpdateGold()
        {
            gold.text = _dataManager.GlobalGameData.gold.ToString();
        }

        void Show()
        {
            this.gameObject.SetActive(true);
        }

        void Hide()
        {
            this.gameObject.SetActive(false);
        }
    }
}
