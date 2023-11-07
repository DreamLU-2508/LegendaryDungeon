using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DreamLU
{
    [CreateAssetMenu(fileName = "WP_", menuName = "Database/Weapon/Weapon Data")]
    public class WeaponData : ScriptableObject
    {
        public WeaponID weaponID;
        public string weaponName;

        public Vector3 weaponShootPosition;

        public Sprite weaponSprite;

        // Thời gian delay bắn của vũ khí
        public float weaponPrechargeTime = 0f;

        [Header("Ammo Prefab")]
        public AmmoData ammoData;

        public int damage;
        public float cooldown;
        public int timeReload;
    }
}
