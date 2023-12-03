using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DreamLU
{
    public interface IUIMain
    {
        public void OnCharacterSelect();

        public void SelectCharacter(CharacterData characterData);
    }
}
