using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Serialization;

namespace DreamLU
{
    [System.Serializable]
    public class CharacterStat
    {
        public int maxHealth;
        public int maxMana;
        public int maxShield;
        public int vitality;
        public float attackSpeed;
        public int manaBonus;
        public int shieldBonus;
        public float cooldownSkillMod;

        public CharacterStat Clone()
        {
            return (CharacterStat)this.MemberwiseClone();
        }
        
        public void CopyStats(CharacterStat stats)
        {
            System.Type type = this.GetType();
            FieldInfo[] fields = type.GetFields();

            for (int i = 0; i < fields.Length; i++)
            {
                fields[i].SetValue(this, fields[i].GetValue(stats));
            }
        }
        
        public void AddPowerup(PowerUp powerup)
        {
            float val = powerup.value;
            
            switch (powerup.powerUpStatID)
            {
                case PowerUpStatID.AttackSpeed:
                    this.attackSpeed += val;
                    break;
                case PowerUpStatID.vitality:
                    this.vitality += (int)val;
                    break;
                case PowerUpStatID.manaBonus:
                    this.manaBonus += (int)val;
                    break;
                case PowerUpStatID.shieldBonus:
                    this.shieldBonus += (int)val;
                    break;
                case PowerUpStatID.cooldownSkillMod:
                    this.cooldownSkillMod += val;
                    break;
                default:break;
            }
        }
        
        public float GetStatBaseByID(PowerUpStatID statID)
        {
            switch (statID)
            {
                case PowerUpStatID.vitality:
                    return this.vitality;
                case PowerUpStatID.manaBonus:
                    return this.manaBonus;
                case PowerUpStatID.shieldBonus:
                    return this.shieldBonus;
                case PowerUpStatID.cooldownSkillMod:
                    return this.cooldownSkillMod;
                case PowerUpStatID.AttackSpeed:
                    return this.attackSpeed;
                default:
                    return 0;
            }
        }
    }
}