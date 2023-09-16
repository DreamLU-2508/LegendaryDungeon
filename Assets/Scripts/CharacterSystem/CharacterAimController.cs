using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DreamLU
{
    public class CharacterAimController : MonoBehaviour
    {
        [SerializeField] private Transform weaponRotationPointTransform;

        public void Aim(AimDirection direction, float aimAngle)
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
    }
}