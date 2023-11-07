using System.Collections;
using System.Collections.Generic;
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
        private StatsAniamtion statsAniamtion = StatsAniamtion.None;

        private EnemeyData _enemeyData;
        private bool isDie = true;
        private bool canFire = false;

        private IWeaponProvider weaponProvider;
        private IHeroPositionProvider heroPositionProvider;
        private ICharacterActor characterActor;

        public EnemeyData Data => _enemeyData;
        public bool CanFire
        {
            get { return canFire; }
            set { canFire = value; }
        }
        public bool IsDie => isDie;

        private int health = 10;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            isDie = false;
            weaponProvider = CoreLifetimeScope.SharedContainer.Resolve<IWeaponProvider>();
            heroPositionProvider = CoreLifetimeScope.SharedContainer.Resolve<IHeroPositionProvider>();
            characterActor = CoreLifetimeScope.SharedContainer.Resolve<ICharacterActor>();
        }

        void Start()
        {
            ResetAimAnimation();
            SetIdle();
        }

        public void EnemySetup(EnemeyData enemyData)
        {
            _enemeyData = enemyData;
            isDie = false;
            health = _enemeyData.enemyMaxHeath;
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
            if (statsAniamtion != StatsAniamtion.Move)
            {
                animator.SetBool(Settings.isMoving, true);
                animator.SetBool(Settings.isIdle, false);
                statsAniamtion = StatsAniamtion.Move;
            }

            if(enemyPosition.x < targetPosition.x)
            {
                SetAimAnimation(AimDirection.Right);
            }
            else
            {
                SetAimAnimation(AimDirection.Left);
            }
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
            this.gameObject.SetActive(false);
            isDie = true;
            Destroy(this.gameObject);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
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

            //var character = collision.GetComponent<Character>();
            //if (character != null)
            //{
            //    int damage = _enemeyData.collisionDamage;
            //    characterActor.AddDamage(damage);
            //}
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            var character = collision.GetComponent<Character>();
            if (character != null)
            {
                int damage = _enemeyData.collisionDamage;
                characterActor.AddDamage(damage);
            }
        }
    }

}