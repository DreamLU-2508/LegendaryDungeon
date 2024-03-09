using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DreamLU
{
    [CreateAssetMenu(menuName = "Database/Weapon/WeaponDataManifest")]
    public class WeaponDataManifest : ScriptableObject
    {
        public List<WeaponData> weapons;

        public bool TryGetWeapon(ItemID itemID, out WeaponData weapon)
        {
            weapon = null;
            if (weapons != null)
            {
                weapon = weapons.Find(x => x.itemID == itemID);
                if(weapon != null)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
