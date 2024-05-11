using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DreamLU
{
    public interface ILevelProvider
    {
        public WaveEnemy GetWaveEnemy(RoomType roomType);
    }
}