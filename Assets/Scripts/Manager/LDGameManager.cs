using System;
using Cinemachine;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using DreamLU.AStar;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace DreamLU
{
    public struct CharacterSkill
    {
        public CharacterActionID defaultSkill;
        public CharacterActionID ultimateSkill;
    }

    public interface IAggregateDataProvider
    {
        public AggregateData AggregateData { get; set; }

        public void UpdateScore(int score);
        public void UpdateDamageCaused(int damage);
        public void UpdateNumberEnemiesKilled(int numberEnemies);
        public void UpdateWeaponID(ItemID weaponID);
        public void UpdateRuntime(float time);

    }

    [DefaultExecutionOrder(-101)]
    public class LDGameManager : MonoBehaviour, IGameStateProvider, IHeroPositionProvider, IPauseGame, IAggregateDataProvider
    {
        [SerializeField] private CharacterData defaultHeroData;
        [SerializeField] private GameStateMachine gameStateMachine;
        [SerializeField] private List<Transform> autoInitializationList;
        [SerializeField] private CinemachineVirtualCamera _trackingCamera;
        [SerializeField] private Camera _camera;
        [SerializeField] private DungeonBuilder _dungeonBuilder;
        [SerializeField] private EnemyManager _enemyManager;
        [SerializeField] private LevelsDataManifest _levelsDataManifest;
        [SerializeField] private LevelManager levelManager;
        [SerializeField] private CardManager _cardManager;
        [SerializeField] private CharacterManager _characterManager;
        [SerializeField] private MiniMap _miniMap;

        [SerializeField] private GameConfig _gameConfig;
        [SerializeField] private WeaponDataManifest _weaponDataManifest;

        [Header("AStar Test")] 
        [SerializeField] private bool isTestAStar;
        [SerializeField] private AStarTest aStarTest;

        public static LDGameManager Instance;

        private Character _character;
        private Transform targetTransform;
        private CharacterSkill _characterSkill;
        private CharacterData characterData;
        private bool isEndGameRoom;
        private bool _isShowInventory;
        private AggregateData _aggregateData;

        public GameConfig GameConfig { get { return _gameConfig; } }
        [ShowInInspector, ReadOnly]
        public Transform HeroTransform => targetTransform;

        [ShowInInspector, ReadOnly]
        public StateID CurrentMasterGameState 
        {
            get
            {
                if(gameStateMachine != null)
                {
                    //Debug.LogError(gameStateMachine.CurrentState);
                    return gameStateMachine.CurrentState;
                }

                return StateID.None;
            }
        }
        
        public bool IsShowInventory
        {
            get => _isShowInventory;
            set => _isShowInventory = value;
        }

        public AggregateData AggregateData
        {
            get => _aggregateData;
            set => _aggregateData = value;
        }

        public GameStateMachine stateMachine { get { return gameStateMachine; } }

        public CharacterSkill CharacterSkill => _characterSkill;

        public WeaponData WeaponData => _character.WeaponData;

        public CharacterData CharacterData => characterData;

        public LevelsDataManifest LevelsDataManifest => _levelsDataManifest;

        // event
        public event System.Action OnInitializeCharacter;
        public event System.Action OnStartGame;
        public event System.Action OnEndGame;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            // Auto initialize registered objects:
            // Modules that need to be initialized early can be configured here
            if (autoInitializationList != null && autoInitializationList.Count > 0)
            {
                foreach(Transform t in autoInitializationList)
                {
                    foreach(var comp in t.GetComponents<MonoBehaviour>())
                    {
                        if (comp is IInitialization autoInit)
                        {
                            autoInit.Initialization();
                        }
                    }
                }
            }

            if(_camera == null) _camera = Camera.main;
        }

        void Start()
        {
            //if (_gameConfig.target == PlayTarget.Demo)
            //{
            //    if (_dungeonBuilder.GenerateDungeon())
            //    {
            //        // If PlayTarget is Demo, init character
            //        gameStateMachine.ChangeState(StateID.Normal);
            //        InitializeCharacter(defaultHeroData);
            //    }
            //}
        }

        // Update is called once per frame
        void Update()
        {
            // Debug.LogError(CurrentMasterGameState);
            
            if (_gameConfig.enableCheat)
            {
                if (Input.GetKeyDown(KeyCode.V))
                {
                    ProceedToSelectCard();
                }

                if (Input.GetKeyDown(KeyCode.B))
                {
                    levelManager.CurrentLevel = 999;
                    ProceedToSelectCard();
                }
                
                if (Input.GetKeyDown(KeyCode.K))
                {
                    _enemyManager.KillAll();
                }
            }
            
            if (Input.GetKeyDown(KeyCode.Escape) && !IsShowInventory)
            {
                if (gameStateMachine.IsState(StateID.Normal))
                {
                    gameStateMachine.ChangeState(StateID.Pause, () => { PauseGame(); });
                }
                else if (gameStateMachine.IsState(StateID.Pause))
                {
                    UnpauseGame();
                }
            }
            
            if (gameStateMachine.CurrentState == StateID.Normal || gameStateMachine.CurrentState == StateID.SelectCard)
            {
                UpdateRuntime(Time.deltaTime);
            }
        }

        private void InitializeCharacter(CharacterData character)
        {
            CharacterData selectedCharacter = character;
            characterData = selectedCharacter;
            AssetReference reference = selectedCharacter.characterPrefab;
            var handle = reference.InstantiateAsync();
            handle.WaitForCompletion();
            var fxObj = handle.Result;
            var nod = fxObj.AddComponent<NotifyOnDestroy>();
            nod.AssetReference = reference;
            nod.OnDestroyed += (a, n) => { Addressables.ReleaseInstance(n.gameObject); };

            _character = fxObj.GetComponent<Character>();
            targetTransform = _character.transform;
            _character.transform.position = _dungeonBuilder.GetPositionRoomEntrance();
            SetCameraFollow(targetTransform, 0);
            SetVirtualCameraDamping(1, 1, 1);
            _miniMap.StartCam(targetTransform, characterData.icon);

            // init wepon
            if(_weaponDataManifest.TryGetWeapon(character.itemID, out var wpData))
            {
                _character.SetWeapon(wpData);
            }
            else
            {
                Debug.LogError("Set Wp Error");
            }

            // int Skill
            _characterSkill = new CharacterSkill()
            {
                defaultSkill = selectedCharacter.defaultAction,
                ultimateSkill = selectedCharacter.ultimateAction
            };

            _aggregateData = new AggregateData();
            UpdateWeaponID(character.itemID);

            OnInitializeCharacter?.Invoke();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="damping">= 0 for no damping</param>
        public void SetCameraFollow(Transform target, float damping)
        {
            if (_trackingCamera != null)
            {
                _trackingCamera.Follow = target;
            }
            SetVirtualCameraDamping(damping, damping, damping);
        }

        public void SetVirtualCameraDamping(float xDamping, float yDamping, float zDamping)
        {
            CinemachineTransposer cinemachineTransposer = _trackingCamera.GetCinemachineComponent<CinemachineTransposer>();
            if (cinemachineTransposer != null)
            {
                cinemachineTransposer.m_XDamping = xDamping;
                cinemachineTransposer.m_YDamping = yDamping;
                cinemachineTransposer.m_ZDamping = zDamping;
            }
        }

        public Vector3 GetTargetPosition()
        {
            if (_character == null)
            {
                return Vector3.zero;
            }

            return _character.transform.position;
        }

        public Vector3 GetWeaponShootPosition()
        {
            if (_character == null)
            {
                return Vector3.zero;
            }

            return _character.GetWeaponShootPosition();
        }

        public Vector3 GetWeaponSecondShootPosition()
        {
            if (_character == null)
            {
                return Vector3.zero;
            }

            return _character.GetWeaponSecondShootPosition();
        }

        public void StartRun(CharacterData characterData)
        {
            gameStateMachine.ChangeState(StateID.Normal, () =>
            {
                OnStartGame?.Invoke();
            });
            levelManager.LoadLevel(1);
            InitializeCharacter(characterData);
        }

        public void ProceedToEndLevel()
        {
            int nextLevel = levelManager.CurrentLevel + 1;

            // test
            if (nextLevel > _gameConfig.maxLevel)
            {
                gameStateMachine.ChangeState(StateID.StageVictory, () =>
                {
                    OnEndGame?.Invoke();
                    _dungeonBuilder.ClearDungeon();
                    _enemyManager.ClearBoss();
                });  
            }
            else
            {
                gameStateMachine.ChangeState(StateID.Victory, () =>
                {
                    _dungeonBuilder.ClearDungeon();
                    _enemyManager.ClearBoss();
                });
                
                gameStateMachine.ChangeState(StateID.Normal, () =>
                {
                    levelManager.LoadLevel(nextLevel);
                    _characterManager.RebuildStat();
                    targetTransform.position = _dungeonBuilder.GetPositionRoomEntrance();
                });
            }
        }

        public void GotoMainMenu()
        {
            if (isPaused)
            {
                UnpauseGame();
            }
            
            _dungeonBuilder.ClearDungeon();
            PoolManager.Instance.DestroyAllPools();
            if (_character != null)
            {
                Destroy(_character.gameObject);
                _character = null;
            }
            
            // int Skill
            _characterSkill = new CharacterSkill()
            {
                defaultSkill = CharacterActionID.None,
                ultimateSkill = CharacterActionID.None
            };

            gameStateMachine.ChangeState(StateID.GameStart);
        }

        public void OnEnterRoom(Room roomData)
        {
            _enemyManager.OnEnterRoom(roomData);
            if (isTestAStar)
            {
                aStarTest.OnRoomChanged(roomData);
            }
        }

        #region Pause

        private bool isPaused = false;
        public bool IsPausedGame => isPaused;
        
        public event Action OnPauseGame;
        public event Action OnUnpauseGame;

        public void PauseGame()
        {
            Time.timeScale = 0;
            isPaused = true;
            OnPauseGame?.Invoke();
        }

        public void UnpauseGame()
        {
            gameStateMachine.ChangeState(StateID.Normal, () =>
            {
                isPaused = false;
                Time.timeScale = 1;
                OnUnpauseGame?.Invoke();
            });
        }

        #endregion
        
        /// <summary>
        /// Get the mouse world position.
        /// </summary>
        public Vector3 GetMouseWorldPosition()
        {
            if (_camera == null) _camera = Camera.main;

            Vector3 mouseScreenPosition = Input.mousePosition;

            // Clamp mouse position to screen size
            mouseScreenPosition.x = Mathf.Clamp(mouseScreenPosition.x, 0f, Screen.width);
            mouseScreenPosition.y = Mathf.Clamp(mouseScreenPosition.y, 0f, Screen.height);

            Vector3 worldPosition = _camera.ScreenToWorldPoint(mouseScreenPosition);

            worldPosition.z = 0f;

            return worldPosition;
        }

        public void ProceedToSelectCard()
        {
            int nextLevel = levelManager.CurrentLevel + 1;

            // test
            if (nextLevel > _gameConfig.maxLevel)
            {
                if (isEndGameRoom)
                {
                    gameStateMachine.ChangeState(StateID.StageVictory, () =>
                    {
                        OnEndGame?.Invoke();
                        isEndGameRoom = false;
                        _enemyManager.ClearBoss();
                        DataManager.Instance.SaveAggregateData(_aggregateData);
                    });  
                }
                else
                {
                    isEndGameRoom = true;
                    
                    gameStateMachine.ChangeState(StateID.Victory, () =>
                    {
                        _dungeonBuilder.ClearDungeon();
                        _enemyManager.ClearBoss();
                    });
                
                    gameStateMachine.ChangeState(StateID.Normal, () =>
                    {
                        _dungeonBuilder.GenerateEndGameRoom();
                        _characterManager.RebuildStat();
                        targetTransform.position = _dungeonBuilder.GetPositionSpawnEndGameRoom();
                    });
                }
            }
            else
            {
                if (!_cardManager.CanSelectCard)
                {
                    ProceedToEndLevel();
                    PoolManager.Instance.DestroyAllPools();
                    return;
                }
                
                gameStateMachine.ChangeState(StateID.SelectCard, () =>
                {
                    _dungeonBuilder.ClearDungeon();
                });
            }
            
            PoolManager.Instance.DestroyAllPools();
        }

        #region AggregateData

        public void UpdateScore(int score)
        {
            _aggregateData.score += score;
        }
        
        public void UpdateDamageCaused(int damage)
        {
            _aggregateData.damageCaused += damage;
        }
        
        public void UpdateNumberEnemiesKilled(int numberEnemies)
        {
            _aggregateData.numberEnemiesKilled += numberEnemies;
        }
        
        public void UpdateWeaponID(ItemID weaponID)
        {
            _aggregateData.weaponID = weaponID;
        }
        
        public void UpdateRuntime(float time)
        {
            _aggregateData.time += time;
        }

        #endregion
    }
}
