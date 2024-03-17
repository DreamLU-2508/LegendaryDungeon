using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DreamLU
{
    public interface IDropItemHandle
    {
        public void DropItem(ItemData data, Vector3 startPosition);

        public GameObject ChestPrefab { get; }
    }
    
    public class DropItemHandle : MonoBehaviour, IDropItemHandle
    {
        [SerializeField] private float minRadius = 2f;
        [SerializeField] private float maxRadius = 5f;
        [SerializeField] private BezierCurveDataManifest _bezier;
        [SerializeField] private BezierCurveType type;
        [SerializeField] private GameObject prefab;
        [SerializeField] private DropDataManifest _dropDataManifest;
        [SerializeField] private GameObject chestPrefab;

        private CharacterManager characterManager;

        public GameObject ChestPrefab => chestPrefab;
        
        private void Awake()
        {
            characterManager = GetComponent<CharacterManager>();
        }

        [Button]
        public void TestDropItem()
        {
            if(characterManager.CharacterTransform == null) return;
            
            Vector2 randomDirection = Random.insideUnitCircle.normalized * Random.Range(minRadius, maxRadius);
            Vector3 randomPosition = characterManager.CharacterTransform.position + new Vector3(randomDirection.x, randomDirection.y, 0);
            
            var item = PoolManager.GetPool(prefab).RetrieveObject(characterManager.CharacterTransform.position, Quaternion.identity, PoolManager.Instance.CurrentTransform).GetComponent<DropItem>();
            if (item != null)
            {
                var spriteRenderer = item.GetComponentInChildren<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    var droppable = _dropDataManifest.RandomChestItemsDroppable(new List<Droppable>()
                    {
                        characterManager.Character.WeaponData,
                    });
                    if (droppable is ItemData itemData)
                    {
                        spriteRenderer.sprite = itemData.itemSprite;
                        item.Setup(itemData);
                    }
                }
                
                if (_bezier.TryGetP2P3(type, characterManager.CharacterTransform.position, randomPosition,
                        out var p2, out var p3))
                {
                    DOTween.To(() => 0f, x =>
                    {
                        item.transform.position =
                            HelperUtilities.CalculateCubicBezierPoint(x, characterManager.CharacterTransform.position, p2, p3,
                                randomPosition);
                    }, 1, 1);
                }
                else
                {
                    item.transform.DOMove(randomPosition, 1);
                }
            }
        }
        
        public void DropItem(ItemData data, Vector3 startPosition)
        {
            var prefabData = data.itemDropPrefab;

            Vector2 randomDirection = Random.insideUnitCircle.normalized * Random.Range(minRadius, maxRadius);
            Vector3 randomPosition = startPosition + new Vector3(randomDirection.x, randomDirection.y, 0);
            
            var item = PoolManager.GetPool(prefabData).RetrieveObject(startPosition, Quaternion.identity, PoolManager.Instance.CurrentTransform).GetComponent<DropItem>();
            if (item != null)
            {
                item.Setup(data);
                
                if (_bezier.TryGetP2P3(type, startPosition, randomPosition,
                        out var p2, out var p3))
                {
                    DOTween.To(() => 0f, x =>
                    {
                        item.transform.position =
                            HelperUtilities.CalculateCubicBezierPoint(x, startPosition, p2, p3,
                                randomPosition);
                    }, 1, 1);
                }
                else
                {
                    item.transform.DOMove(randomPosition, 1);
                }
            }
        }
    }

}