using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DreamLU
{
    public class InstancedMaterialHelper 
    {
        private SpriteRenderer _renderer;
        private Material _instancedMaterial;
        public InstancedMaterialHelper(SpriteRenderer renderer)
        {
            _renderer = renderer;
        }

        public Material InstancedMaterial
        {
            get
            {
                if (_instancedMaterial == null)
                {
                    _instancedMaterial = _renderer.material;
                }

                return _instancedMaterial;
            }
        }

        public void Dispose()
        {
            if (_instancedMaterial != null)
            {
                Object.Destroy(_instancedMaterial);
                _instancedMaterial = null;
            }
        }
    }
}
