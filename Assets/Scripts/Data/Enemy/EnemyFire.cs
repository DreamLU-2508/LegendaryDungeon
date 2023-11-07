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

        }

        public override void OnFire(float aimAngle, float weaponAimAngle, Vector3 weaponAimDirectionVector)
        {

        }

        public override void ResetCooldown()
        {
            fireRateCoolDown = coolDownShot;
        }
    }

}