using System.Collections;
using System.Collections.Generic;
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
    }
}
