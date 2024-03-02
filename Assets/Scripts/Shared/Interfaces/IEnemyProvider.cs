using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DreamLU
{
    public interface IEnemyProvider
    {
        public float GetEnemyMoveSpeed(EnemeyData enemeyData);
    }
}