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

        private LevelsDataManifest levelsDataManifest;
        private int currentLevel = 0;
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
            _currentLevelData = levelsDataManifest.GetLevelData(currentLevel);

            LoadMap();

            eventLoadLevel?.Invoke();
        }

        private void LoadMap()
        {
            LevelData level = CurrentLevelData;
            _dungeonBuilder.GenerateDungeon(level);
        }

        private void Update()
        {
            
        }
    }
}