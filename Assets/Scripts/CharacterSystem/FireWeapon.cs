using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace DreamLU
{
    public class FireWeapon : MonoBehaviour
    {
        protected Character character;

        protected float fireRateCoolDown = 0;

        protected IGameStateProvider gameStateProvider;

        protected void Awake()
        {
            character = GetComponent<Character>();
            gameStateProvider = CoreLifetimeScope.SharedContainer.Resolve<IGameStateProvider>();
        }

        protected void Update()
        {
            fireRateCoolDown -= Time.deltaTime;
        }

        public virtual void OnFire(float aimAngle, float weaponAimAngle, Vector3 weaponAimDirectionVector)
        {
            if(character.WeaponData == null) return;

            AmmoData ammoData = character.WeaponData.ammoData;

            if (ammoData != null && fireRateCoolDown <= 0)
            {
                GameObject ammoPrefab = ammoData.prefab;

                float ammoSpeed = Random.Range(ammoData.minMaxSpeed.x, ammoData.minMaxSpeed.y);

                IAmmo ammo = (IAmmo)PoolManager.Instance.ReuseComponent(ammoPrefab, character.GetWeaponShootPosition(), Quaternion.identity);
                ammo.OnCreateAmmo(ammoData, aimAngle, weaponAimAngle, weaponAimDirectionVector);

                if (character.UseSkillDoubleGun)
                {
                    StartCoroutine(WaitShot(() =>
                    {
                        IAmmo ammo2 = (IAmmo)PoolManager.Instance.ReuseComponent(ammoPrefab, character.GetWeaponSecondShootPosition(), Quaternion.identity);
                        ammo2.OnCreateAmmo(ammoData, aimAngle, weaponAimAngle, weaponAimDirectionVector);
                    }));
                }

                ResetCooldown();
            }
        }

        IEnumerator WaitShot(System.Action callback)
        {
            yield return new WaitForSeconds(0.1f);

            callback?.Invoke();
        }

        public virtual void ResetCooldown()
        {
            fireRateCoolDown = gameStateProvider.GameConfig.fireRateCoolDown;
        }
    }
}
