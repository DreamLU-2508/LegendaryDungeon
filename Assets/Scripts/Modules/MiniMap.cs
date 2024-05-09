using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

namespace DreamLU
{
    public class MiniMap : MonoBehaviour
    {
        [SerializeField] private GameObject minimapPlayer;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private CinemachineVirtualCamera _cinemachineVirtualCamera;

        private Transform heroTransform;
        
        public void StartCam(Transform transform, Sprite sprite)
        {
            _cinemachineVirtualCamera.Follow = transform;
            spriteRenderer.sprite = sprite;
            heroTransform = transform;
        }
        
        public void EndCam()
        {
            heroTransform = null;
        }

        private void Update()
        {
            if (heroTransform != null)
            {
                minimapPlayer.transform.position = heroTransform.position;
            }
        }
    }

}