using System.Collections;
using System.Collections.Generic;
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

        public CharacterStat Clone()
        {
            return (CharacterStat)this.MemberwiseClone();
        }
    }
}