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

    public class CharacterManager : MonoBehaviour, ICharacterActor, IWeaponProvider, ICharacterManager
    {
        public static CharacterManager Instance;
        
        [SerializeField][TableList] private List<CharacterActionDefinition> characterActions;
        [SerializeField] private DropItemHandle _dropItemHandle;

        private LDGameManager gameManager;

        private Transform characterTransform;
        private bool _isMovementLocked;
        private bool _isActionLocked = false;
        private bool _characterInitialized = false;
        private CharacterSkill _characterSkill;
        private int health;
        private int mana;
        private int shield;
        private int maxMana;
        private int maxHealth;
        private int maxShield;
        private int goldInGame;
        private CharacterData _characterData;
        private Character _character;
        private float _heroInvulnerableTimer = 0;
        private bool _isHeroDead;
        private CharacterStat _characterStat;
        private List<CardData> _cards = new();

        public bool IsHeroDead
        {
            get => _isHeroDead;
            set => _isHeroDead = value;
        }

        public Character Character => _character;
        public CharacterStat CharacterStat => _characterStat;
        
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
        
        public bool IsActionLocked
        {
            get => _isActionLocked;
            set => _isActionLocked = value;
        }

        public bool UseSkillDoubleGun
        {
            get
            {
                if (_character == null) return false;

                return _character.UseSkillDoubleGun;
            }
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
        public int GoldInGame
        {
            get { return goldInGame; }
            set 
            { 
                goldInGame = value;
                OnUpdateGoldInGame?.Invoke();
            }
        } 
        
        [ShowInInspector, ReadOnly]
        public int Shield
        {
            get { return shield; }
            set 
            { 
                shield = value;
                OnUpdateShield?.Invoke();
            }
        }

        [ShowInInspector, ReadOnly]
        public int MaxHealth
        {
            get
            {
                // Max Health Never fall below zero
                if (_characterStat != null)
                {
                    return Mathf.Max(_characterStat.maxHealth + (int)_characterStat.GetStatBaseByID(PowerUpStatID.vitality), 1);
                }
                else
                {
                    return 1;
                }
            }
        }

        [ShowInInspector, ReadOnly]
        public int MaxShield
        {
            get
            {
                // Max Health Never fall below zero
                if (_characterStat != null)
                {
                    return Mathf.Max(_characterStat.maxShield + (int)_characterStat.GetStatBaseByID(PowerUpStatID.shieldBonus), 1);
                }
                else
                {
                    return 1;
                }
            }
        }

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
        public int MaxMana
        {
            get
            {
                if (_characterStat != null)
                {
                    return Mathf.Max(
                        _characterStat.maxMana + (int)_characterStat.GetStatBaseByID(PowerUpStatID.manaBonus), 1);
                }
                else
                {
                    return 1;
                }
            }
        }

        // Action
        public event System.Action OnInitCharacter;
        public event System.Action OnUpdateHealth;
        public event System.Action OnUpdateShield;
        public event System.Action OnUpdateMana;
        public event System.Action OnUpdateGoldInGame;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            
            gameManager = FindObjectOfType<LDGameManager>();
            gameManager.OnInitializeCharacter += OnInitializeCharacter;
            Debug.Log("CharacterManager");
        }

        private void Update()
        {
            if (!_characterInitialized) { return; }

            if (_isHeroDead) {
                Destroy(_character.gameObject);
                _characterInitialized = false;
                GameStateMachine.Instance.ChangeState(StateID.GameOver);
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

                ShieldRecovery();

                // cheat
                if (Input.GetKeyDown(KeyCode.Alpha1) && Input.GetKey(KeyCode.LeftShift))
                {
                    AddDamage(1);
                }

                if (dropItem != null)
                {
                    if (Input.GetKeyDown(KeyCode.F))
                    {
                        if (dropItem.ItemData is WeaponData weaponData && HandleChangeWeapon(weaponData))
                        {
                            PoolManager.Release(dropItem.gameObject);
                            dropItem = null;
                        }
                    }
                }
            }
        }

        private float timeShieldRecovery = 0;
        private void ShieldRecovery()
        {
            if(this.shield >= MaxShield) return;
            
            timeShieldRecovery -= Time.deltaTime;

            if (timeShieldRecovery <= 0)
            {
                Shield = this.shield + 1;
                timeShieldRecovery = gameManager.GameConfig.timeShieldRecovery;
            }
        }

        private void OnInitializeCharacter()
        {
            characterTransform = gameManager.HeroTransform;
        
            SetupActions();

            _characterData = gameManager.CharacterData;
            _characterStat = _characterData.characterStat.Clone();

            this.health = _characterStat.maxHealth;
            this.maxHealth = _characterStat.maxHealth;
            this.mana = _characterStat.maxMana;
            this.maxMana = _characterStat.maxMana;
            this.shield = _characterStat.maxShield;
            this.maxShield = _characterStat.maxShield;
            _character = characterTransform.GetComponent<Character>();
            _heroInvulnerableTimer = 0;
            _isHeroDead = false;
            _cards = new List<CardData>();
            _characterInitialized = true;
            _isActionLocked = false;
            _isMovementLocked = false;
            // Time.timeScale = 1;
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
        
        bool CheckIsInDashAction()
        {
            var actionDef = characterActions.Find((x) => x.actionID == CharacterActionID.Dash);
            if (actionDef != null)
            {
                return actionDef.action.IsInAction();
            }

            return false;
        }

        public void AddDamage(int damage)
        {
            if (_heroInvulnerableTimer > 0) return;
            
            if(CheckIsInDashAction()) return;

            if (shield > 0)
            {
                this.shield -= damage;
                if (timeShieldRecovery <= 0)
                {
                    timeShieldRecovery = gameManager.GameConfig.timeShieldRecovery;
                }
                OnUpdateShield?.Invoke(); 
            }
            else
            {
                this.health -= damage;
                OnUpdateHealth?.Invoke();   
            }

            if(this.health <= 0)
            {
                this.health = 0;
                _isHeroDead = true;
            }

            _heroInvulnerableTimer = 0.3f;
        }

        public void AddCard(CardData cardData)
        {
            _cards.Add(cardData);
        }

        #region Weapon

        public int GetWeaponDamage(WeaponData weaponData)
        {
            return weaponData.damage;
        }

        public float GetWeaponAttackSpeed(WeaponData weaponData, InstancedItem instancedItem)
        {
            return weaponData.cooldown;
        }

        #endregion

        public void RebuildStat()
        {
            _characterStat.CopyStats(_characterData.characterStat);

            if (_cards.Count > 0)
            {
                foreach (var cardData in _cards)
                {
                    _characterStat.AddPowerup(new PowerUp()
                    {
                        powerUpStatID = cardData.powerUp.powerUpStatID,
                        value = cardData.powerUp.value,
                    });
                }
            }

            OnUpdateHealth?.Invoke();
            OnUpdateMana?.Invoke();
            OnUpdateShield?.Invoke();
        }

        public void MinusMana(int manaConsumed, out bool isSuccess)
        {
            var newMana = this.mana - manaConsumed;
            if (newMana >= 0)
            {
                isSuccess = true;
                Mana = newMana;
            }
            else
            {
                isSuccess = false;
            }
        }
        
        public void AddMana(int mana)
        {
            Mana += mana;
        }

        [ShowInInspector, ReadOnly]
        private DropItem dropItem = null;
        public DropItem DropItem
        {
            set
            {
                dropItem = value;
            }
        }

        public float GetDistancePickUp()
        {
            return LDGameManager.Instance.GameConfig.distancePickUpBase;
        }

        bool HandleChangeWeapon(WeaponData weaponData)
        {
            if(_character == null) return false;

            var oldWeapon = _character.WeaponData; 
            _character.SetWeapon(weaponData);

            if (oldWeapon != null)
            {
                _dropItemHandle.DropItemChess(oldWeapon, _character.transform.position);
            }

            return true;
        }

        public void AddGoldInGame(int gold)
        {
            GoldInGame += gold;
        }
    }

}