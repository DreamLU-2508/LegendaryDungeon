using DG.Tweening.Core.Easing;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Animation;
using UnityEngine;

namespace DreamLU
{
    [System.Serializable]
    public class CharacterActionDefinition
    {
        public CharacterActionID actionID;
        public CharacterAction action;
    }

    public class CharacterManager : MonoBehaviour, ICharacterActor, IWeaponProvider
    {
        [SerializeField][TableList] private List<CharacterActionDefinition> characterActions;

        private LDGameManager gameManager;

        private Transform characterTransform;
        private bool _isMovementLocked;
        private bool _isActionLocked = false;
        private bool _characterInitialized = false;
        private CharacterSkill _characterSkill;
        private int health;
        private int mana;
        private int maxMana;
        private int maxHealth;
        private CharacterData characterData;
        private Character _character;
        private float _heroInvulnerableTimer = 0;
        private bool _isHeroDead;
        private CharacterStat _characterStat;

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

        [ShowInInspector, ReadOnly]
        public int Health
        {
            get { return health; }
            set 
            { 
                health = value;
                OnUpdateHealth?.Invoke();
            }
        } 
        [ShowInInspector, ReadOnly]
        public int MaxHealth => maxHealth;

        [ShowInInspector, ReadOnly]
        public int Mana
        {
            get { return mana; }
            set
            {
                mana = value;
                OnUpdateMana?.Invoke();
            }
        }
        [ShowInInspector, ReadOnly]
        public int MaxMana => maxMana;

        // Action
        public event System.Action OnInitCharacter;
        public event System.Action OnUpdateHealth;
        public event System.Action OnUpdateMana;

        private void Awake()
        {
            gameManager = FindObjectOfType<LDGameManager>();
            gameManager.OnInitializeCharacter += OnInitializeCharacter;
            Debug.Log("CharacterManager");
        }

        private void Update()
        {
            if (!_characterInitialized) { return; }

            if (_isHeroDead) {
                Destroy(_character.gameObject);
                Time.timeScale = 0;
                _characterInitialized = false;
                return;
            }

            if (LDGameManager.Instance.CurrentMasterGameState == StateID.None) return;

            if (GameStateMachine.Instance.IsState(StateID.Normal))
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    ActivateAction(_characterSkill.defaultSkill);
                }

                if (Input.GetKeyDown(KeyCode.E))
                {
                    ActivateAction(_characterSkill.ultimateSkill);
                }

                if (_heroInvulnerableTimer > 0)
                {
                    _heroInvulnerableTimer -= Time.deltaTime;
                }

                // cheat
                if (Input.GetKeyDown(KeyCode.Alpha1) && Input.GetKey(KeyCode.LeftShift))
                {
                    AddDamage(1);
                }
            }
        }

        private void OnInitializeCharacter()
        {
            characterTransform = gameManager.HeroTransform;

            SetupActions();

            characterData = gameManager.CharacterData;
            _characterStat = characterData.characterStat.Clone();

            this.health = _characterStat.maxHealth;
            this.maxHealth = _characterStat.maxHealth;
            this.mana = _characterStat.maxMana;
            this.maxMana = _characterStat.maxMana;
            _character = characterTransform.GetComponent<Character>();
            _heroInvulnerableTimer = 0;
            _isHeroDead = false;

            _characterInitialized = true;

            OnInitCharacter?.Invoke();
        }

        void SetupActions()
        {
            foreach (var actionDef in this.characterActions)
            {
                actionDef.action.ActionID = actionDef.actionID;
                CoreLifetimeScope.SharedContainer.Inject(actionDef.action);
                actionDef.action.Setup();
            }

            _characterSkill = gameManager.CharacterSkill;
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

        public void AddDamage(int damage)
        {
            if (_heroInvulnerableTimer > 0) return;
            
            this.health -= damage;
            OnUpdateHealth?.Invoke();

            if(this.health <= 0)
            {
                this.health = 0;
                _isHeroDead = true;
            }

            _heroInvulnerableTimer = 0.3f;
        }

        #region Weapon

        public int GetWeaponDamage(WeaponData weaponData)
        {
            return weaponData.damage;
        }

        #endregion
    }

}