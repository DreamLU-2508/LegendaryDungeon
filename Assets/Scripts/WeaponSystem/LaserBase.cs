using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DreamLU
{
    public class LaserBase : MonoBehaviour
    {
        [SerializeField] private LineRenderer _renderer;
        [SerializeField] private GameObject startVfx;
        [SerializeField] private GameObject endVFX;
        [SerializeField] private List<ParticleSystem> _list;

        public void EnableLaser()
        {
            this.gameObject.SetActive(true);
            foreach (var particle in _list)
            {
                particle.Play();
            }
        }
        
        public void DisableLaser()
        {
            this.gameObject.SetActive(false);
            foreach (var particle in _list)
            {
                particle.Stop();
            }
        }

        public void UpdateLaser(Vector3 startPosition, Vector3 endPosition)
        {
            _renderer.SetPosition(0, startPosition);
            startVfx.transform.position = startPosition;
            
            _renderer.SetPosition(1, endPosition);
            endVFX.transform.position = endPosition;

            // Vector2 dir = endPosition - startPosition;
            // RaycastHit2D hit2D = Physics2D.Raycast(startPosition + Vector3.one, dir.normalized, dir.magnitude);
            //
            // ContactFilter2D contactFilter = new ContactFilter2D();
            // contactFilter.useLayerMask = true;
            // contactFilter
            // Debug.LogError(hit2D;
            // if (hit2D )
            // {
            //     _renderer.SetPosition(1, hit2D.point);
            //     endVFX.transform.position = _renderer.GetPosition(1);
            // }
        }
    }
}