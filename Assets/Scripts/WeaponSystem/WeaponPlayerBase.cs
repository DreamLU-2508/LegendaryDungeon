using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        protected bool _isReady;
        // private TrailEffect _trailEffect;

        public float actionTimer = 0;
        
        public InstancedItem InstancedItem => _instancedItem;
        public WeaponData WeaponData => _weaponData;
        public bool IsShutdown => _isShutdown;
        public AttackState AttackState => _attackState;
    }

}