using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DreamLU
{
    public struct AggregateData
    {
        public int score;
        public int damageCaused;
        public int numberEnemiesKilled;
        public ItemID weaponID;
        public float time;
    }
    
    public struct WeaponLock
    {
        public ItemID weaponID;
        public bool isLock;
    }
}
