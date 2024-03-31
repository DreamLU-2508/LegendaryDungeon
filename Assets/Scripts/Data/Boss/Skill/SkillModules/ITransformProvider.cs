using UnityEngine;

namespace DreamLU
{
    public interface ITransformProvider
    {
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
    }
}