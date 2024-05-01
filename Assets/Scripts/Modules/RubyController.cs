using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using VContainer;

namespace DreamLU
{
    public class RubyController : MonoBehaviour
    {
        [SerializeField] private Transform _transformRuby;
        [SerializeField] private AnimationCurve _animationCurve;
        [SerializeField] private float dynamicSpeed = 1;
        [SerializeField] private float dynamicOffset = 3;

        [Header("Tween")] 
        [SerializeField] private Vector3 positionTarget;
        [SerializeField] private float durationTween;
        [SerializeField] private Ease easeTween;
        [SerializeField] private float durationScale;
        [SerializeField] private Ease easeScale;

        private float timeCount;
        private bool isTriggerEnter = false;
        
        private IGameStateProvider _gameStateProvider;

        private void Awake()
        {
            isTriggerEnter = false;
            _transformRuby.localPosition = Vector3.zero;
            _transformRuby.localScale = Vector3.one;
            
            _gameStateProvider = CoreLifetimeScope.SharedContainer.Resolve<IGameStateProvider>();
        }

        private void Update()
        {
            RubyDynamic();
        }

        void RubyDynamic()
        {
            if(isTriggerEnter) return;
            
            timeCount += Time.deltaTime * dynamicSpeed;
            var value= _animationCurve.Evaluate(timeCount);
            _transformRuby.localPosition = new Vector3(0, value * dynamicOffset, 0);

            if (timeCount >= 1)
            {
                timeCount = 0;
            }
        }

        private Sequence _sequence = null;
        private void OnTriggerEnter2D(Collider2D col)
        {
            if(isTriggerEnter) return;

            isTriggerEnter = true;
            KillTween();

            _sequence = DOTween.Sequence();
            _sequence.Append(_transformRuby.DOLocalMove(positionTarget, durationTween).SetEase(easeTween));
            _sequence.Append(_transformRuby.DOScale(0, durationScale).SetEase(easeScale));
            _sequence.OnComplete(() =>
            {
                _gameStateProvider.ProceedToSelectCard();
            });
        }

        private void KillTween()
        {
            if (_sequence != null)
            {
                _sequence.Kill();
                _sequence = null;
            }   
        }

        private void OnDestroy()
        {
            KillTween();
        }
    }
}