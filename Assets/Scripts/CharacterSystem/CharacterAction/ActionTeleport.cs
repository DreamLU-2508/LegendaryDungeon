using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace DreamLU
{
    public class ActionTeleport : CharacterAction
    {
        [Header("Teleport Config")]
        [SerializeField] private float teleportDistance = 6f;
        [SerializeField] private float teleportDuration = 0.25f;
        [SerializeField] private Ease teleportEase = Ease.Linear;
        [SerializeField] private float dashWaitDelay = 0.2f;

        private Animator _animator;
        private Vector3 _teleportDirection;
        private Vector3 _targetPosition;
        private Tween _tween;
        private Character _char;

        [Inject] private ICharacterActor _characterActor;

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
            _char = _characterActor.CharacterTransform.GetComponent<Character>();
        }

        public override void Activate()
        {
            base.Activate();

            DoTeleport();
        }

        private void DoTeleport()
        {
            var tf = _characterActor.CharacterTransform;

            float hor = Input.GetAxisRaw("Horizontal");
            float ver = Input.GetAxisRaw("Vertical");

            _teleportDirection = GetTeleportDirection(hor, ver, out var lookDirection);

            var sourcePos = tf.position;

            _targetPosition = tf.position + (_teleportDirection * teleportDistance);
            _characterActor.IsMovementLocked = true;

            var holdX = false;
            float lastX = 0;
            _tween = DOTween.To(() => 0f, (x =>
            {
                if (!holdX && IsCollide(tf))
                {
                    holdX = true;
                }

                lastX = holdX ? lastX : x;

                //var pos = Vector3.Lerp(sourcePos, _targetPosition, lastX);
                //tf.position = pos;
            }), 1f, teleportDuration).SetEase(teleportEase).OnComplete(() =>
            {
                tf.position = _targetPosition;
                _tween = null;
                Shutdown();
            });
        }

        Vector3 GetTeleportDirection(float hor, float ver, out Direction direction)
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
            //if (Physics2D.Raycast(tf.position, _teleportDirection, 1, _colliderLayerMask))
            //{
            //    return true;
            //}

            //if (Physics.SphereCast(tf.position, 1f, _dashDirection, out var hitInfo2, 1, _colliderLayerMask))
            //{
            //    Debug.Log("Hit sphere " + hitInfo.collider);
            //    return true;
            //}

            return false;
        }
    }
}
