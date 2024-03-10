using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DreamLU
{
    public class WeaponBulletPlayer : WeaponPlayerBase
    {
        public override void Activate(float aimAngle, float weaponAimAngle, Vector3 weaponAimDirectionVector, bool isSecondWeapon, Vector3 mousePos)
        {
            if(_weaponData == null) return;

            AmmoData ammoData = _weaponData.ammoData;
            if (ammoData != null && _isReady)
            {
                StartCooldown();
                GameObject ammoPrefab = ammoData.prefab;
                    
                var ammoGO = PoolManager.GetPool(ammoPrefab).RetrieveObject(isSecondWeapon ? _heroPosition.GetWeaponSecondShootPosition() : _heroPosition.GetWeaponShootPosition(), Quaternion.identity, PoolManager.Instance.CurrentTransform);
                var ammo = ammoGO.GetComponent<Ammo>();
                if (ammo != null)
                {
                    ammo.OnCreateAmmo(ammoData, aimAngle, weaponAimAngle, weaponAimDirectionVector);
                }
            }
        }
        
        IEnumerator WaitShot(System.Action callback)
        {
            yield return new WaitForSeconds(0.1f);

            callback?.Invoke();
            
        }
    }

}