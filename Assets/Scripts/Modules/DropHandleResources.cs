using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace DreamLU
{
    public class DropHandleResources : MonoBehaviour
    {
        private ICharacterActor _characterActor;
        private bool isDestroy = false;
        private bool isAnimation = false;
        private float speed;
        private Action _callback = null;

        public void Setup(float moveSpeed, Action callback = null)
        {
            isDestroy = false;
            isAnimation = false;
            speed = moveSpeed;
            _callback = callback;
        }
        
        private void Awake()
        {
            _characterActor = CoreLifetimeScope.SharedContainer.Resolve<ICharacterActor>();
        }

        private void Update()
        {
            if(_characterActor.IsHeroDead) return;

            if (isDestroy) return;

            if (isAnimation)
            {
                transform.position = Vector3.MoveTowards(transform.position, _characterActor.CharacterTransform.position, speed *Time.deltaTime);
                // transform.position = Vector3.Lerp(transform.position, _characterActor.CharacterTransform.position, speed *Time.deltaTime);
                if (Vector3.Distance(_characterActor.CharacterTransform.position, transform.position) < 0.3f)
                {
                    isDestroy = true;
                    _callback?.Invoke();
                    PoolManager.Release(this.gameObject);
                }
            }

            var distance = Vector3.Distance(_characterActor.CharacterTransform.position, transform.position);
            if (distance <= _characterActor.GetDistancePickUp())
            {
                isAnimation = true;
            }
        }

        private void Reset()
        {
            speed = 0;
            _callback = null;
        }
    }
}