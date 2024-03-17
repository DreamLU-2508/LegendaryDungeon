using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace DreamLU
{
    public class DropItem : MonoBehaviour
    {
        private ItemData _itemData;
        private bool entered;

        private ICharacterActor _characterActor;
        
        public ItemData ItemData => _itemData;

        private void Awake()
        {
            _characterActor = CoreLifetimeScope.SharedContainer.Resolve<ICharacterActor>();
        }

        public void Setup(ItemData itemData)
        {
            _itemData = itemData;
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if(entered) return;

            entered = true;
            _characterActor.DropItem = this;
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if(entered) return;

            entered = true;
            _characterActor.DropItem = null;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            entered = false;
        }
    }
}