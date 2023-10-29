using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace DreamLU
{
    public class EnemyFire : FireWeapon
    {
        public float coolDownShot = 1f;
        private Enemy enemy;
        private IHeroPositionProvider heroPositionProvider;
        
        protected void Awake()
        {
            enemy = this.GetComponent<Enemy>();
            heroPositionProvider = CoreLifetimeScope.SharedContainer.Resolve<IHeroPositionProvider>();
        }

        protected void Update()
        {
            base.Update();
            if (fireRateCoolDown <= 0 && enemy.CanFire == true)
            {
                OnFire(0, 0, Vector3.zero);
            }
        }

        public override void OnFire(float aimAngle, float weaponAimAngle, Vector3 weaponAimDirectionVector)
        {
            var target = new Vector3(heroPositionProvider.GetTargetPosition().x, heroPositionProvider.GetTargetPosition().y, heroPositionProvider.GetTargetPosition().z);

            if (enemy.Data != null && fireRateCoolDown <= 0)
            {
                GameObject ammoPrefab = enemy.Data.ammoData.prefab;

                float ammoSpeed = Random.Range(enemy.Data.ammoData.minMaxSpeed.x, enemy.Data.ammoData.minMaxSpeed.y);

                IAmmo ammo = (IAmmo)PoolManager.Instance.ReuseComponent(ammoPrefab, enemy.transform.position + new Vector3(0, 1, 0), Quaternion.identity);

                ammo.OnCreateAmmo(enemy.Data.ammoData, target);

                ResetCooldown();
            }
        }

        public override void ResetCooldown()
        {
            fireRateCoolDown = coolDownShot;
        }
    }

}