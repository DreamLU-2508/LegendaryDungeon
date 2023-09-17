using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DreamLU
{
    public class AmmoBullet : Ammo
    {
        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Update()
        {
            Vector3 distanceVector = fireAimDir * ammoSpeed * Time.deltaTime;

            transform.position += distanceVector;

            ammoRange -= distanceVector.magnitude;

            if( ammoRange < 0)
            {
                DisableAmmo();
            }
        }

        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            base.OnTriggerEnter2D(collision);
        }

        public override void OnCreateAmmo(AmmoData data, float aimAngle, float weaponAimAngle, Vector3 weaponAimDir, bool overrideAmmoMovement = false)
        {
            base.OnCreateAmmo(data, aimAngle, weaponAimAngle, weaponAimDir, overrideAmmoMovement);
        }
    }

}