using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DreamLU
{
    public class TestDrop : MonoBehaviour
    {
        public Transform transform;
        public Transform newtf;
        public AnimationCurve curve;
        public float minRadius = 2f;
        public float maxRadius = 5f;
        public BezierCurveDataManifest _bezier;
        public BezierCurveType type;

        [Button]
        public void GetPosition()
        {
            Vector2 randomDirection = Random.insideUnitCircle.normalized * Random.Range(minRadius, maxRadius);
            Vector3 randomPosition = transform.position + new Vector3(randomDirection.x, randomDirection.y, 0);
            newtf.position = randomPosition;
            
            if (_bezier.TryGetP2P3(type, transform.position, randomPosition,
                    out var p2, out var p3))
            {
                DOTween.To(() => 0f, x =>
                {
                    newtf.position =
                        CalculateCubicBezierPoint(x, transform.position, p2, p3,
                            randomPosition);
                }, 1, 1);
            }
            else
            {
                newtf.DOMove(randomPosition, 1);
            }
        }
        
        public static float3 CalculateCubicBezierPoint(float t, float3 p0, float3 p1, float3 p2, float3 p3)
        {
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;
            float uuu = uu * u;
            float ttt = tt * t;

            float3 p = uuu * p0;
            p += 3 * uu * t * p1;
            p += 3 * u * tt * p2;
            p += ttt * p3;

            return p;
        }
    }

}