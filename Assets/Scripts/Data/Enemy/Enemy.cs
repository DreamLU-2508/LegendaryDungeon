using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using VContainer;

namespace DreamLU
{
    [RequireComponent(typeof(SortingGroup))]
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(CircleCollider2D))]
    [RequireComponent(typeof(PolygonCollider2D))]


    [DisallowMultipleComponent]
    public class Enemy : MonoBehaviour
    {
        private Animator animator;
        private SpriteRenderer _spriteRenderer;
        private StatsAniamtion statsAniamtion = StatsAniamtion.None;

        private EnemyData _enemyData;
        private bool isDie = true;
        private bool isSpawning;
        private bool canFire = false;
        private float moveSpeed;
        private Tween _tween;
        private InstancedMaterialHelper _helper;

        private IWeaponProvider weaponProvider;
        private IHeroPositionProvider heroPositionProvider;
        private ICharacterActor characterActor;
        private IEnemyProvider _enemyProvider;

        public EnemyData Data => _enemyData;
        public bool CanFire
        {
            get { return canFire; }
            set { canFire = value; }
        }
        public bool IsDie => isDie;
        public bool IsSpawning => isSpawning;

        public float MoveSpeed => moveSpeed;

        private int health = 10;
        private static readonly int Disable = Shader.PropertyToID("_Disable");
        private static readonly int DissolveAmount = Shader.PropertyToID("_DissolveAmount");

        private void Awake()
        {
            animator = GetComponent<Animator>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            isDie = false;
            weaponProvider = CoreLifetimeScope.SharedContainer.Resolve<IWeaponProvider>();
            heroPositionProvider = CoreLifetimeScope.SharedContainer.Resolve<IHeroPositionProvider>();
            characterActor = CoreLifetimeScope.SharedContainer.Resolve<ICharacterActor>();
            _enemyProvider = CoreLifetimeScope.SharedContainer.Resolve<IEnemyProvider>();
            _helper = new InstancedMaterialHelper(_spriteRenderer);
        }

        void Start()
        {
            ResetAimAnimation();
            SetIdle();
        }

        public void EnemySetup(EnemyData enemyData)
        {
            _enemyData = enemyData;
            isDie = false;
            isSpawning = true;
            health = _enemyData.stat.maxHealth;
            moveSpeed = _enemyProvider.GetEnemyMoveSpeed(_enemyData);
            if (_helper != null && _helper.InstancedMaterial != null)
            {
                _helper.InstancedMaterial.SetFloat(DissolveAmount, 0);
                KillTween();
                _tween = DOTween.To(() => 0f, (value) =>
                    {
                        if (_helper.InstancedMaterial.GetInt(Disable) != 0)
                        {
                            _helper.InstancedMaterial.SetInt(Disable, 0);
                        }

                        _helper.InstancedMaterial.SetFloat(DissolveAmount, value);
                    }, 1f, LDGameManager.Instance.GameConfig.timeSpawnEnemy)
                    .SetEase(LDGameManager.Instance.GameConfig.easeSpawnEnemy);
                _tween.OnComplete(() =>
                {
                    isSpawning = false;
                });
            }
        }

        public void SetIdle()
        {
            if (statsAniamtion != StatsAniamtion.Idle)
            {
                animator.SetBool(Settings.isMoving, false);
                animator.SetBool(Settings.isIdle, true);
                statsAniamtion = StatsAniamtion.Idle;
            }
        }

        public void SetAnimationMovement(Vector3 enemyPosition, Vector3 targetPosition)
        {
            if(isSpawning) return;
            
            if (statsAniamtion != StatsAniamtion.Move)
            {
                animator.SetBool(Settings.isMoving, true);
                animator.SetBool(Settings.isIdle, false);
                statsAniamtion = StatsAniamtion.Move;
            }

            SetAimAnimation(enemyPosition.x < targetPosition.x ? AimDirection.Right : AimDirection.Left);
        }


        public void SetAimAnimation(AimDirection direction)
        {
            ResetAimAnimation();
            switch (direction)
            {
                case AimDirection.Up:
                    animator.SetBool(Settings.aimUp, true);
                    break;

                case AimDirection.UpRight:
                    animator.SetBool(Settings.aimUpRight, true);
                    break;

                case AimDirection.UpLeft:
                    animator.SetBool(Settings.aimUpLeft, true);
                    break;

                case AimDirection.Right:
                    animator.SetBool(Settings.aimRight, true);
                    break;

                case AimDirection.Left:
                    animator.SetBool(Settings.aimLeft, true);
                    break;

                case AimDirection.Down:
                    animator.SetBool(Settings.aimDown, true);
                    break;
            }
        }

        private void ResetAimAnimation()
        {
            animator.SetBool(Settings.aimUp, false);
            animator.SetBool(Settings.aimUpRight, false);
            animator.SetBool(Settings.aimUpLeft, false);
            animator.SetBool(Settings.aimRight, false);
            animator.SetBool(Settings.aimLeft, false);
            animator.SetBool(Settings.aimDown, false);
        }

        public void ShutDown()
        {
            isDie = true;
            PoolManager.Release(this.gameObject);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(isSpawning) return;
            
            var ammo = collision.GetComponent<Ammo>();
            if (ammo != null)
            {
                int damage = weaponProvider.GetWeaponDamage(heroPositionProvider.WeaponData);
                if(damage > 0)
                {
                    health -= damage;
                    if (health <= 0)
                    {
                        ShutDown();
                    }
                }
            }

            var character = collision.GetComponent<Character>();
            if (character != null)
            {
                int damage = _enemyData.stat.collisionDamage;
                characterActor.AddDamage(damage);
            }
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if(isSpawning) return;
            
            var character = collision.GetComponent<Character>();
            if (character != null)
            {
                int damage = _enemyData.stat.collisionDamage;
                characterActor.AddDamage(damage);
            }
        }

        private void OnDestroy()
        {
            KillTween();
            _helper.Dispose();
        }

        void KillTween()
        {
            if (_tween != null)
            {
                _tween.Kill();
                _tween = null;
            }
        }
    }

}