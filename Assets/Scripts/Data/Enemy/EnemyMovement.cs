using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace DreamLU
{
    public class EnemyMovement : MonoBehaviour
    {
        private Enemy enemy;
        private bool isFollowCharacter = false;

        private IHeroPositionProvider _positionProvider;
        private IGameStateProvider _gameStateProvider;

        private void Awake()
        {
            enemy = GetComponent<Enemy>();
            _positionProvider = CoreLifetimeScope.SharedContainer.Resolve<IHeroPositionProvider>();
            _gameStateProvider = CoreLifetimeScope.SharedContainer.Resolve<IGameStateProvider>();
        }

        private void Update()
        {
            if (enemy.IsDie) return;

            float distance = Vector3.Distance(this.transform.position, _positionProvider.GetTargetPosition());
            if (!isFollowCharacter && distance > _gameStateProvider.GameConfig.distanceFollowCharacter)
            {
                enemy.CanFire = false;
                return;
            }

            if(!isFollowCharacter) isFollowCharacter = true;
            enemy.CanFire = true;
            this.transform.position = Vector3.MoveTowards(this.transform.position, _positionProvider.GetTargetPosition(), enemy.Data.stat.moveSpeed * Time.deltaTime);
            enemy.SetAnimationMovement(this.transform.position, _positionProvider.GetTargetPosition());
        }

        private void OnEnable()
        {
            isFollowCharacter = false;
        }
    }

}