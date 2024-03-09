using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.TextCore.Text;
using VContainer;

namespace DreamLU
{
    public enum StatsAniamtion
    {
        None,
        Idle,
        Move,
        Dash
    }

    [RequireComponent(typeof(SortingGroup))]
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(PolygonCollider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    [DisallowMultipleComponent]

    public class Character : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer wpSprite;
        [SerializeField] private Transform weaponShootPosition;
        [SerializeField] private Transform weaponShootSecondPosition;
        [SerializeField] private SpriteRenderer wpSecondSprite;

        private ICharacterActor _characterActor;

        private Animator animator;
        //private Rigidbody2D rigidbody2D;
        private StatsAniamtion statsAniamtion = StatsAniamtion.None;

        private WeaponData _weaponData;
        private bool useSkillDoubleGun;

        public WeaponData WeaponData => _weaponData;

        public bool UseSkillDoubleGun => useSkillDoubleGun;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            //rigidbody2D = GetComponent<Rigidbody2D>();
            _characterActor = CoreLifetimeScope.SharedContainer.Resolve<ICharacterActor>();
        }

        private void Start()
        {
            ResetAimAnimation();
            ResetDashAnimation();
            SetIdle();
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

        public void ResetDashAnimation()
        {
            animator.SetBool(Settings.rollDown, false);
            animator.SetBool(Settings.rollLeft, false);
            animator.SetBool(Settings.rollRight, false);
            animator.SetBool(Settings.rollUp, false);
        }

        public void SetIdle()
        {
            if (statsAniamtion == StatsAniamtion.Dash) return;

            if (statsAniamtion != StatsAniamtion.Idle)
            {
                animator.SetBool(Settings.isMoving, false);
                animator.SetBool(Settings.isIdle, true);
                statsAniamtion = StatsAniamtion.Idle;
            }
            GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        }

        public void SetMovement(Vector2 moveDir)
        {
            if (_characterActor.IsMovementLocked) return;

            if (statsAniamtion != StatsAniamtion.Move)
            {
                animator.SetBool(Settings.isMoving, true);
                animator.SetBool(Settings.isIdle, false);
                ResetDashAnimation();
                statsAniamtion = StatsAniamtion.Move;
            }

            GetComponent<Rigidbody2D>().velocity = moveDir * Settings.baseMoveSpeed;
        }

        public void SetDash(Direction Dir)
        {
            animator.SetBool(Settings.isMoving, false);
            animator.SetBool(Settings.isIdle, false);
            ResetDashAnimation();
            statsAniamtion = StatsAniamtion.Dash;

            switch (Dir)
            {
                case Direction.Left:
                    animator.SetBool(Settings.rollLeft, true);
                    break;
                case Direction.Right:
                    animator.SetBool(Settings.rollRight, true);
                    break;
                case Direction.Up:
                    animator.SetBool(Settings.rollUp, true);
                    break;
                case Direction.Down:
                    animator.SetBool(Settings.rollDown, true);
                    break;
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

        public void SetWeapon(WeaponData weaponData)
        {
            this._weaponData = weaponData;

            wpSprite.sprite = weaponData.itemSprite;
            weaponShootPosition.localPosition = weaponData.weaponShootPosition;
            weaponShootSecondPosition.localPosition = weaponData.weaponShootPosition;
        }

        public void SetupSecondWeapon()
        {
            useSkillDoubleGun = true;
            wpSecondSprite.sprite = _weaponData.itemSprite;
        }

        public void ShutDownSkillDoubleGun()
        {
            useSkillDoubleGun = false;
            wpSecondSprite.sprite = null;
        }

        public Vector3 GetWeaponShootPosition()
        {
            return weaponShootPosition.position;
        }

        public Vector3 GetWeaponSecondShootPosition()
        {
            return weaponShootSecondPosition.position;
        }

        public void SetStatsAniamtion(StatsAniamtion stats)
        {
            statsAniamtion = stats;
        }
    }
}
