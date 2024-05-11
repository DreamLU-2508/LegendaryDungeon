using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DreamLU
{
    public class WeaponShotgunPlayer : WeaponPlayerBase
    {
        [SerializeField] private int projectileCount;
        [SerializeField, Range(0, 180)] private int angle;

        public override void Activate(float aimAngle, float weaponAimAngle, Vector3 weaponAimDirectionVector,
            bool isSecondWeapon, Vector3 mousePos)
        {
            var shots = projectileCount <= 0 ? 1 : projectileCount;
        
            AmmoData ammoData = _weaponData.ammoData;
            if (ammoData != null && _isReady)
            {
                _characterActor.MinusMana(ammoData.manaConsumed,out bool isSuccess);
                if(!isSuccess) return;
                
                StartCooldown();
                for (int i = 0; i < shots; i++)
                {
                    SpawnProjectile(mousePos,isSecondWeapon, i, ammoData);
                }
                
            }
        }
        
        private void SpawnProjectile(Vector3 mousePos, bool isSecondWeapon, int projectileIdx, AmmoData ammoData)
        {
            Vector3 spawnPosition = isSecondWeapon
                ? _heroPosition.GetWeaponSecondShootPosition()
                : _heroPosition.GetWeaponShootPosition();
            Vector3 weaponDirection = mousePos - spawnPosition;
            // First shot always aim at target
           
            var direction = HelperUtilities.SpreadedDirectionInArc(weaponDirection, angle,
                projectileIdx, projectileCount);

            float weaponAngle = CharacterUtilities.GetAngleFromVector(direction);

            createProject(ammoData, spawnPosition, weaponAngle, direction, weaponAngle);
        }

        private void createProject(AmmoData ammoData, Vector3 wpPos, float weaponAimAngle, Vector3 weaponAimDirectionVector, float aimAngle)
        {
            GameObject ammoPrefab = ammoData.prefab;
            var ammoGO = PoolManager.GetPool(ammoPrefab).RetrieveObject(wpPos, Quaternion.identity, PoolManager.Instance.CurrentTransform);
            var ammo = ammoGO.GetComponent<Ammo>();
            if (ammo != null)
            {
                ammo.OnCreateAmmo(ammoData, aimAngle, weaponAimAngle, weaponAimDirectionVector);
            }
        }
        
        
        
    }
}
