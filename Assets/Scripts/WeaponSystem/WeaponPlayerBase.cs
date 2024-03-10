using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;

namespace DreamLU
{
    public enum AttackState
    {
        Anticipate = 0,
        Attack = 1, // RangedWeapon use this to update position
        Finish = 2, // Unsed
    }
    
    public class WeaponPlayerBase : MonoBehaviour
    {
        [SerializeField] protected bool enableWeaponEmission;
        
        protected InstancedItem _instancedItem;
        protected WeaponData _weaponData;
        protected bool _isShutdown = false;
        protected AttackState _attackState;
        protected bool _isInAction;
        [ShowInInspector, ReadOnly]
        protected bool _isReady;
        // private TrailEffect _trailEffect;
        
        protected IWeaponProvider _statProvider;
        protected ICharacterActor _characterActor;
        protected IHeroPositionProvider _heroPosition;

        public float actionTimer = 0;
        
        public InstancedItem InstancedItem => _instancedItem;
        public WeaponData WeaponData => _weaponData;
        public bool IsShutdown => _isShutdown;
        public AttackState AttackState => _attackState;
        
        public bool IsInAction => _isInAction;
        
        public bool IsReady
        {
            get => _isReady;
            set
            {
                if (_isReady != value)
                {
                    _isReady = value;
                }
            }
        }
        
        public event System.Action OnBeginAction;
        public event System.Action OnEndAction;

        public event System.Action<AttackState> OnAttackState;
        
        public virtual void SetData(InstancedItem data)
        {
            _instancedItem = data;
            _weaponData = _instancedItem.ItemData as WeaponData;
        }
        
        protected virtual void BeginAction()
        {
            _isInAction = true;
            // _statProvider.ItemActionBegin(_instancedItem);
            _attackState = AttackState.Anticipate;
            OnBeginAction?.Invoke();
        }
        
        protected void BeginState(AttackState state)
        {
            _attackState = state;
            OnAttackState?.Invoke(state);
        }
        
        protected virtual void EndAction()
        {
            _isInAction = false;
            OnEndAction?.Invoke();
        }

        private void Awake()
        {
            _statProvider = CoreLifetimeScope.SharedContainer.Resolve<IWeaponProvider>();
            _characterActor = CoreLifetimeScope.SharedContainer.Resolve<ICharacterActor>();
            _heroPosition = CoreLifetimeScope.SharedContainer.Resolve<IHeroPositionProvider>();
        }

        protected virtual void Start()
        {
            _isReady = true;
        }
        
        protected virtual float GetCooldown()
        {
            return _statProvider.GetWeaponAttackSpeed(_weaponData, _instancedItem);
        }

        protected virtual float GetActionTimerScale() => 1f;
        
        private bool ShouldPlay()
        {
            if (_isInAction)
            {
                return false;
            }

            return CheckActiveAble();
        }
        
        protected virtual bool CheckActiveAble()
        {
            return true;
        }
        
        protected virtual bool ShouldCooldown()
        {
            return true;
        }
        
        protected virtual void OnUpdate()
        {
            // Subclass can override this method to add update behavior
        }
        
        protected virtual void OnCooldown(bool activated)
        {
            // Subclass can override this method to add update behavior
        }
        
        protected virtual void Update()
        {
            if (!_isReady && actionTimer > 0)
            {
                actionTimer -= GetActionTimerScale() * Time.deltaTime;
            }

            if (actionTimer <= 0)
            {
                _isReady = true;
            }
        }

        protected virtual void StartCooldown()
        {
            _isReady = false;
            actionTimer = GetCooldown();
        }
        
        /// <summary>
        /// Called before Start().
        /// </summary>
        public virtual void Setup()
        {
            // Subclass customization
        }
        
        public void DisableWeapon()
        {
            Shutdown();
        }
        
        /// <summary>
        /// This base method should be called in overriden methods.
        /// </summary>
        public virtual void Shutdown()
        {
            this.gameObject.SetActive(false);
            _isShutdown = true;
        }
        
        public virtual void DestroySelf()
        {
            Destroy(this.gameObject);
        }
        
        public virtual void Activate(float aimAngle, float weaponAimAngle, Vector3 weaponAimDirectionVector, bool isSecondWeapon)
        {
        }
    }

}