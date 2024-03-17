using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace DreamLU
{
    [CreateAssetMenu(fileName = "Room_", menuName = "Database/Map/Room")]
    public class RoomData : ScriptableObject
    {
        public string guid;

        [Space(10)]
        [Header("ROOM PREFAB")]
        public GameObject prefab;

        [ShowInInspector, ReadOnly] public GameObject previousPrefab; // this is used to regenerate the guid if the so is copied and the prefab is changed


        [Space(10)]
        [Header("ROOM CONFIGURATION")]
        public RoomType roomType;

        public Vector2Int lowerBounds;
        public Vector2Int upperBounds;

        [TableList]
        [SerializeField] public List<Doorway> doorwayList;
        public Vector2Int[] spawnPositionArray;

        public int enemyAmount = 0;

        [Header("Chest")] public Vector2Int positionChest;

        public List<Doorway> CloneDoorwayList()
        {
            return this.doorwayList.Select(item => item.Clone()).ToList();
        }


        [ShowIf("@this.roomType == RoomType.BossRoom || this.roomType == RoomType.EndRoom")]
        public Vector2Int positionTele;

        #region Validation

#if UNITY_EDITOR

        // Validate SO fields
        private void OnValidate()
        {
            // Set unique GUID if empty or the prefab changes
            if (guid == "" || previousPrefab != prefab)
            {
                guid = GUID.Generate().ToString();
                previousPrefab = prefab;
                EditorUtility.SetDirty(this);
            }

            //HelperUtilities.ValidateCheckEnumerableValues(this, nameof(doorwayList), doorwayList);

            //// Check spawn positions populated
            //HelperUtilities.ValidateCheckEnumerableValues(this, nameof(spawnPositionArray), spawnPositionArray);
        }

#endif

        #endregion Validation
    }
}