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
    public enum TypeResources
    {
        None,
        Gold,
        Mana,
    }
    
    public class DropData
    {
        public GameObject gameObject;
        public bool isWeapon;
        public ItemData itemData;
        public TypeResources typeResources;
    }
    
    public interface IDropItemHandle
    {
        public void DropItemChess(ItemData data, Vector3 startPosition);

        public GameObject ChestPrefab { get; }
        public void Drop(Vector3 position);
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
        
        [Header("Item can drop")]
        [SerializeField] private GameObject goldPrefab;
        [SerializeField] private GameObject manaPrefab;
        [SerializeField] private float speedPickUp;

        [SerializeField] private float goldRate;
        [SerializeField] private float manaRate;
        [SerializeField] private float weaponRate;

        private CharacterManager characterManager;
        private LDGameManager ldGameManager;

        public GameObject ChestPrefab => chestPrefab;
        
        private void Awake()
        {
            characterManager = GetComponent<CharacterManager>();
            ldGameManager = LDGameManager.Instance;
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
                
                // if (_bezier.TryGetP2P3(type, characterManager.CharacterTransform.position, randomPosition,
                //         out var p2, out var p3))
                // {
                //     DOTween.To(() => 0f, x =>
                //     {
                //         item.transform.position =
                //             HelperUtilities.CalculateCubicBezierPoint(x, characterManager.CharacterTransform.position, p2, p3,
                //                 randomPosition);
                //     }, 1, 1);
                // }
                // else
                // {
                //     item.transform.DOMove(randomPosition, 1);
                // }

                item.transform.position = characterManager.CharacterTransform.position;
            }
        }
        
        public void DropItemChess(ItemData data, Vector3 startPosition)
        {
            var prefabData = data.itemDropPrefab;

            Vector2 randomDirection = Random.insideUnitCircle.normalized * Random.Range(minRadius, maxRadius);
            Vector3 randomPosition = startPosition + new Vector3(randomDirection.x, randomDirection.y, 0);
            
            var item = PoolManager.GetPool(prefabData).RetrieveObject(startPosition, Quaternion.identity, PoolManager.Instance.CurrentTransform).GetComponent<DropItem>();
            if (item != null)
            {
                item.Setup(data);
                
                // if (_bezier.TryGetP2P3(type, startPosition, randomPosition,
                //         out var p2, out var p3))
                // {
                //     DOTween.To(() => 0f, x =>
                //     {
                //         item.transform.position =
                //             HelperUtilities.CalculateCubicBezierPoint(x, startPosition, p2, p3,
                //                 randomPosition);
                //     }, 1, 1);
                // }
                // else
                // {
                //     item.transform.DOMove(randomPosition, 1);
                // }
                
                item.transform.position = characterManager.CharacterTransform.position;
            }
        }

        private DropData RollDropItem()
        {
            ChancefTable<DropData> chancefTable = new ChancefTable<DropData>();
            chancefTable.AddRange(goldRate, new DropData()
            {
                gameObject = goldPrefab,
                typeResources = TypeResources.Gold
            });
            chancefTable.AddRange(manaRate, new DropData()
            {
                gameObject = manaPrefab,
                typeResources = TypeResources.Mana
            });
            
            Droppable droppable = _dropDataManifest.RandomChestItemsDroppable(new List<Droppable>());
            if (droppable && droppable is WeaponData itemData)
            {
                chancefTable.AddRange(weaponRate, new DropData()
                {
                    gameObject = itemData.itemDropPrefab,
                    isWeapon = true,
                    itemData = itemData,
                    typeResources = TypeResources.None
                });
            }

            if (chancefTable.CanRoll)
            {
                var uRandom = URandom.CreateSeeded();
                chancefTable.TryRoll(uRandom, null, out var result);
                return result;
            }

            return null;
        }

        public void Drop(Vector3 position)
        {
            DropData dropObject = RollDropItem();
            if (dropObject != null)
            {
                var item = PoolManager.GetPool(dropObject.gameObject).RetrieveObject(position, Quaternion.identity, PoolManager.Instance.CurrentTransform);
                if (dropObject.isWeapon)
                {
                    var dropItem = item.GetComponent<DropItem>();
                    if (dropItem != null)
                    {
                        dropItem.Setup(dropObject.itemData);
                    }
                }
                else
                {
                    var dropItem = item.GetComponent<DropHandleResources>();
                    if (dropItem != null)
                    {
                        dropItem.Setup(speedPickUp, () =>
                        {
                            if (dropObject.typeResources == TypeResources.Gold)
                            {
                                characterManager.AddGoldInGame(ldGameManager.GameConfig.goldPickUp);
                            }
                            else if (dropObject.typeResources == TypeResources.Mana)
                            {
                                characterManager.AddMana(ldGameManager.GameConfig.manaPickUp);
                            }
                        });
                    }
                }
            }
        }

        public void PickUpItem(DropItem dropItem)
        {
            if(dropItem == null) return;
            
            if (dropItem.ItemData.itemType == ItemType.Blueprint)
            {
                DataManager.Instance.AddItemInventory(new GlobalItemDataInventory()
                {
                    itemID = dropItem.ItemData.itemID
                });
                PoolManager.Release(dropItem.gameObject);
            }
        }
    }

}