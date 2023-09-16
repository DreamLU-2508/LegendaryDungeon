using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace DreamLU
{
    public class NotifyOnDestroy : MonoBehaviour
    {
        public event System.Action<AssetReference, NotifyOnDestroy> OnDestroyed;
        public AssetReference AssetReference { get; set; }

        private void OnDestroy()
        {
            OnDestroyed?.Invoke(AssetReference, this);
        }
    }

}