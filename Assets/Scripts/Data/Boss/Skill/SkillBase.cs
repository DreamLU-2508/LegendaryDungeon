using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using VContainer;

namespace DreamLU
{
    public class SkillBase : MonoBehaviour
    {
        private float _readyTime;
        private bool _isExecuting;
        private bool _hasCaster;
        private bool _shutdown;
        protected ICaster _caster;
        protected SkillData _data;
        private IPauseGame _pauseGame;
        protected IHeroPositionProvider _heroPosition;

        protected URandom _uRandom;

        public bool IsExecuting() => _isExecuting;

        public bool IsCooldown() => _readyTime > Time.time;

        public void RemoveCooldown() => _readyTime = 0;

        public bool CanExecute() => !_shutdown && _hasCaster && !_isExecuting && !IsCooldown() && !IsAnySkillExecuting();
        
        public bool Execute()
        {
            if (!CanExecute()) return false;

            _isExecuting = true;
            _caster.IsExecutingSkill = true;
            OnExecute();

            return true;
        }

        public void Update()
        {
            if (!_shutdown)
                OnUpdate(_isExecuting);
        }

        public void Shutdown()
        {
            _shutdown = true;
            ClearAllCoroutine();
            KillAllTween();
            OnShutdown();
        }

        public void Stop()
        {
            if (!_isExecuting) return;

            _isExecuting = false;
            _caster.IsExecutingSkill = false;
            _readyTime = Time.time + _data.cooldown;

            ClearAllCoroutine();
            KillAllTween();
            OnStop(true);
        }

        protected void Complete()
        {
            if (!_isExecuting) return;

            _isExecuting = false;
            _caster.IsExecutingSkill = false;
            _readyTime = Time.time + _data.cooldown;

            OnStop(false);
        }
        
        public void SetData(SkillData data)
        {
            _data = data;
        }

        public void SetCaster(ICaster newCaster)
        {
            _hasCaster = false;
            if (this.InjectCasterProviderFrom(newCaster) == false)
            {
                Debug.LogWarning($"{newCaster.GetType().Name} can't use skill {GetType().Name}");
                return;
            }

            _caster = newCaster;
            _hasCaster = true;
        }

        private void Awake()
        {
            _uRandom = URandom.CreateSeeded();

            _pauseGame = CoreLifetimeScope.SharedContainer.Resolve<IPauseGame>();
            _heroPosition = CoreLifetimeScope.SharedContainer.Resolve<IHeroPositionProvider>();

            _pauseGame.OnPauseGame += HandlePauseGame;
            _pauseGame.OnUnpauseGame += HandleUnpauseGame;
        }

        private void Start()
        {
            OnInit();
        }

        private void OnDestroy()
        {
            _pauseGame.OnPauseGame -= HandlePauseGame;
            _pauseGame.OnUnpauseGame -= HandleUnpauseGame;
        }

        protected virtual void OnInit()
        {

        }

        protected virtual void OnExecute()
        {

        }

        protected virtual void OnUpdate(bool isExecuting)
        {

        }

        protected virtual void OnPause(bool isPause)
        {

        }
        
        protected virtual void OnStop(bool isInterrupt)
        {
            
        }

        protected virtual void OnShutdown()
        {

        }

        private bool IsAnySkillExecuting()
        {
            for (var i = 0; i < _caster.SkillCount; i++)
                if (_caster.GetSkill(i).IsExecuting())
                    return true;
            return false;
        }

        private void HandlePauseGame()
        {
            //if (!_isExecuting) return;
            OnPause(true);
        }

        private void HandleUnpauseGame()
        {
            //if (!_isExecuting) return;
            OnPause(false);
        }
        
        #region Coroutines Management

        private readonly List<Coroutine> coroutines = new();

        protected IEnumerator ParallelCoroutine(params IEnumerator[] enumerators)
        {
            var tasks = new List<Coroutine>(enumerators.Length);
            foreach (var value in enumerators)
                tasks.Add(StartSafeCoroutine(value));
            foreach (var task in tasks)
                yield return task;
        }
        
        protected Coroutine StartSafeCoroutine(IEnumerator coroutine)
        {
            var co = StartCoroutine(coroutine);
            coroutines.Add(co);
            return co;
        }

        private void ClearAllCoroutine()
        {
            foreach (var co in coroutines)
                if (co != null)
                    StopCoroutine(co);
            coroutines.Clear();
        }

        #endregion
        
        #region Tween Management

        protected readonly List<Tween> tweens = new();
        
        public Tween KillOnStop(Tween tween)
        {
            tweens.Add(tween);
            return tween;
        }
        
        /// Using it carefully, kill sequence does not kill tween added to it/
        /// If you want to stop added tween, you must use KillOnStop on that tween.
        public Sequence CreateSafeSequence()
        {
            var sequence = DOTween.Sequence();
            tweens.Add(sequence);
            return sequence;
        }

        private void KillAllTween()
        {
            foreach (var tween in tweens)
                tween.Kill();
            tweens.Clear();
        }

        #endregion
    }
}
