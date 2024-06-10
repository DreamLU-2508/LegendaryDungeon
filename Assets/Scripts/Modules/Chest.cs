using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;

namespace DreamLU
{
    public class Chest : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private DropDataManifest _dropDataManifest;

        private IDropItemHandle _handle;
        private CharacterManager _characterManager;

        private void Awake()
        {
            _handle = CoreLifetimeScope.SharedContainer.Resolve<IDropItemHandle>();
            _characterManager = FindObjectOfType<CharacterManager>();
            animator.SetBool(Settings.useChest, false);
        }

        private bool entered = false;
        private bool opened = false;
        private void Update()
        {
            if(opened) return;

            if (entered && Input.GetKeyDown(KeyCode.F))
            {
                Open();
            }
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if(entered) return;
            entered = true;
        }
        
        private void OnTriggerExit2D(Collider2D col)
        {
            entered = false;
        } 
        
        private void OnTriggerStay2D(Collider2D col)
        {
            if(entered) return;
            entered = true;
        } 

        private void Open()
        {
            Vector3 startPos = transform.position;
            List<Droppable> excludedList = new List<Droppable>();

            if (_characterManager.Character.WeaponData != null)
            {
                excludedList.Add(_characterManager.Character.WeaponData);
            }

            Droppable droppable = _dropDataManifest.RandomChestItemsDroppable(excludedList);
            if (droppable && droppable is ItemData itemData)
            {
                _handle.DropItemChess(itemData, startPos);
                animator.SetBool(Settings.useChest, true);
                opened = true;
            }
        }
    }

}