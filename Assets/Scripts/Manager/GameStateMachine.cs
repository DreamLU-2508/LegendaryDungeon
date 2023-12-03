using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DreamLU
{
    public enum StateID
    {
        None,
        GameStart = 1,
        Normal,
        Pause,
        GameOver,
        Victory, // end dungeon
        StageVictory, // end game
    }

    public class WaitingState
    {
        public StateID queueState;
        public System.Action queueAction;
        public WaitingState(StateID queueState, System.Action queueAction)
        {
            this.queueState = queueState;
            this.queueAction = queueAction;
        }
    }

    [DefaultExecutionOrder(-102)]
    public class GameStateMachine : MonoBehaviour
    {
        static GameStateMachine _Instance = null;

        public static GameStateMachine Instance { get => _Instance; }

        private StateID _currentState;

        [ShowInInspector]
        public StateID CurrentState => _currentState;

        private Queue<WaitingState> _waitQueue = new();

        public System.Action<StateID> onStateEnter;
        public System.Action<StateID, StateID> onStateExit;

        public bool IsState(StateID stateID)
        {
            return _currentState == stateID;
        }

        private void Awake()
        {
            if (_Instance == null)
            {
                _Instance = this;
            }
        }

        void Start()
        {
            _currentState = StateID.GameStart;
            onStateEnter?.Invoke(_currentState);
        }

        public string CurrentStateName()
        {
            return _currentState.ToString();
        }

        public void ForceChangeState(StateID newState)
        {
            //_currentState.Exit();
            onStateExit?.Invoke(_currentState, newState);
            _currentState = newState;
            Debug.Log("FSM FORCE Switched to State -> " + _currentState);
            //_currentState.Enter();
            onStateEnter?.Invoke(_currentState);
        }

        [Button]
        public bool ChangeState(StateID newState, System.Action action = null)
        {
            if(newState == _currentState)
            {
                return false;
            }

            onStateExit?.Invoke(_currentState, newState);

            _currentState = newState;
            Debug.Log("FSM Switched to State -> " + _currentState);
            //_currentState.Enter();
            onStateEnter?.Invoke(_currentState);
            if (_waitQueue.Count > 0) DoQueueState();
            action?.Invoke();
            return true;
        }

        private void DoQueueState()
        {
            var canDoQueue = false;
            WaitingState first = null;
            while (_waitQueue.Count > 0)
            {
                first = _waitQueue.Dequeue();
                canDoQueue = true;
                break;
            }


            if (!canDoQueue) return;

            //_currentState.Exit();
            onStateExit?.Invoke(_currentState, first.queueState);

            _currentState = first.queueState;
            first.queueAction?.Invoke();

            Debug.Log("FSM Switched to State -> " + _currentState);
            //_currentState.Enter();
            onStateEnter?.Invoke(_currentState);
        }
    }
}
