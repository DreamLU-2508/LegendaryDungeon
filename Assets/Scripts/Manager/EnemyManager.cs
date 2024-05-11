using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace DreamLU
{
    public class WaveEnemy
    {
        public Dictionary<EnemyID, int> numberEnemies = new Dictionary<EnemyID, int>();
        public float score;

        public void AddDic(EnemyID enemyID, int number)
        {
            if (!numberEnemies.ContainsKey(enemyID))
            {
                numberEnemies.Add(enemyID, number);
            }
        }
        
        public EnemyID GetRandomEnemy(URandom random)
        {
            if (numberEnemies.Count <= 0) return EnemyID.None;
            
            ChancefTable<EnemyID> chancefTable = new ChancefTable<EnemyID>();
            foreach (var numberEnemy in numberEnemies)
            {
                chancefTable.AddRange(numberEnemy.Value, numberEnemy.Key);
            }
            
            if (chancefTable.CanRoll)
            {
                var id = chancefTable.RollWithinMaxRange(random);
                if (numberEnemies.ContainsKey(id))
                {
                    numberEnemies[id] -= 1;
                }

                return id;
            }

            return EnemyID.None;
        }
    }

    public class EnemyManager : MonoBehaviour, IEnemySpawnProvider, IEnemyProvider
    {
        [SerializeField] private EnemeyDataManifest manifest;
        [SerializeField] private float timeDelaySpawn = 0.5f;
        [SerializeField] private float maxEnemyInRoom = 3;

        [Header("Pool")] [SerializeField] private string nameParentPool;

        [ShowInInspector, ReadOnly] 
        private List<Enemy> _enemies = new List<Enemy>();
        private List<BossData> bossDatasExclude = new List<BossData>();
        private GameObject _bossGameObject;
        
        private bool isSpawnEnemy = false;
        private Vector2Int[] spawnPositionArray;
        private float timeCount  = 0;
        private Room _currentRoom;
        private Transform parentTransform;
        [ShowInInspector]
        private WaveEnemy _waveEnemy = null;
        [ShowInInspector]
        private float countScore = 0;

        public event System.Action<int> OnKillEnemy;
        public event System.Action<Room> OnClear;

        [ShowInInspector]
        public bool IsSpawnEnemy => isSpawnEnemy;

        [ShowInInspector]
        public Room CurrentRoom => _currentRoom;

        private ILevelProvider _levelProvider;

        private void Awake()
        {
            _levelProvider = CoreLifetimeScope.SharedContainer.Resolve<ILevelProvider>();
            
            PoolManager.RegisterParent(nameParentPool, out parentTransform);
        }

        private void Start()
        {
            timeCount = 0;
            countScore = 0;
        }

        private void Update()
        {
            timeCount += Time.deltaTime;
            
            if (isSpawnEnemy && _enemies.Count < maxEnemyInRoom && _waveEnemy != null)
            {
                if (countScore >= _waveEnemy.score)
                {
                    ClearRoom();
                }
                
                if (timeCount >= timeDelaySpawn)
                {
                    SpawnEnemy();
                }
            }
            
            ClearEnemies();
        }

        public void OnEnterRoom(Room room)
        {
            if(room != null)
            {
                _currentRoom = room;
                if (room.InstancedRoom.EnemyAmount > 0 && !_currentRoom.InstancedRoom.IsClearEnemy)
                {
                    isSpawnEnemy = true;
                    spawnPositionArray = room.InstancedRoom.SpawnPositionArray;
                    _waveEnemy = _levelProvider.GetWaveEnemy(_currentRoom.InstancedRoom.RoomType);
                }
            }
        }

        private void SpawnEnemy()
        {
            if (_currentRoom == null) return;
            
            if (_waveEnemy == null) return;
                
            URandom random = URandom.CreateSeeded();
            EnemyID enemyID = _waveEnemy.GetRandomEnemy(random);

            if (enemyID == EnemyID.None)
            {
                Debug.Log("Spawn Enemy Error");
                return;
            }

            var enemyData = manifest.GetEnemyByID(enemyID);
            if (enemyData == null)
            {
                Debug.Log("Not Found Enemy");
                return;
            }

            Vector3Int cellPosition = (Vector3Int)spawnPositionArray[Random.Range(0, spawnPositionArray.Length)];

            GameObject newObj = PoolManager.GetPool(enemyData.prefab, parentTransform).RetrieveObject(_currentRoom.Grid.CellToWorld(cellPosition), Quaternion.identity, parentTransform);
            Enemy enemy = newObj.GetComponent<Enemy>();
            enemy.EnemySetup(enemyData);
            countScore += enemyData.CalcScore(new EnemyStatMod());
            _enemies.Add(enemy);
        }

        public void ClearRoom()
        {
            isSpawnEnemy = false;
            spawnPositionArray = null;
            timeCount = 0;
            countScore = 0;
            _waveEnemy = null;
            _currentRoom.InstancedRoom.IsClearEnemy = true;
            OnClear?.Invoke(_currentRoom);
        }

        public void ClearEnemies()
        {
            if (_enemies.Count > 0)
            {
                for (int i = _enemies.Count - 1; i >= 0; i--)
                {
                    if (_enemies[i].IsDie)
                    {
                        _enemies.RemoveAt(i);
                    }
                }
            }
        }

        public float GetEnemyMoveSpeed(EnemyData enemyData)
        {
            return enemyData.stat.moveSpeed; 
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

        public void KillAll()
        {
            if (_enemies.Count > 0)
            {
                foreach (var enemy in _enemies)
                {
                    enemy.ShutDown();
                }
            }
        }

        [Header("test Spawn")] 
        public List<EnemyData> testEnemeyDatas;
        private Enemy _enemytest;

        [Button]
        public void TestSpawn()
        {
            if (_currentRoom == null) return;

            Vector3Int cellPosition = (Vector3Int)_currentRoom.InstancedRoom.RoomData.spawnPositionArray[Random.Range(0, _currentRoom.InstancedRoom.RoomData.spawnPositionArray.Length)];
            int id = Random.Range(0, testEnemeyDatas.Count);

            if (_enemytest != null)
            {
                PoolManager.Release(_enemytest.gameObject);
                _enemytest = null;
            }
            
            GameObject newObj = PoolManager.GetPool(testEnemeyDatas[id].prefab, parentTransform).RetrieveObject(_currentRoom.Grid.CellToWorld(cellPosition), Quaternion.identity, parentTransform);
            Enemy enemy = newObj.GetComponent<Enemy>();
            enemy.EnemySetup(testEnemeyDatas[id]);
            _enemytest = enemy;
        }
        
        [Button]
        public WaveEnemy TestGetWave(RoomType roomType)
        {
            return _levelProvider.GetWaveEnemy(roomType);
        }
    }

}