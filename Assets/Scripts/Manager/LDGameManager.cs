using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace DreamLU
{
    [DefaultExecutionOrder(-101)]
    public class LDGameManager : MonoBehaviour, IGameStateProvider
    {
        [SerializeField] private CharacterData defaultHeroData;
        [SerializeField] private GameStateMachine gameStateMachine;
        [SerializeField] private List<Transform> autoInitializationList;
        [SerializeField] private CinemachineVirtualCamera _trackingCamera;
        [SerializeField] private Camera _camera;

        [SerializeField] private GameConfig _gameConfig;
        [SerializeField] private WeaponDataManifest _weaponDataManifest;

        public static LDGameManager Instance;

        private Character _character;
        private Transform targetTransform;

        public GameConfig GameConfig { get { return _gameConfig; } }

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
            if (_gameConfig.target == PlayTarget.Demo)
            {
                // If PlayTarget is Demo, init character
                gameStateMachine.ChangeState(StateID.Normal);
                InitializeCharacter();
            }
        }

        // Update is called once per frame
        void Update()
        {
            //CharacterUtilities.GetMouseWorldPosition(_camera);
        }

        void InitializeCharacter()
        {
            CharacterData selectedCharacter = defaultHeroData;
            AssetReference reference = selectedCharacter.characterPrefab;
            var handle = reference.InstantiateAsync();
            handle.WaitForCompletion();
            var fxObj = handle.Result;
            var nod = fxObj.AddComponent<NotifyOnDestroy>();
            nod.AssetReference = reference;
            nod.OnDestroyed += (a, n) => { Addressables.ReleaseInstance(n.gameObject); };

            _character = fxObj.GetComponent<Character>();
            targetTransform = _character.transform;
            SetCameraFollow(targetTransform, 0);
            SetVirtualCameraDamping(1, 1, 1);

            // init wepon
            if(_weaponDataManifest.TryGetWeapon(defaultHeroData.weaponID, out var wpData))
            {
                _character.SetWeapon(wpData);
            }
            else
            {
                Debug.LogError("Set Wp Error");
            }
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
    }
}
