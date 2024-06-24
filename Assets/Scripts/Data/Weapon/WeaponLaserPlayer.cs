using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DreamLU
{
    public class WeaponLaserPlayer : WeaponPlayerBase
    {
        private LaserBase laserBase = null;

        public override void Activate(float aimAngle, float weaponAimAngle, Vector3 weaponAimDirectionVector, bool isSecondWeapon, Vector3 mousePos)
        {
            base.Activate(aimAngle, weaponAimAngle, weaponAimDirectionVector, isSecondWeapon, mousePos);
            if (_attackState != AttackState.Attack)
            {
                BeginAction();
            }

            AmmoData ammoData = _weaponData.ammoData;
            if (_attackState == AttackState.Attack)
            {
                if (laserBase == null)
                {
                    laserBase = PoolManager.GetPool(ammoData.prefab).RetrieveObject(isSecondWeapon ? _heroPosition.GetWeaponSecondShootPosition() : _heroPosition.GetWeaponShootPosition(), Quaternion.identity, PoolManager.Instance.CurrentTransform).GetComponent<LaserBase>();
                    laserBase.EnableLaser();
                    laserBase.UpdateLaser(isSecondWeapon ? _heroPosition.GetWeaponSecondShootPosition() : _heroPosition.GetWeaponShootPosition(), mousePos);
                }
                else
                {
                    laserBase.UpdateLaser(isSecondWeapon ? _heroPosition.GetWeaponSecondShootPosition() : _heroPosition.GetWeaponShootPosition(), mousePos);
                }
            }
        }
        
        protected override void EndAction()
        {
            base.EndAction();
            if (laserBase != null)
            {
                laserBase.DisableLaser();
                PoolManager.Release(laserBase.gameObject);
                laserBase = null;
            }
        }
    }
}
