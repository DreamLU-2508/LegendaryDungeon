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
        
        // Thời gian delay bắn của vũ khí
        public float weaponPrechargeTime = 0f;

        [Header("Ammo Prefab")]
        public AmmoData ammoData;

        public int damage;
        public float cooldown;
        public int timeReload;

        // [Header("Weapon Player")] 
        public WeaponPlayerBase _weaponPlayerBasePrefab;
    }
}
