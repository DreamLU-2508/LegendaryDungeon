using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;
using VContainer;

namespace DreamLU
{
    public class ActionDash : CharacterAction
    {
        [Header("Dash Config")]
        [SerializeField] private float dashDistance = 6f;
        [SerializeField] private float dashDuration = 0.25f;
        [SerializeField] private Ease dashEase = Ease.Linear;
        [SerializeField] private float dashWaitDelay = 0.2f;
        //[SerializeField] private CharacterStatScalingConfig statScalingConfig;

        private Animator _animator;
        private Vector3 _dashDirection;
        private Vector3 _targetPosition;
        private Tween _tween;
        //private CharacterController _characterController;
        private Character _char;

        [Inject] private ICharacterActor _characterActor;

        private int _colliderLayerMask;

        // Index i = cooldown of dash i
        //private List<DashCooldownInfo> _cooldownTimes = new();

        private void Awake()
        {
            var layer = LayerMask.NameToLayer("Wall");
            _colliderLayerMask = 1 << layer;
        }

        public override bool IsCompleted()
        {
            return _isInAction == false;
        }

        public override void Setup()
        {
            base.Setup();

            Reload();
        }

        public override void Reload()
        {
            _animator = _characterActor.CharacterAnimator;
            //_characterController = _characterActor.CharacterTransform.GetComponent<CharacterController>();
            _char = _characterActor.CharacterTransform.GetComponent<Character>();
        }

        public override void Activate()
        {
            base.Activate();

            DoDash();
            
        }

        private void DoDash()
        {
            var tf = _characterActor.CharacterTransform;

            float hor = Input.GetAxisRaw("Horizontal");
            float ver = Input.GetAxisRaw("Vertical");

            _dashDirection = GetDashDirection(hor, ver, out var lookDirection);

            var sourcePos = tf.position;

            _targetPosition = tf.position + (_dashDirection * dashDistance);
            _characterActor.IsMovementLocked = true;
            _char.SetDash(lookDirection);

            var holdX = false;
            float lastX = 0;
            _tween = DOTween.To(() => 0f, (x =>
            {
                if (!holdX && IsCollide(tf))
                {
                    holdX = true;
                }

                lastX = holdX ? lastX : x;

                var pos = Vector3.Lerp(sourcePos, _targetPosition, lastX);
                tf.position = pos;
            }), 1f, dashDuration).SetEase(dashEase).OnComplete(() =>
            {
                _tween = null;
                Shutdown();
            });
        }

        Vector3 GetDashDirection(float hor, float ver, out Direction direction)
        {
            direction = Direction.Up;

            if (hor != 0 || ver != 0)
            {
                Remap(ref hor);
                Remap(ref ver);
                var dir = new Vector3(hor, ver, 0).normalized;
                if (ver > 0)
                {
                    direction = Direction.Up;
                }
                else if (ver < 0)
                {
                    direction = Direction.Down;
                }
                else if (hor < 0)
                {
                    direction = Direction.Left;
                }
                else if (hor > 0)
                {
                    direction = Direction.Right;
                }

                return dir;
            }

            // Fallback, using transform's forward
            var tf = _characterActor.CharacterTransform;
            return tf.up;
        }

        static void Remap(ref float value)
        {
            if (value > 0f) value = 1f;
            else if (value < 0f) value = -1f;
        }

        public override void Shutdown()
        {
            base.Shutdown();
            ReleaseControl();
        }

        private void ReleaseControl()
        {
            _char.SetStatsAniamtion(StatsAniamtion.None);
            _char.ResetDashAnimation();
            _char.SetIdle();
            _animator.SetBool(Settings.isMoving, IsInMovement());
            _characterActor.IsMovementLocked = false;

            bool IsInMovement()
            {
                float hor = Input.GetAxisRaw("Horizontal");
                float ver = Input.GetAxisRaw("Vertical");
                return hor != 0 || ver != 0;
            }
        }

        private bool IsCollide(Transform tf)
        {
            if (Physics2D.Raycast(tf.position, _dashDirection, 1, _colliderLayerMask))
            {
                return true;
            }

            //if (Physics.SphereCast(tf.position, 1f, _dashDirection, out var hitInfo2, 1, _colliderLayerMask))
            //{
            //    Debug.Log("Hit sphere " + hitInfo.collider);
            //    return true;
            //}

            return false;
        }
    }
}