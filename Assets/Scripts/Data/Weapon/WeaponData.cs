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

        public bool isInfiniteAmmo = false;

        // Thời gian delay bắn của vũ khí
        public float weaponPrechargeTime = 0f;

    }
}
