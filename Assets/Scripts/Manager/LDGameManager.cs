using Cinemachine;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
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

    [DefaultExecutionOrder(-101)]
    public class LDGameManager : MonoBehaviour, IGameStateProvider, IHeroPositionProvider
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

        [SerializeField] private GameConfig _gameConfig;
        [SerializeField] private WeaponDataManifest _weaponDataManifest;

        public static LDGameManager Instance;

        private Character _character;
        private Transform targetTransform;
        private CharacterSkill _characterSkill;
        private CharacterData characterData;

        public GameConfig GameConfig { get { return _gameConfig; } }
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

        public GameStateMachine stateMachine { get { return gameStateMachine; } }

        public CharacterSkill CharacterSkill => _characterSkill;

        public WeaponData WeaponData => _character.WeaponData;

        public CharacterData CharacterData => characterData;

        public LevelsDataManifest LevelsDataManifest => _levelsDataManifest;

        // event
        public event System.Action OnInitializeCharacter;

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
            //CharacterUtilities.GetMouseWorldPosition(_camera);
            if (_gameConfig.target == PlayTarget.Demo)
            {
                //if (Input.GetKeyDown(KeyCode.G) && Input.GetKey(KeyCode.LeftControl))
                //{
                //    if (_dungeonBuilder.GenerateDungeon())
                //    {
                //        // If PlayTarget is Demo, init character
                //        gameStateMachine.ChangeState(StateID.Normal);
                //        if (_character != null)
                //        {
                //            _character.transform.position = _dungeonBuilder.GetPositionRoomEntrance();
                //            SetCameraFollow(targetTransform, 1);
                //        }
                //        else
                //        {
                //            InitializeCharacter(defaultHeroData);
                //        }
                //        _enemyManager.OnStopSpawnEnemy();
                //    }
                //}
                    
            }
            else
            {
                if (_gameConfig.enableCheat)
                {
                    if (Input.GetKeyDown(KeyCode.V))
                    {
                        ProceedToEndLevel();
                    }
                }
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

            // init wepon
            if(_weaponDataManifest.TryGetWeapon(character.weaponID, out var wpData))
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

        public void StartRun(CharacterData characterData)
        {
            gameStateMachine.ChangeState(StateID.Normal);
            InitializeCharacter(characterData);
            levelManager.LoadLevel(1);
        }

        public void ProceedToEndLevel()
        {
            int nextLevel = levelManager.CurrentLevel + 1;

            gameStateMachine.ChangeState(StateID.StageVictory, () =>
            {
                _dungeonBuilder.ClearDungeon();
            });

            // test
            gameStateMachine.ChangeState(StateID.Normal, () =>
            {
                levelManager.LoadLevel(nextLevel);
                targetTransform.position = _dungeonBuilder.GetPositionRoomEntrance();
            });
        }
    }
}
