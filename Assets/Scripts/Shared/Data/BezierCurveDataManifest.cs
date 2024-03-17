using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace DreamLU
{
    [System.Serializable]
    public enum BezierCurveType
    {
        CurveI,
    }
    
    [System.Serializable]
    public class BezierCurveData
    {
        public BezierCurveType curveType;
        public List<Vector2> angleAxisMinMax;
        public Vector2 distanceP2MinMax;
        public bool isP2Back;
        [ShowIf("isP2Back")] public Vector2 angleBackMinMax;

        public float GetRandomAngleAxis()
        {
            if (this.angleAxisMinMax != null && this.angleAxisMinMax.Count > 0)
            {
                Vector2 rangeAngleAxisMinMax = this.angleAxisMinMax[Random.Range(0, this.angleAxisMinMax.Count)];
                return Random.Range(rangeAngleAxisMinMax.x, rangeAngleAxisMinMax.y);
            }
            return Random.Range(0, 360);
        }
    }

    [CreateAssetMenu(menuName = "Database/BezierCurveDataManifest")]
    public class BezierCurveDataManifest : ScriptableObject
    {
        [TableList]
        public List<BezierCurveData> _bezierCurveDatas;

        public bool TryGetP2P3(BezierCurveType type, Vector3 startPos, Vector3 endPos, out Vector3 _p2, out Vector3 _p3)
        { 
            _p2 = Vector3.zero;
            _p3 = Vector3.zero;

            var bezierCurveData = _bezierCurveDatas.Find((x) => x.curveType == type);
            if (bezierCurveData != null)
            {
                if (bezierCurveData.curveType == BezierCurveType.CurveI)
                {
                    float rangeRange = bezierCurveData.GetRandomAngleAxis();
                    float upDist = Random.Range(bezierCurveData.distanceP2MinMax.x, bezierCurveData.distanceP2MinMax.y);
                    Vector3 direction = Vector3.Normalize(endPos - startPos);
                    var dir = Quaternion.AngleAxis(rangeRange, direction) * Vector3.up.normalized;
                    if (bezierCurveData.isP2Back)
                    {
                        float randomAngleBack = Random.Range(bezierCurveData.angleBackMinMax.x, bezierCurveData.angleBackMinMax.y);
                        float signed = 1f;
                        if(randomAngleBack < 0f)
                        {
                            randomAngleBack *= -1f;
                            signed = -1f;
                        }
                        dir = Vector3.Slerp(dir,-direction * signed,Mathf.Clamp01(randomAngleBack / 90f));
                    }

                    _p2 = startPos + dir * upDist;
                    _p3 = (_p2 + endPos) / 2f;
                }

                return true;
            }

            return false;
        }
    }
}
