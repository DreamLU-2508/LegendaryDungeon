using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace DreamLU
{
    public class SkillDash : SkillBase
    {
        public Ease dashEase;
        public float dashSpeed;
        [Space]
        public float minDashDistance;
        public float maxDashDistance;
        [Space]
        public float dashTowardTargetChance;
        public float radiusAroundTarget;
        public float minRadiusAroundTarget;
        
        [CasterProvider] protected ITransformProvider _transformProvider;
        // [CasterProvider] protected ITargetProvider _targetProvider;
        [CasterProvider] protected IDamageProvider _damageProvider;
        // [CasterProvider(false)] protected IAnimationProvider _animationProvider;
        [CasterProvider(false)] protected IMovementProvider _movementProvider;
        // [CasterProvider(false)] protected ITrailsProvider _trailsProvider;
        
        private Vector3 direction;
        private int bodyDamage;
        private bool usingAimTargetPosition;
        private Vector3 aimTargetPosition;
        private int _colliderLayerMask;
        
        
        protected override void OnInit()
        {
            // _levelLayoutProvider = CoreLifetimeScope.SharedContainer.Resolve<ILevelLayoutProvider>();
            var layer = LayerMask.NameToLayer("Wall");
            _colliderLayerMask = 1 << layer;
        }
        
        protected override void OnExecute()
        {
            bodyDamage = _damageProvider.CollisionDamage;
            _damageProvider.CollisionDamage = 0;
            _movementProvider?.SetCharge(true);
            
            // Calculate direction
            var isDashTowardTarget = _uRandom.NextFloat(0f, 1f) < dashTowardTargetChance;

            var dashDistance = CalculateDashDistance();
            var startPosition = _transformProvider.Position;
            
            var targetPosition = HelperUtilities.RandomPositionNearby(_transformProvider.Position, dashDistance);
            if (usingAimTargetPosition)
                targetPosition = aimTargetPosition;
            else if (isDashTowardTarget)
                targetPosition = HelperUtilities.RandomPositionNearby(_heroPosition.GetTargetPosition(), radiusAroundTarget, minRadiusAroundTarget);
            
            direction = Vector3.Normalize(targetPosition.ToPinnedZ() - startPosition.ToPinnedZ());
            targetPosition = startPosition + direction * dashDistance;
            
            var speed = dashSpeed;
            var duration = speed != 0 ? dashDistance / speed : 1;
            
            // Setup Animation
            var localDirection = Quaternion.Inverse(_transformProvider.Rotation) * direction;
            _movementProvider?.SetAnimationMovement(targetPosition, startPosition);

            // Setup VFX
            // _trailsProvider?.SetActiveTrails(true);

            // Play Sound
            // if (dashSFX != null)
            //     SoundPlayer.PlayThrottledSFXSound(dashSFX);
            
            // Build Sequence
            var sequence = CreateSafeSequence();
            sequence.Append(DOVirtual.Float(0, 1, duration, t =>
            {
                _transformProvider.Position = Vector3.Lerp(startPosition, targetPosition, t);
            }).SetEase(dashEase));
            sequence.AppendCallback(Complete);
        }
        
        protected override void OnUpdate(bool isExecuting)
        {
            if (!isExecuting) return;
            
            if (IsCollide(_transformProvider.Position))
                Stop();
        }
        
        protected override void OnStop(bool isInterrupt)
        {
            _damageProvider.CollisionDamage = bodyDamage;
            
            _movementProvider?.SetCharge(false);

            usingAimTargetPosition = false;
        }
        
        public void AimTargetForCastRange(Vector2 range)
        {
            var tp = _heroPosition.GetTargetPosition();
            var direct = (_transformProvider.Position - tp).normalized;
            var dist = _uRandom.NextFloat(range.x, range.y);
            AimTarget(tp + direct * dist);
        }
        
        private float CalculateDashDistance()
        {
            return usingAimTargetPosition ? 
                Mathf.Clamp((aimTargetPosition - _transformProvider.Position).magnitude, minDashDistance, maxDashDistance) : 
                _uRandom.NextFloat(minDashDistance, maxDashDistance);
        }
        
        public void AimTarget(Vector3 aimPosition)
        {
            usingAimTargetPosition = true;
            aimTargetPosition = aimPosition;
        }
        
        private bool IsCollide(Vector3 position)
        {
            if (Physics2D.Raycast(position, direction, 1, _colliderLayerMask))
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