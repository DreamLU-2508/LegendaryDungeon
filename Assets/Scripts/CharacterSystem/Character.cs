using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.TextCore.Text;

namespace DreamLU
{
    public enum StatsAniamtion
    {
        None,
        Idle,
        Move
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

        private Animator animator;
        private Rigidbody2D rigidbody2D;
        private StatsAniamtion statsAniamtion = StatsAniamtion.None;

        private WeaponData _weaponData;

        public WeaponData WeaponData => _weaponData;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            rigidbody2D = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            ResetAimAnimation();
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

        public void SetIdle()
        {
            if(statsAniamtion != StatsAniamtion.Idle)
            {
                animator.SetBool(Settings.isMoving, false);
                animator.SetBool(Settings.isIdle, true);
                statsAniamtion = StatsAniamtion.Idle;
            }
            rigidbody2D.velocity = Vector3.zero;
        }

        public void SetMovement(Vector2 moveDir)
        {
            if (statsAniamtion != StatsAniamtion.Move)
            {
                animator.SetBool(Settings.isMoving, true);
                animator.SetBool(Settings.isIdle, false);
                statsAniamtion = StatsAniamtion.Move;
            }

            rigidbody2D.velocity = moveDir * Settings.baseMoveSpeed * Time.deltaTime;
            //rigidbody2D.velocity = moveDir * Settings.baseMoveSpeed;
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

            wpSprite.sprite = weaponData.weaponSprite;
            weaponShootPosition.localPosition = weaponData.weaponShootPosition;
        }

        public Vector3 GetWeaponShootPosition()
        {
            return weaponShootPosition.position;
        }
    }
}
