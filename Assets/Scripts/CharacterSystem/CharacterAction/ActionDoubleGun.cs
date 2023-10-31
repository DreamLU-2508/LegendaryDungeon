using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace DreamLU
{
    public class ActionDoubleGun : CharacterAction
    {
        [Header("Double Gun Config")]
        //[SerializeField] private float distance = 6f;
        [SerializeField] private float durationTween = 5f;
        [SerializeField] private Ease ease = Ease.Linear;
        //[SerializeField] private float waitDelay = 0.2f;
        //[SerializeField] private CharacterStatScalingConfig statScalingConfig;

        private Animator _animator;
        private Vector3 _dashDirection;
        private Vector3 _targetPosition;
        private Sequence _tween;
        private Character _char;

        [Inject] private ICharacterActor _characterActor;

        private int _colliderLayerMask;


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

            DoAction();
        }

        private void DoAction()
        {
            if (_tween != null)
            {
                _tween.Kill();
                _tween = null;
            }

            _tween = DOTween.Sequence();
            _tween.AppendCallback(() =>
            {
                _char.SetupSecondWeapon();
            });
            _tween.AppendInterval(durationTween);
            _tween.SetEase(ease);
            _tween.OnComplete(() =>
            {
                _tween = null;
                _char.ShutDownSkillDoubleGun();
            });
            Shutdown();
        }

        private void OnDestroy()
        {
            if(_tween != null)
            {
                _tween.Kill();
                _tween = null;
            }
        }

        public override void Shutdown()
        {
            base.Shutdown();
            ReleaseControl();
        }

        private void ReleaseControl()
        {

        }
    }
}
