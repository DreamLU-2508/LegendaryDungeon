using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace DreamLU
{
    public class EnemyManager : MonoBehaviour, IEnemySpawnProvider, IEnemyProvider
    {
        [SerializeField] private EnemeyDataManifest manifest;
        [SerializeField] private bool isDemo;
        [SerializeField] private float timeDelaySpawn = 0.5f;
        
        [SerializeField, ShowIf("isDemo")] private Vector2 minMaxX;
        [SerializeField, ShowIf("isDemo")] private Vector2 minMaxY;

        [Header("Pool")] [SerializeField] private string nameParentPool;

        [ShowInInspector, ReadOnly]
        List<Enemy> enemies = new List<Enemy>();
        private List<BossData> bossDatasExclude = new List<BossData>();
        private GameObject _bossGameObject;

        private int enemyAmount = 0;
        private bool isSpawnEnemy = false;
        private Vector2Int[] spawnPositionArray;
        private float timeCount  = 0;
        private float spawnTime = 0;
        private Room _currentRoom;
        private Transform parentTransform;

        public event System.Action<int> OnKillEnemy;
        public event System.Action<Room> OnClear;

        private IGameStateProvider _gameStateProvider;

        [ShowInInspector]
        public int EnemyAmount => enemyAmount;

        [ShowInInspector]
        public bool IsSpawnEnemy => isSpawnEnemy;

        [ShowInInspector]
        public Room CurrentRoom => _currentRoom;

        private void Awake()
        {
            _gameStateProvider = CoreLifetimeScope.SharedContainer.Resolve<IGameStateProvider>();
            PoolManager.RegisterParent(nameParentPool, out parentTransform);
        }

        private void Start()
        {
            spawnTime = 0;
            timeCount = 0;
        }

        private void Update()
        {
            timeCount += Time.deltaTime;

            if (!isSpawnEnemy) return;

            if (enemies.Count > 0)
            {
                int count = 0;
                foreach (Enemy enemy in enemies)
                {
                    if (enemy.IsDie)
                    {
                        count += 1;
                    }
                }
                OnKillEnemy?.Invoke(count);
                enemies.RemoveAll(x => x.IsDie);
            }

            if(enemyAmount > 0 && timeCount >= spawnTime && enemies.Count < _gameStateProvider.GameConfig.maxSpawnEnemyInRoom)
            {
                spawnTime += timeDelaySpawn;
                SpawnEnemy();
                enemyAmount -= 1;
            }

            if(enemyAmount == 0)
            {
                OnStopSpawnEnemy();
            }
        }

        public void OnEnterRoom(Room room)
        {
            if(room != null)
            {
                _currentRoom = room;
                if (room.InstancedRoom.EnemyAmount > 0 && !_currentRoom.InstancedRoom.IsClearEnemy)
                {
                    isSpawnEnemy = true;
                    enemyAmount = room.InstancedRoom.EnemyAmount;
                    spawnPositionArray = room.InstancedRoom.SpawnPositionArray;
                }
            }
        }

        private void SpawnEnemy()
        {
            if (_currentRoom == null) return;

            Vector3Int cellPosition = (Vector3Int)spawnPositionArray[Random.Range(0, spawnPositionArray.Length)];
            int id = Random.Range(0, manifest.list.Count);
            
            GameObject newObj = PoolManager.GetPool(manifest.list[id].prefab, parentTransform).RetrieveObject(_currentRoom.Grid.CellToWorld(cellPosition), Quaternion.identity, parentTransform);
            Enemy enemy = newObj.GetComponent<Enemy>();
            enemy.EnemySetup(manifest.list[id]);
            enemies.Add(enemy);
        }

        public void OnStopSpawnEnemy()
        {
            isSpawnEnemy = false;
            enemyAmount = 0;
            spawnPositionArray = null;
            spawnTime = 0;
            timeCount = 0;
            _currentRoom.InstancedRoom.IsClearEnemy = true;
            OnClear?.Invoke(_currentRoom);

            if (enemies.Count > 0)
            {
                foreach(Enemy enemy in enemies)
                {
                    if(enemy != null)
                    {
                        PoolManager.Release(enemy.gameObject);
                    }
                }
                enemies.Clear();
            }
        }

        public float GetEnemyMoveSpeed(EnemeyData enemeyData)
        {
            return enemeyData.stat.moveSpeed; 
        }
        
        public float GetEnemyMoveSpeed(BossData bossData)
        {
            return bossData.stat.moveSpeed; 
        }

        public void SpawnBoss(Vector3 position)
        {
            if (_bossGameObject)
            {
                PoolManager.Release(_bossGameObject);
            }
            
            BossData bossData = manifest.GetBossData(bossDatasExclude);
            if(bossData == null) return;
            // bossDatasExclude.Add(bossData);
            
            _bossGameObject = PoolManager.GetPool(bossData.bossPrefab).RetrieveObject(position, Quaternion.identity, parentTransform);
            Boss boss = _bossGameObject.GetComponent<Boss>();
            boss.BossSetup(bossData);
        }

        public void ClearBoss()
        {
            if (_bossGameObject)
            {
                PoolManager.Release(_bossGameObject);
            }
        }
    }

}