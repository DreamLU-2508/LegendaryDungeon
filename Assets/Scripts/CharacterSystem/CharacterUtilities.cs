using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DreamLU
{
    public static class CharacterUtilities
    {
        public static Vector3 GetMouseWorldPosition(Camera camera)
        {
            if(camera == null) camera = Camera.main;

            Vector3 mousePosition = Input.mousePosition;

            // Clamp
            mousePosition.x = Mathf.Clamp(mousePosition.x, 0, Screen.width);
            mousePosition.y = Mathf.Clamp(mousePosition.y, 0, Screen.height);

            Vector3 mouseWorldPosition = camera.ScreenToWorldPoint(mousePosition);

            mouseWorldPosition.z = 0;

            //Debug.LogError($"{mousePosition} - {mouseWorldPosition}");

            return mouseWorldPosition;
        }

        public static float GetAngleFromVector(Vector3 vector)
        {
            float radians = Mathf.Atan2(vector.y, vector.x);

            float angle = Mathf.Rad2Deg * radians;

            return angle;
        }

        public static AimDirection GetAimDirection(float angle)
        {
            AimDirection aimDirection;

            if(angle >= 22f && angle <= 67f)
            {
                aimDirection = AimDirection.UpRight;
            }
            else if(angle > 67f && angle <= 112f)
            {
                aimDirection = AimDirection.Up;
            }
            else if (angle > 112f && angle <= 158f)
            {
                aimDirection = AimDirection.UpLeft;
            }
            else if ((angle <=180 && angle > 158f) || (angle > -180f && angle <= -135f))
            {
                aimDirection = AimDirection.Left;
            }
            else if (angle > -135f && angle <= -45f)
            {
                aimDirection = AimDirection.Down;
            }
            else if ((angle > -45f && angle <= 0f) || (angle > 0f && angle < 22f))
            {
                aimDirection = AimDirection.Right;
            }
            else
            {
                aimDirection=AimDirection.Right;
            }

            return aimDirection;
        }
    }
}