using DreamLU.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using VContainer;

namespace DreamLU
{
    public class CharacterSelect : MonoBehaviour
    {
        [SerializeField] private float offsetX = 100f;
        [SerializeField] private List<CharacterSlot> list;
        [SerializeField] private TextMeshProUGUI nameChar;

        private int currentIndex = 0;

        private IUIMain _uiMain;

        private void Awake()
        {
            _uiMain = CoreLifetimeScope.SharedContainer.Resolve<IUIMain>();
        }

        private void Start()
        {
           currentIndex = 0;
           ResetPosition();
           nameChar.text = list[currentIndex].CharacterData.characterName;
        }

        public void Show()
        {
            this.gameObject.SetActive(true);
            ResetPosition();
        }

        public void Hide()
        {
            this.gameObject.SetActive(false);
            ResetPosition();
        }

        public void ResetPosition()
        {
            if (list.Count > 0)
            {
                foreach (CharacterSlot slot in list)
                {
                    slot.RestPostion();
                }
            }

            currentIndex = 0;
            nameChar.text = list[currentIndex].CharacterData.characterName;
        }

        public void Next()
        {
            if(list.Count > 0)
            {
                currentIndex += 1;
                if(currentIndex > list.Count - 1)
                {
                    currentIndex = list.Count - 1;
                    return;
                }

                nameChar.text = list[currentIndex].CharacterData.characterName;
                MoveNext();
            }
        }

        public void Previous()
        {
            if (list.Count > 0)
            {
                currentIndex -= 1;
                if (currentIndex < 0)
                {
                    currentIndex = 0;
                    return;
                }

                nameChar.text = list[currentIndex].CharacterData.characterName;
                MovePrevious();
            }
        }

        public void SelectCharacter()
        {
            if(list.Count > 0)
            {
                _uiMain.SelectCharacter(list[currentIndex].CharacterData);
            }
        }

        private void MoveNext()
        {
            foreach ( CharacterSlot slot in list )
            {
                slot.MovePosition(offsetX, false);
            }
        }

        private void MovePrevious()
        {
            foreach (CharacterSlot slot in list)
            {
                slot.MovePosition(offsetX, true);
            }
        }
    }
}
