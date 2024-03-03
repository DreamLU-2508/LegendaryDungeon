using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace DreamLU
{
    public enum PowerUpType
    {
        WeaponStat,
        CharacterStat
    }
    
    public enum PowerUpStatID
    {
        None = 0,
        // Stat Powerup 
        MaxHealth,
        MaxMana,
        MaxShield,
        // Amount,
        // Recovery,
        // Armor,
        // MoveSpeedMod,
        // DamageMod,
        // AreaMod,
        // SpeedMod,
        // DurationMod,        
        CooldownSkillMod,
        // Luck,
        // ExperienceGainMod,
        // GoldGainMod,
        // CurseMod,
        // AttractorMod,
        // Revival,
        // RarityUpgrade,
        // AbilityCooldownMod,
        // ReRollCount,
        // ReRollRelicCount,
        // SkipCount,
        // BanishCount,
        // PickupDropChanceMod,
        //
        // // Weapon Powup Up
        // WeaponAmount = 100,
        // WeaponDamage,
        WeaponSpeed,
        // WeaponDuration,
        // WeaponArea,
        // WeaponCooldown,
        // WeaponForce, // Knockback
        // WeaponCritChance, // 0..1
        // WeaponCritDamage, // 2 = 200%. 3 = 300%
        // WeaponPassthroughCount,
        // WeaponDamageMod,
        // WeaponSpeedMod,
        // WeaponDurationMod,
        // WeaponAreaMod,
        // WeaponCooldownMod,
        // WeaponChainCount,
        // WeaponProcChance,
        // WeaponChargeCount,
    }
    
    [System.Serializable]
    public class  PowerUp
    {
        public PowerUpType powerUpType;
        public PowerUpStatID powerUpStatID;
        public float value;
        public string description;

        // /// <summary>
        // /// The chance to roll the powerup from Powerup Pool
        // /// </summary>
        // [ShowIf("IsWeaponPowerUp")]
        // public float rollChance = 1.0f;

        // [Tooltip("Maximum times this powerup can be chosen")]
        // public int cap = 10000;

        // public bool hasPairing;
        // [ShowIf("hasPairing")] public PairedPowerUp pairedPowerUp;

        // public bool HasPairingPowerup => hasPairing && pairedPowerUp.powerupStatID != PowerUpStatID.None;
        
        // public static bool IsPercentageStat(PowerUpStatID stat)
        // {
        //     return 
        //         (stat == PowerUpStatID.WeaponSpeed) ||
        //         // Weapon Mods
        //         (stat == PowerUpStatID.WeaponProcChance) ||
        //         (stat == PowerUpStatID.WeaponCritChance) || (stat == PowerUpStatID.WeaponDamageMod) || (stat == PowerUpStatID.WeaponSpeedMod)
        //         || (stat == PowerUpStatID.WeaponDurationMod) || (stat == PowerUpStatID.WeaponAreaMod) || (stat == PowerUpStatID.WeaponCooldownMod)
        //         // Passive Mods
        //         || (stat == PowerUpStatID.MaxHealthMod) || (stat == PowerUpStatID.MoveSpeedMod) || (stat == PowerUpStatID.AreaMod) || (stat == PowerUpStatID.SpeedMod) || (stat == PowerUpStatID.MoveSpeedMod) || (stat == PowerUpStatID.DamageMod) || (stat == PowerUpStatID.DurationMod) || (stat == PowerUpStatID.CooldownMod) || (stat == PowerUpStatID.AttractorMod)
        //         || (stat == PowerUpStatID.AbilityCooldownMod)
        //         || (stat == PowerUpStatID.GoldGainMod) || (stat == PowerUpStatID.ExperienceGainMod)
        //         || (stat == PowerUpStatID.PickupDropChanceMod)
        //         ;
        // }
        
        // public static bool IsFloatStat(PowerUpStatID stat)
        // {
        //     return (stat == PowerUpStatID.WeaponDamage ||  stat == PowerUpStatID.WeaponCooldown || stat == PowerUpStatID.Recovery || stat == PowerUpStatID.Armor || stat == PowerUpStatID.WeaponDuration);
        // }

        // public static bool IsFluralStat(PowerUpStatID stat)
        // {
        //     return stat == PowerUpStatID.WeaponAmount ;
        // }

        // public static bool IsAdditiveModifierStat(PowerUpStatID stat)
        // {
        //     return
        //         (stat == PowerUpStatID.WeaponDamageMod) ||
        //         (stat == PowerUpStatID.WeaponSpeedMod) || 
        //         (stat == PowerUpStatID.WeaponDurationMod) || 
        //         (stat == PowerUpStatID.WeaponAreaMod);
        // }

        // public static bool IsSubtractiveModifierStat(PowerUpStatID stat)
        // {
        //     return (stat == PowerUpStatID.CooldownMod) || (stat == PowerUpStatID.AbilityCooldownMod);
        // }
        
        // public bool IsWeaponPowerup => powerupType == PowerUpType.WeaponStat;
        // public static bool IgnoreRarity(PowerUpStatID stat)
        // {
        //     return
        //         (stat == PowerUpStatID.Amount) ||
        //         (stat == PowerUpStatID.Revival) ||
        //         (stat == PowerUpStatID.Luck);
        // }
        // public Powerup Clone()
        // {
        //     return (Powerup)this.MemberwiseClone();
        // }

        // public override string ToString()
        // {
        //     return $"POWERUP({powerupStatID}, val {value} roll {rollChance} cap {cap})";
        // }
    }
}