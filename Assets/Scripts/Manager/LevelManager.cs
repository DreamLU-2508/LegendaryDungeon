using System;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace DreamLU
{
    public class LevelManager : MonoBehaviour, ILevelProvider
    {
        public static LevelManager Instance;

        [SerializeField] private EnemeyDataManifest enemeyDataManifest;

        private LevelsDataManifest levelsDataManifest;
        private int currentLevel = 0;
        [ShowInInspector]
        private LevelData _currentLevelData;

        private IDungeonBuilder _dungeonBuilder;

        public LevelData CurrentLevelData => _currentLevelData ? _currentLevelData : levelsDataManifest.GetLevelData(currentLevel);

        [ShowInInspector, ReadOnly]
        public int CurrentLevel
        {
            get => currentLevel;
            set
            {
                currentLevel = value;
            }
        }

        // Event
        public event System.Action eventLoadLevel;

        private void Awake()
        {
            if (Instance == null) { Instance = this; }

            levelsDataManifest = LDGameManager.Instance.LevelsDataManifest;
            _dungeonBuilder = CoreLifetimeScope.SharedContainer.Resolve<IDungeonBuilder>();

            currentLevel = 0;
        }

        [Button]
        public void LoadLevel(int level)
        {
            currentLevel = level;
            LoadMap();
            eventLoadLevel?.Invoke();
        }

        private void LoadMap()
        {
            LevelData level = levelsDataManifest.GetLevelData(currentLevel);;
            _dungeonBuilder.GenerateDungeon(level, (currentLevel, rooms) =>
            {
                _currentLevelData =  GenerateLevelData(currentLevel, rooms);
            });
        }
        
        public LevelData GenerateLevelData(int level, List<Room> rooms)
        {
            var lvl = levelsDataManifest.GetLevelData(level);
            var instLevel = Instantiate(lvl);
            
            Score targetScore = levelsDataManifest.GetLevelScore(level);

            // _thematicController.ApplyTheme(ref mapConfig);
            instLevel.goldDropChanceMultiplier = targetScore.dropChanceMultiplier;

            int roomCombatCount = 0;
            float countSmallRoom = 0;
            float countMediumRoom = 0;
            float countLargeRoom = 0;
            if (rooms != null)
            {
                foreach (var room in rooms)
                {
                    if (room.IsRoomCombat())
                    {
                        roomCombatCount += 1;
                    }

                    if (room.InstancedRoom.RoomType == RoomType.SmallRoom)
                    {
                        countSmallRoom += 1f;
                    }
                    
                    if (room.InstancedRoom.RoomType == RoomType.MediumRoom)
                    {
                        countMediumRoom += 1f;
                    }
                    
                    if (room.InstancedRoom.RoomType == RoomType.LargeRoom)
                    {
                        countLargeRoom += 1f;
                    }
                }
            }
            
            targetScore.score *= Math.Max(1, roomCombatCount);

            float multiplesRoom = levelsDataManifest.multiplesRoom;
            float scoreOneRoom = targetScore.score / (countSmallRoom + multiplesRoom * countMediumRoom +
                                                      Mathf.Pow(multiplesRoom, 2) * countLargeRoom);
            
            // Debug.LogError($"countSmallRoom: {countSmallRoom}");
            // Debug.LogError($"countMediumRoom: {countMediumRoom}");
            // Debug.LogError($"countLargeRoom: {countLargeRoom}");
            instLevel.score = targetScore.score;
            instLevel.scoreRoomSmall = countSmallRoom == 0 ? 0 : scoreOneRoom;
            instLevel.scoreRoomMedium = countMediumRoom == 0 ? 0 : scoreOneRoom * multiplesRoom;
            instLevel.scoreRoomLarge = countLargeRoom == 0 ? 0 : scoreOneRoom * Mathf.Pow(multiplesRoom, 2);
            return instLevel;
        }

        public WaveEnemy GetWaveEnemy(RoomType roomType)
        {
            WaveEnemy waveEnemy = new WaveEnemy();

            var list = enemeyDataManifest.GetListByLevel(_currentLevelData.level);
            foreach (var enemyData in list)
            {
                int total = (int)(SelectScore(roomType, _currentLevelData) / enemyData.CalcScore(new EnemyStatMod()));
                waveEnemy.AddDic(enemyData.enemyID, total);
            }

            waveEnemy.score = SelectScore(roomType, _currentLevelData);
            
            return waveEnemy;

            float SelectScore(RoomType roomType, LevelData levelData)
            {
                if (roomType == RoomType.LargeRoom)
                {
                    return levelData.scoreRoomLarge;
                }
                else if (roomType == RoomType.MediumRoom)
                {
                    return levelData.scoreRoomMedium;
                }
                else if (roomType == RoomType.SmallRoom)
                {
                    return levelData.scoreRoomSmall;
                }

                return 0;
            }
        }

        private void Update()
        {
            
        }
    }
}