using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DreamLU
{
    public class CharacterManager : MonoBehaviour, ICharacterActor
    {
        private LDGameManager gameManager;

        private void Awake()
        {
            gameManager = FindObjectOfType<LDGameManager>();

            Debug.Log("CharacterManager");
        }
    }

}