using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace DreamLU.UI
{
    public class UIGameOver : MonoBehaviour
    {
        private GameStateMachine gameStateMachine;
        
        private IGameStateProvider _gameStateProvider;
        
        private void Awake()
        {
            _gameStateProvider = CoreLifetimeScope.SharedContainer.Resolve<IGameStateProvider>();
            
            gameStateMachine = GameStateMachine.Instance;
            gameStateMachine.onStateEnter += OnStateEnter;
        }

        private void OnStateEnter(StateID stateID)
        {
            if (stateID == StateID.GameOver)
            {
                this.gameObject.SetActive(true);
            }
            else
            {
                this.gameObject.SetActive(false);
            }
        }

        public void ActionMainMenu()
        {
            _gameStateProvider.GotoMainMenu();
        }
    }

}