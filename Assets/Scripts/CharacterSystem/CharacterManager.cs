using DG.Tweening.Core.Easing;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DreamLU
{
    [System.Serializable]
    public class CharacterActionDefinition
    {
        public CharacterActionID actionID;
        public CharacterAction action;
    }

    public class CharacterManager : MonoBehaviour, ICharacterActor
    {
        [SerializeField][TableList] private List<CharacterActionDefinition> characterActions;

        private LDGameManager gameManager;

        private Transform characterTransform;
        private bool _isMovementLocked;
        private bool _isActionLocked = false;
        private bool _characterInitialized = false;

        public Transform CharacterTransform => characterTransform;

        public Animator CharacterAnimator
        {
            get
            {
                return characterTransform.GetComponent<Animator>();
            }
        }

        public bool IsMovementLocked
        {
            get => _isMovementLocked;
            set => _isMovementLocked = value;
        }

        private void Awake()
        {
            gameManager = FindObjectOfType<LDGameManager>();
            gameManager.OnInitializeCharacter += OnInitializeCharacter;
            Debug.Log("CharacterManager");
        }

        private void Update()
        {
            if (!_characterInitialized) { return; }

            if (LDGameManager.Instance.CurrentMasterGameState == StateID.None) return;

            if (GameStateMachine.Instance.IsState(StateID.Normal))
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    ActivateAction(CharacterActionID.Dash);
                }

                //if (_heroInvulnerableTimer > 0)
                //{
                //    _heroInvulnerableTimer -= Time.deltaTime;
                //}
            }
        }

        private void OnInitializeCharacter()
        {
            characterTransform = gameManager.HeroTransform;

            SetupActions();
            _characterInitialized = true;
        }

        void SetupActions()
        {
            foreach (var actionDef in this.characterActions)
            {
                actionDef.action.ActionID = actionDef.actionID;
                CoreLifetimeScope.SharedContainer.Inject(actionDef.action);
                actionDef.action.Setup();
            }
        }

        void ActivateAction(CharacterActionID actionID)
        {
            if (_isActionLocked) return;

            var actionDef = characterActions.Find((x) => x.actionID == actionID);
            if (actionDef != null)
            {
                if (actionDef.action.IsActivable() && actionDef.action.IsCompleted() && !actionDef.action.IsInCooldown())
                {
                    actionDef.action.Activate();
                }
            }
        }
    }

}