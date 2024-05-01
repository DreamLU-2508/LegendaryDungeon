using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace DreamLU
{
    public class AmmoMovement : MonoBehaviour
    {
        private float _ammoRange;
        private float _ammoSpeed;
        private Vector3 _direction;
        private Vector3 _sourcePosition;
        private bool _isMoveWithDir;
        private float _ammoDamage;
        
        private SpriteRenderer spriteRenderer;
        private Vector3 positonTarget;

        private bool isAvtive;

        private ICharacterActor _characterActor;
        
        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            _characterActor = CoreLifetimeScope.SharedContainer.Resolve<ICharacterActor>();
        }

        private void Update()
        {
            if(!isAvtive) return;

            if (!_isMoveWithDir)
            {
                transform.position = Vector3.MoveTowards(transform.position, positonTarget, this._ammoSpeed * Time.deltaTime);
                if(transform.position == positonTarget)
                {
                    DisableAmmo();
                }
            }
            else
            {
                Vector3 newPosition = transform.position + _direction * _ammoSpeed * Time.deltaTime;
                transform.position = newPosition;
                var dis =  Vector3.Distance(transform.position, _sourcePosition);
                if (dis > _ammoRange)
                {
                    DisableAmmo();
                }
            }
        }
        
        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            DisableAmmo();
            var character = collision.GetComponent<Character>();
            if (character != null)
            {
                _characterActor.AddDamage((int)_ammoDamage);
            }
        }
        
        protected virtual void DisableAmmo()
        {
            isAvtive = false;
            PoolManager.Release(this.gameObject);
        }

        #region API

        public void OnActive(bool isMoveWithDir, Vector3 dir, float speed, float range, Vector3 sourcePos, Vector3 targetPos, float damage)
        {
            _isMoveWithDir = isMoveWithDir;
            _direction = dir;
            _ammoSpeed = speed;
            _ammoRange = range;
            _sourcePosition = sourcePos;
            _ammoDamage = damage;
            positonTarget = targetPos;
            isAvtive = true;
        }

        #endregion
    }
}
