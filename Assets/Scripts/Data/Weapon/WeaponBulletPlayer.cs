using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace DreamLU
{
    public class WeaponBulletPlayer : WeaponPlayerBase
    {
        [SerializeField] private bool isColt;
        [ShowIf("isColt"), SerializeField] private int numberColt;
        
        public override void Activate(float aimAngle, float weaponAimAngle, Vector3 weaponAimDirectionVector, bool isSecondWeapon, Vector3 mousePos)
        {
            if(_weaponData == null) return;

            AmmoData ammoData = _weaponData.ammoData;
            if (ammoData != null && _isReady)
            {
                int count = numberColt;
                StartCooldown();
                GameObject ammoPrefab = ammoData.prefab;

                var ammoGO = PoolManager.GetPool(ammoPrefab).RetrieveObject(isSecondWeapon ? _heroPosition.GetWeaponSecondShootPosition() : _heroPosition.GetWeaponShootPosition(), Quaternion.identity, PoolManager.Instance.CurrentTransform);
                var ammo = ammoGO.GetComponent<Ammo>();
                if (ammo != null)
                {
                    ammo.OnCreateAmmo(ammoData, aimAngle, weaponAimAngle, weaponAimDirectionVector);
                    count = count - 1;
                }
                
                if (isColt)
                {
                    StartCoroutine(WaitShot(() =>
                    {
                        var ammoGO2 = PoolManager.GetPool(ammoPrefab).RetrieveObject(
                            isSecondWeapon
                                ? _heroPosition.GetWeaponSecondShootPosition()
                                : _heroPosition.GetWeaponShootPosition(), Quaternion.identity,
                            PoolManager.Instance.CurrentTransform);
                        var ammo2 = ammoGO2.GetComponent<Ammo>();
                        if (ammo2 != null)
                        {
                            ammo2.OnCreateAmmo(ammoData, aimAngle, weaponAimAngle, weaponAimDirectionVector);
                        }
                    }, count));
                }
            }
        }
        
        IEnumerator WaitShot(System.Action callback, int count)
        {
            bool isShoot = false;
            while (count >= 0)
            {
                if (!isShoot)
                {
                    callback?.Invoke();
                    isShoot = true;
                    yield return new WaitForSeconds(0.1f);
                    isShoot = false;
                    count = count - 1;
                }
            }
        }
    }

}