using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace DreamLU
{
    public class FireWeapon : MonoBehaviour
    {
        private Character character;

        private float fireRateCoolDown = 0;

        private IGameStateProvider gameStateProvider;

        private void Awake()
        {
            character = GetComponent<Character>();
            gameStateProvider = CoreLifetimeScope.SharedContainer.Resolve<IGameStateProvider>();
        }

        private void Update()
        {
            fireRateCoolDown -= Time.deltaTime;
        }

        public void OnFire(float aimAngle, float weaponAimAngle, Vector3 weaponAimDirectionVector)
        {
            if(character.WeaponData == null) return;

            AmmoData ammoData = character.WeaponData.ammoData;

            if (ammoData != null && fireRateCoolDown <= 0)
            {
                GameObject ammoPrefab = ammoData.prefab;

                float ammoSpeed = Random.Range(ammoData.minMaxSpeed.x, ammoData.minMaxSpeed.y);

                IAmmo ammo = (IAmmo)PoolManager.Instance.ReuseComponent(ammoPrefab, character.GetWeaponShootPosition(), Quaternion.identity);

                ammo.OnCreateAmmo(ammoData, aimAngle, weaponAimAngle, weaponAimDirectionVector);

                ResetCooldown();
            }
        }

        public void ResetCooldown()
        {
            fireRateCoolDown = gameStateProvider.GameConfig.fireRateCoolDown;
        }
    }
}
