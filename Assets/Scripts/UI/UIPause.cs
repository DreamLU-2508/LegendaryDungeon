using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace DreamLU.UI
{
    public class UIPause : MonoBehaviour
    {
        private GameStateMachine gameStateMachine;
        
        private IGameStateProvider _gameStateProvider;
        private IPauseGame _pauseGame;
        
        private void Awake()
        {
            _gameStateProvider = CoreLifetimeScope.SharedContainer.Resolve<IGameStateProvider>();
            _pauseGame = CoreLifetimeScope.SharedContainer.Resolve<IPauseGame>();
            
            gameStateMachine = GameStateMachine.Instance;
            gameStateMachine.onStateEnter += OnStateEnter;
        }

        private void OnStateEnter(StateID stateID)
        {
            if (stateID == StateID.Pause)
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
        
        public void ActionResume()
        {
            _pauseGame.UnpauseGame();
        }
    }

}