using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DreamLU
{
    public enum CharacterActionID
    {
        None,
        Dash,
        Teleport,
        DoubleGun,
        AttachmentEffects,
        StopMovingTime
    }

    public abstract class CharacterAction : MonoBehaviour
    {
        [Header("Timing")][SerializeField] protected bool hasCooldown;

        [ShowIf("hasCooldown")]
        [SerializeField]
        private float baseCooldown = 1;

        protected float cooldown;

        protected float _cooldownTime = 0;

        protected bool _isInAction;

        private bool _cooldownRequested = false;

        public CharacterActionID ActionID { get; set; }

        public abstract bool IsCompleted();

        public virtual bool IsInCooldown()
        {
            return hasCooldown && Time.time < _cooldownTime;
        }

        /// <summary>
        /// Return true if all requirements (beside cooldown) is met.
        /// </summary>
        public virtual bool IsActivable()
        {
            return true;
        }

        protected virtual void Update()
        {
        }

        public virtual void Shutdown()
        {
            this.enabled = false;
            _isInAction = false;

            if (!_cooldownRequested)
            {
                _cooldownTime = Time.time + cooldown;
            }
            else
            {
                _cooldownTime = 0;
                _cooldownRequested = false;
            }
        }

        public virtual void Activate()
        {
            this.enabled = true;
            _isInAction = true;
            _cooldownTime = Time.time + cooldown;
            _cooldownRequested = false;
        }

        public virtual void Setup()
        {
            this.enabled = false;
            cooldown = baseCooldown;
        }

        /// <summary>
        /// Update the action with new instantiated character
        /// </summary>
        public virtual void Reload() { }
    }

}