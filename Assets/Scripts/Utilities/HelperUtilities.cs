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
    }
}
