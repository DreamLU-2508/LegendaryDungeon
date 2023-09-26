using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace DreamLU
{
    public class CoreLifetimeScope : LifetimeScope
    {
        private static CoreLifetimeScope _Instance;

        public static IObjectResolver SharedContainer => _Instance.Container;

        public static CoreLifetimeScope Instance => _Instance;

        protected override void Configure(IContainerBuilder builder)
        {
            Debug.Log("GameLifeTimeScope Configure..");
            _Instance = this;

            CharacterManager characterManager = FindObjectOfType<CharacterManager>();
            LDGameManager gameManager = FindObjectOfType<LDGameManager>();
            DungeonBuilder dungeonBuilder = FindObjectOfType<DungeonBuilder>();

            builder.RegisterInstance(characterManager).AsImplementedInterfaces();
            builder.RegisterInstance(gameManager).AsImplementedInterfaces();
            builder.RegisterInstance(dungeonBuilder).AsImplementedInterfaces();
        }
    }

}