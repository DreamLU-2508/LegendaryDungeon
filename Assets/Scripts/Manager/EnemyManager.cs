using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace DreamLU
{
    public class EnemyManager : MonoBehaviour, IEnemySpawnProvider
    {
        [SerializeField] private EnemeyDataManifest manifest;
        [SerializeField] private bool isDemo;
        [SerializeField] private float timeDelaySpawn = 0.5f;
        
        [SerializeField, ShowIf("isDemo")] private Vector2 minMaxX;
        [SerializeField, ShowIf("isDemo")] private Vector2 minMaxY;

        [ShowInInspector, ReadOnly]
        List<Enemy> enemies = new List<Enemy>();

        private int enemyAmount = 0;
        private bool isSpawnEnemy = false;
        private Vector2Int[] spawnPositionArray;
        private float timeCount  = 0;
        private float spawnTime = 0;
        private Room _currentRoom;

        public event System.Action<int> OnKillEnemy;

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

            if (isDemo)
            {
                if (Input.GetKeyDown(KeyCode.S) && Input.GetKey(KeyCode.LeftControl))
                {
                    if(spawnPositionArray != null && spawnPositionArray.Length > 0)
                    {
                        SpawnEnemy();
                    }
                }

                if (enemies.Count > 0)
                {
                    int count = 0;
                    foreach (Enemy enemy in enemies)
                    {
                        if (enemy == null)
                        {
                            count += 1;
                        }
                    }
                    OnKillEnemy?.Invoke(count);
                    enemies.RemoveAll(x => x == null);
                }
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
            GameObject newObj = Instantiate(manifest.list[id].prefab);
            Enemy enemy = newObj.GetComponent<Enemy>();
            enemy.transform.position = _currentRoom.Grid.CellToWorld(cellPosition);
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
            
            if(enemies.Count > 0)
            {
                foreach(Enemy enemy in enemies)
                {
                    if(enemy != null)
                    {
                        Destroy(enemy.gameObject);
                    }
                }
                enemies.Clear();
            }
        }
    }

}