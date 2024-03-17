using DreamLU.UI;
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
            EnemyManager enemyManager = FindObjectOfType<EnemyManager>();
            UIMain uiMain = FindObjectOfType<UIMain>();
            LevelManager levelManager = FindObjectOfType<LevelManager>();
            CastingManager castingManager = FindObjectOfType<CastingManager>();
            CardManager cardManager = FindObjectOfType<CardManager>();
            DropItemHandle dropItemHandle = FindObjectOfType<DropItemHandle>();

            builder.RegisterInstance(characterManager).AsImplementedInterfaces();
            builder.RegisterInstance(gameManager).AsImplementedInterfaces();
            builder.RegisterInstance(dungeonBuilder).AsImplementedInterfaces();
            builder.RegisterInstance(enemyManager).AsImplementedInterfaces();
            builder.RegisterInstance(uiMain).AsImplementedInterfaces();
            builder.RegisterInstance(levelManager).AsImplementedInterfaces();
            builder.RegisterInstance(castingManager).AsImplementedInterfaces();
            builder.RegisterInstance(cardManager).AsImplementedInterfaces();
            builder.RegisterInstance(dropItemHandle).AsImplementedInterfaces();
        }
    }

}