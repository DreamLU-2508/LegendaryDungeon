using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DreamLU
{
    public interface IDungeonBuilder
    {
        public bool GenerateDungeon(LevelData levelData, System.Action<int, List<Room>> callback);
    }
}
