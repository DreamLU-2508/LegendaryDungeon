using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace DreamLU
{
    public class Portal : MonoBehaviour
    {
        private IGameStateProvider _gameStateProvider;
        bool isTele = false;

        private void Awake()
        {
            _gameStateProvider = CoreLifetimeScope.SharedContainer.Resolve<IGameStateProvider>();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            // Debug.LogError("OnTriggerEnter2D");
            var character = collision.GetComponent<Character>();
            if (character != null)
            {
                isTele = true;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            Debug.LogError("OnTriggerExit2D");
            var character = collision.GetComponent<Character>();
            if (character != null)
            {
                isTele = false;
            }
        }

        private void Update()
        {
            if(isTele)
            {
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    _gameStateProvider.ProceedToSelectCard();
                }
            }
        }
    }

}