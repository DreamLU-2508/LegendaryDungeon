using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

namespace DreamLU
{
    public static class HelperUtilities
    {
        public static bool ValidateCheckEmtyString(Object thisObject, string fieldName, string stringToCheck)
        {
            if (string.IsNullOrEmpty(stringToCheck))
            {
                Debug.Log($"{fieldName} is empty and must contain a value in object {thisObject.name}");
                return true;
            }

            return false;
        }

        public static List<T> Clone<T>(List<T> listToClone) where T : System.ICloneable
        {
            return listToClone.Select(item => (T)item.Clone()).ToList();
        }

        public static bool IsCorridor(RoomType roomType)
        {
            if (roomType == RoomType.Corridor || roomType == RoomType.CorridorEW || roomType == RoomType.CorridorNS) return true;

            return false;
        }
        
        public static void ClearChildren(Transform tr, bool immediately)
        {
            for (int i = tr.childCount - 1; i >= 0; i--)
            {
                if (immediately)
                {
                    UnityEngine.Object.DestroyImmediate(tr.GetChild(i).gameObject);                
                }
                else
                {
                    UnityEngine.Object.Destroy(tr.GetChild(i).gameObject);
                }
            }
        }
        
        public static Vector3 SpreadedDirectionInArc(Vector3 mainDir, float arc, int dirIndex, int dirAmount)
        {
            if (dirAmount <= 1)
            {
                return mainDir;
            }

            var startDir = Quaternion.Euler(0, 0, -arc / 2f) * mainDir;            
        
            float angleStep = arc / ((float)dirAmount - 1f);
        
            //Debug.Log($"ShootDir for idx {dirAmount} step {angleStep} {angleStep * dirIndex}");

            return Quaternion.Euler(0,0, angleStep * dirIndex) * startDir;
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
        
        public static Vector3 RandomPositionNearby(Vector3 pos, float radius)
        {
            pos.x += UnityEngine.Random.Range(1.0f, radius) * (HelperUtilities.Chance(50) ? 1.0f : -1f);
            pos.z += UnityEngine.Random.Range(1.0f, radius) * (HelperUtilities.Chance(50) ? 1.0f : -1f);
            return pos;
        }
        
        public static Vector3 RandomPositionNearby(Vector3 pos, float radius, float minDist)
        {
            // pos.x += UnityEngine.Random.Range(minDist, radius) * (RGUtility.Chance(50) ? 1.0f : -1f);
            // pos.z += UnityEngine.Random.Range(minDist, radius) * (RGUtility.Chance(50) ? 1.0f : -1f);
            // return pos;
        
            var dir = Quaternion.Euler(0,  UnityEngine.Random.Range(0, 360), 0) * (new Vector3(0,0,1));
            return pos + dir * UnityEngine.Random.Range(minDist, radius);
        }
        
        public static bool Chance(float chancePer100)
        {
            return UnityEngine.Random.Range(0, 100) < chancePer100;
        }
        
        public static Vector3 ToPinnedZ(this Vector3 v)
        {
            return new Vector3(v.x, v.y, 0);
        }
        
        /// <summary>
        /// Convert the linear volume scale to decibels
        /// </summary>
        public static float LinearToDecibels(int linear)
        {
            float linearScaleRange = 20f;

            // formula to convert from the linear scale to the logarithmic decibel scale
            return Mathf.Log10((float)linear / linearScaleRange) * 20f;
        }

    }
}
