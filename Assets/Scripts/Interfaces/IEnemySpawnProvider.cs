using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DreamLU
{
    public interface IEnemySpawnProvider
    {
        public Room CurrentRoom { get; }

        public event System.Action<int> OnKillEnemy;

        public event System.Action<Room> OnClear;

        public void SpawnBoss(Vector3 position);
    }
}