using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using DreamLU;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class SkillShootAround : SkillBase
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private int numberOfRings = 3;
    [SerializeField] private int objectsPerRing = 10;
    [SerializeField] private float initialRadius = 2f;
    [SerializeField] private float timeExecute;

    [SerializeField] private float speed = 1;
    [SerializeField] private float range = 20;

    private Vector3 sourcePosition;
    private float timeCount = 0;
    private float castPerTime;
    private float castTime;
    private int countRing;
    
    [CasterProvider(false)] protected IMovementProvider _movementProvider;
    [CasterProvider] protected ITransformProvider _transformProvider;
    
    
    protected override void OnInit()
    {

    }
    
    protected override void OnExecute()
    {
        _movementProvider?.SetCharge(true);
        sourcePosition = _transformProvider.Position;
        castPerTime = timeExecute / 3;
        castTime = castPerTime;
        countRing = 1;
        OnFire(countRing);
    }
    
    protected override void OnUpdate(bool isExecuting)
    {
        if (!isExecuting) return;
        
        if (timeCount >= timeExecute)
        {
            Complete();
        }

        timeCount += Time.deltaTime;
        if (timeCount >= castTime)
        {
            countRing += 1;
            castTime += castPerTime;
            OnFire(countRing);
        }
    }
    
    protected override void OnStop(bool isInterrupt)
    {
        _movementProvider?.SetCharge(false);
        timeCount = 0;
        castTime = 0;
        castPerTime = 0;
        countRing = 0;
    }

    private void OnFire(int i)
    {
        float totalAngle = 360f;
        float angleBetweenRings = totalAngle / numberOfRings;
        
        float currentRadius = initialRadius;
        
        float ringAngleOffset = angleBetweenRings * i;

        for (int j = 0; j < objectsPerRing; j++)
        {
            // Tính toán góc cho từng đối tượng trên vòng một cách đều đặn
            float angle = 360f / objectsPerRing * j + ringAngleOffset + angleBetweenRings/2;
            float radian = angle * Mathf.Deg2Rad;

            // Tính toán vị trí trên vòng với bán kính đã được giãn đều
            float x = Mathf.Cos(radian) * currentRadius;
            float y = Mathf.Sin(radian) * currentRadius;
            
            Vector3 spawnPosition = sourcePosition + new Vector3(x, y, 0f);
            Vector3 dir = Vector3.Normalize(spawnPosition - sourcePosition);
            Quaternion spawnRotation = Quaternion.identity;
            
            var ammoGO = PoolManager.GetPool(prefab).RetrieveObject(spawnPosition, spawnRotation, PoolManager.Instance.CurrentTransform);
            var movement = ammoGO.GetComponent<AmmoMovement>();
            if (movement)
            {
                movement.OnActive(true, dir, speed, range, sourcePosition, Vector3.one, _data.damageScaling);
            }
            else
            {
                PoolManager.Release(ammoGO);
            }
        }
    }
}
