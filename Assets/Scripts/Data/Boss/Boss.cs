using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Mathematics;
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
    public class Boss : MonoBehaviour, ICaster, ITransformProvider, IDamageProvider, IMovementProvider
    {
        private SpriteRenderer _spriteRenderer;
        private Animator animator;
        private StatsAniamtion statsAniamtion = StatsAniamtion.None;

        private BossData _bossData;
        private bool isDie = true;
        private float moveSpeed;

        private IWeaponProvider weaponProvider;
        private IHeroPositionProvider heroPositionProvider;
        private ICharacterActor characterActor;
        private IEnemyProvider _enemyProvider;
        URandom _random = new URandom();
        private int collisionDamage = 0;
        private bool isMove;
        private bool _isExecutingSkill = false;
        private float timeDelaySkill = 0;
        private bool isSpawning;
        private Tween _tween;
        private InstancedMaterialHelper _helper;

        public BossData Data => _bossData;
        public bool IsDie => isDie;
        public bool IsMove => isMove;
        public bool IsSpawning => isSpawning;

        public float MoveSpeed => moveSpeed;

        private int health = 10;
        
        protected readonly List<SkillBase> skills = new();
        private SkillBase currentSkill = null;
        private float cooldownActiveSkill = 0;
        private ChancefTable<SkillBase> _chancefTable = new ChancefTable<SkillBase>();
        private static readonly int DissolveAmount = Shader.PropertyToID("DissolveAmount");
        private static readonly int Disable = Shader.PropertyToID("Disable");

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
            isDie = false;
            weaponProvider = CoreLifetimeScope.SharedContainer.Resolve<IWeaponProvider>();
            heroPositionProvider = CoreLifetimeScope.SharedContainer.Resolve<IHeroPositionProvider>();
            characterActor = CoreLifetimeScope.SharedContainer.Resolve<ICharacterActor>();
            _enemyProvider = CoreLifetimeScope.SharedContainer.Resolve<IEnemyProvider>();
            _random = URandom.CreateSeeded();
            _helper = new InstancedMaterialHelper(_spriteRenderer);
        }

        void Start()
        {
            ResetAimAnimation();
            SetIdle();
        }

        public void BossSetup(BossData bossData)
        {
            _bossData = bossData;
            isDie = false;
            health = _bossData.stat.maxHealth;
            moveSpeed = _enemyProvider.GetEnemyMoveSpeed(_bossData);
            LoadSkill(_bossData.skills);
            cooldownActiveSkill = _bossData.cooldownActiveSkill;
            collisionDamage = _bossData.stat.collisionDamage;
            isMove = true;
            isSpawning = true;
            if (_helper != null && _helper.InstancedMaterial != null)
            {
                _helper.InstancedMaterial.SetFloat(DissolveAmount, 0);
                KillTween();
                _tween = DOTween.To(() => 0f, (value) =>
                    {
                        // if (_helper.InstancedMaterial.GetInt(Disable) != 0)
                        // {
                        //     _helper.InstancedMaterial.SetInt(Disable, 0);
                        // }

                        _helper.InstancedMaterial.SetFloat(DissolveAmount, value);
                    }, 1f, LDGameManager.Instance.GameConfig.timeSpawnEnemy)
                    .SetEase(LDGameManager.Instance.GameConfig.easeSpawnEnemy);
                _tween.OnComplete(() =>
                {
                    isSpawning = false;
                });
            }
        }
        
        private void LoadSkill(List<SkillExecuteChance> dataList)
        {
            skills.Clear();
            foreach (var skillExecute in dataList)
            {
                var skill = Instantiate(skillExecute.data.prefab, transform);
                skill.name = skillExecute.data.name;
                skill.SetData(skillExecute.data);
                skill.SetCaster(this);
                skills.Add(skill);
                _chancefTable.AddRange(skillExecute.chance, skill);
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
            isDie = true;
            PoolManager.Release(this.gameObject);
            if (skills.Count > 0)
            {
                foreach (var skillBase in skills)
                {
                    skillBase.Stop();
                    skillBase.Shutdown();
                }
            }
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
                characterActor.AddDamage(collisionDamage);
            }
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if(isSpawning) return;
            
            var character = collision.GetComponent<Character>();
            if (character != null)
            {
                characterActor.AddDamage(collisionDamage);
            }
        }

        public int SkillCount => skills.Count;
        public SkillBase GetSkill(int id)
        {
            return skills[math.clamp(id, 0, skills.Count - 1)];
        }

        public object GetExternalProvider(Type providerType)
        {
            return null;
        }

        public bool IsExecutingSkill
        {
            get => _isExecutingSkill;
            set => _isExecutingSkill = value;
        }

        private void Update()
        {
            Debug.LogError(isSpawning);
            if(isDie) return;
            
            if(isSpawning) return;

            // if (timeDelaySkill <= 2f)
            // {
            //     timeDelaySkill += Time.deltaTime;
            //     return;
            // }

            cooldownActiveSkill -= Time.deltaTime;
            
            if (cooldownActiveSkill <= 0 && !_isExecutingSkill)
            {
                if (_chancefTable.CanRoll)
                {
                    currentSkill = _chancefTable.RollWithinMaxRange(_random);
                    currentSkill.Execute();
                    cooldownActiveSkill = _bossData.cooldownActiveSkill;
                }
            }
        }

        public Vector3 Position
        {
            get => transform.position;
            set => transform.position = value;
        }
        public Quaternion Rotation
        {
            get => transform.rotation;
            set => transform.rotation = value;
        }

        public int CollisionDamage
        {
            get => collisionDamage;
            set => collisionDamage = value;
        }

        public void SetCharge(bool stopMove, bool isIdle = false)
        {
            isMove = !stopMove;

            if (isIdle)
            {
                SetIdle();
            }
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