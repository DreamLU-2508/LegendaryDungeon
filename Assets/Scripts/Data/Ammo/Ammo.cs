using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DreamLU
{
    public class Ammo : MonoBehaviour, IAmmo
    {
        protected float ammoRange;
        protected float ammoSpeed;
        protected Vector3 fireAimDir;
        protected float fireAimAngle;
        protected bool overrideAmmoMovement;

        protected SpriteRenderer spriteRenderer;
        protected AmmoData ammoData;
        private Vector3 positonTarget;
        private bool isTargetChar = false;

        protected virtual void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        protected virtual void Update()
        {
            if(isTargetChar)
            {
                transform.position = Vector3.MoveTowards(transform.position, positonTarget, this.ammoSpeed * Time.deltaTime);
                if(transform.position == positonTarget)
                {
                    DisableAmmo();
                }
            }
        }

        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            DisableAmmo();
        }

        protected virtual void DisableAmmo()
        {
            PoolManager.Release(this.gameObject);
            var vfx = Instantiate(ammoData.vfxPrefabHit);
            vfx.transform.position = this.transform.position;
            Destroy(vfx,1);
        }

        public virtual void OnCreateAmmo(AmmoData data, float aimAngle, float weaponAimAngle, Vector3 weaponAimDir, bool overrideAmmoMovement = false)
        {
            this.ammoData = data;
            SetFireDirection(data, aimAngle, weaponAimAngle, weaponAimDir);
            spriteRenderer.sprite = data.ammoSprite;
            this.ammoRange = data.range;
            this.ammoSpeed = Random.Range(data.minMaxSpeed.x, data.minMaxSpeed.y);
            this.overrideAmmoMovement = overrideAmmoMovement;

            this.gameObject.SetActive(true);
        }

        public virtual void OnCreateAmmo(AmmoData data, Vector3 target)
        {
            this.ammoData = data;
            spriteRenderer.sprite = data.ammoSprite;
            this.ammoRange = data.range;
            this.ammoSpeed = Random.Range(data.minMaxSpeed.x, data.minMaxSpeed.y);

            isTargetChar = true;
            positonTarget = target;
            this.gameObject.SetActive(true);
        }

        private void SetFireDirection(AmmoData data, float aimAngle, float weaponAimAngle, Vector3 weaponAimDir)
        {
            float randomSpeard = Random.Range(data.minMaxSpread.x, data.minMaxSpread.y);

            int speardToggle = Random.Range(0, 2) * 2 - 1;

            if(weaponAimDir.magnitude < Settings.useAimAngleDistance)
            {
                fireAimAngle = aimAngle;
            }
            else
            {
                fireAimAngle = weaponAimAngle;
            }

            fireAimAngle += speardToggle * randomSpeard;

            transform.eulerAngles = new Vector3(0, 0, fireAimAngle);

            Vector3 dirVector = new Vector3(Mathf.Cos(Mathf.Deg2Rad * fireAimAngle), Mathf.Sin(Mathf.Deg2Rad * fireAimAngle), 0);

            fireAimDir = dirVector;
        }

        public virtual GameObject GetGameObject()
        {
            return this.gameObject;
        }
    }
}
