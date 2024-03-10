using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DreamLU
{
    public interface IWeaponProvider
    {
        public int GetWeaponDamage(WeaponData weaponData);
        
        public float GetWeaponAttackSpeed(WeaponData weaponData, InstancedItem instancedItem);
    }

}