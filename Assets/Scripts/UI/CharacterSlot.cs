using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DreamLU.UI
{
    public class CharacterSlot : MonoBehaviour
    {
        [SerializeField] private CharacterData characterData;
        [SerializeField] private Vector2 positionDefault;
        [SerializeField] private RectTransform rectTransform;

        public CharacterData CharacterData => characterData;

        public void RestPostion()
        {
            rectTransform.anchoredPosition = positionDefault;
        }

        public void MovePosition(float x, bool isPositive)
        {
            if (isPositive)
            {
                rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x + x, rectTransform.anchoredPosition.y);
            }
            else
            {
                rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x - x, rectTransform.anchoredPosition.y);
            }
        }
    }
}
