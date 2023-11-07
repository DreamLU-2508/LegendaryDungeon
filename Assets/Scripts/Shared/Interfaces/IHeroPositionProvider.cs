using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace DreamLU
{
    public interface IHeroPositionProvider
    {
        public WeaponData WeaponData { get; }

        public Vector3 GetTargetPosition();
    }
}
