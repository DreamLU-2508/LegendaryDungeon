using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace DreamLU
{
    [CreateAssetMenu(fileName = "WP_", menuName = "Database/Weapon/Weapon Data")]
    public class WeaponData : ItemData
    {
        public Vector3 weaponShootPosition;
        
        [Header("Ammo Prefab")]
        public AmmoData ammoData;

        public int damage;
        public float cooldown;

        // [Header("Weapon Player")] 
        public WeaponPlayerBase _weaponPlayerBasePrefab;

        public bool isLock;
    }
}
