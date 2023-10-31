using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DreamLU
{
    public class CharacterAimController : MonoBehaviour
    {
        [SerializeField] private Transform weaponRotationPointTransform;
        [SerializeField] private Transform weaponSecondRotationPointTransform;

        public void Aim1(AimDirection direction, float aimAngle)
        {
            weaponRotationPointTransform.eulerAngles = new Vector3 (0, 0, aimAngle);

            switch (direction)
            {
                case AimDirection.Left:
                case AimDirection.UpLeft:
                    weaponRotationPointTransform.localScale = new Vector3(1f, -1f, 0f);
                    break;
                case AimDirection.Up:
                case AimDirection.UpRight:
                case AimDirection.Right:
                case AimDirection.Down:
                    weaponRotationPointTransform.localScale = new Vector3(1f, 1f, 0f);
                    break;
            }
        }

        public void Aim2(AimDirection direction, float aimAngle)
        {
            weaponSecondRotationPointTransform.eulerAngles = new Vector3(0, 0, aimAngle);

            switch (direction)
            {
                case AimDirection.Left:
                case AimDirection.UpLeft:
                    weaponSecondRotationPointTransform.localScale = new Vector3(1f, -1f, 0f);
                    break;
                case AimDirection.Up:
                case AimDirection.UpRight:
                case AimDirection.Right:
                case AimDirection.Down:
                    weaponSecondRotationPointTransform.localScale = new Vector3(1f, 1f, 0f);
                    break;
            }
        }
    }
}