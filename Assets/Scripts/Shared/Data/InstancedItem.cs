using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DreamLU
{
    public interface IDistributorValueProvider
    {
        public float DistributeValue { get; }
    }
    
    public class InstancedItem : IDistributorValueProvider
    {
        private int instanceID;
        private ItemData itemData;
        private ItemRarity tier;
        
        public ItemData ItemData => itemData;
        public ItemRarity Rarity => tier;
        
        private static int _instanceIDSeed = 0;
        private int _killCount = 0;
        
        public InstancedItem(ItemData itemData, ItemRarity tier)
        {
            this.itemData = itemData;
            
            this.tier = tier;
            this.instanceID = ++_instanceIDSeed;
        }
        
        public int KillCount
        {
            get => _killCount;
            set => _killCount = value;
        }
        
        public int InstanceID
        {
            get => instanceID;
        }
        
        public int Price => ItemData.price;
        
        public static ItemRarity MaxTier() => ItemRarity.Tier5;
        
        // public bool GetIntrinsicStat(ItemStatID itemStatID, out float val)
        // {
        //     val = 0;
        //     if (itemData is InventoryWeaponData weaponData)
        //     {
        //         switch (itemStatID)
        //         {
        //             case ItemStatID.Damage:
        //                 if (weaponData.damage.Count == 0) return false;
        //                 val = InventoryItemData.GetTieredDamage(weaponData.damage, ItemTier, 0).baseDamage;
        //                 return true;
        //             case ItemStatID.AttackSpeed:
        //                 if (weaponData.attackSpeed.Count == 0) return false;
        //                 val = InventoryItemData.GetTieredValue(weaponData.attackSpeed, ItemTier, 0);
        //                 return true;
        //             case ItemStatID.Range:
        //                 if (weaponData.range.Count == 0) return false;
        //                 val = InventoryItemData.GetTieredValue(weaponData.range, ItemTier, 0);
        //                 return true;
        //             case ItemStatID.Knockback:
        //                 if (weaponData.knockback.Count == 0) return false;
        //                 val = InventoryItemData.GetTieredValue(weaponData.knockback, ItemTier, 0);
        //                 return true;
        //             case ItemStatID.CritChange:
        //                 if (weaponData.critChance.Count == 0) return false;
        //                 val = InventoryItemData.GetTieredValue(weaponData.critChance, ItemTier, 0);
        //                 return true;
        //             case ItemStatID.CritModifier:
        //                 if (weaponData.critModifier.Count == 0) return false;
        //                 val = InventoryItemData.GetTieredValue(weaponData.critModifier, ItemTier, 0);
        //                 return true;
        //             case ItemStatID.Bounces:
        //                 if (weaponData.bounces.Count == 0) return false;
        //                 val = InventoryItemData.GetTieredValue(weaponData.bounces, ItemTier, 0);
        //                 return true;
        //             case ItemStatID.Projectiles:
        //                 if (weaponData.projectiles.Count == 0) return false;
        //                 val = InventoryItemData.GetTieredValue(weaponData.projectiles, ItemTier, 0);
        //                 return true;
        //             case ItemStatID.LifeSteal:
        //                 if (weaponData.lifeSteal.Count == 0) return false;
        //                 val = InventoryItemData.GetTieredValue(weaponData.lifeSteal, ItemTier, 0);
        //                 return true;
        //             case ItemStatID.Pierce:
        //                 if (weaponData.pierce.Count == 0) return false;
        //                 val = InventoryItemData.GetTieredValue(weaponData.pierce, ItemTier, 0);
        //                 return true;
        //             case ItemStatID.AttackSpeedBonus:
        //                 if(!InventoryItemData.GetKeyedTieredValue(weaponData.secondaryStats,tier,ItemStatID.AttackSpeedBonus,out val)) return false;
        //                 return true;
        //             case ItemStatID.DamageBonus:
        //                 if(!InventoryItemData.GetKeyedTieredValue(weaponData.secondaryStats,tier,ItemStatID.DamageBonus,out val)) return false;
        //                 return true;
        //             case ItemStatID.AreaOfEffect:
        //                 if(weaponData.areaOfEffect.Count == 0) return false;
        //                 val = InventoryItemData.GetTieredValue(weaponData.areaOfEffect,ItemTier,0);
        //                 return true;
        //             default:
        //                 break;
        //         }
        //     }
        //     return false;
        // }
        
        public float DistributeValue { get; }
    }

}