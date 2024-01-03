using DuloGames.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DreamLU
{
    public class UIDisplayHUD : MonoBehaviour
    {
        [SerializeField] private UIProgressBar barHealth;
        [SerializeField] private UIProgressBar manaHealth;

        [Header("Text")]
        [SerializeField] private TextMeshProUGUI textHealth;
        [SerializeField] private TextMeshProUGUI textMana;
        [SerializeField] private TextMeshProUGUI nameCharacter;

        [Header("Image")]
        [SerializeField] private Image icon;


        private CharacterManager characterManager;
        private LDGameManager gameManager;

        private void Awake()
        {
            characterManager = FindObjectOfType<CharacterManager>();
            gameManager = FindObjectOfType<LDGameManager>();

            characterManager.OnInitCharacter += Setup;
            characterManager.OnUpdateHealth += () =>
            {
                SetUIProgressBar(barHealth, (float)characterManager.Health / (float)characterManager.MaxHealth);
                SetText(textHealth, characterManager.MaxHealth, characterManager.Health);
            };

            characterManager.OnUpdateMana += () =>
            {
                SetUIProgressBar(barHealth, (float)characterManager.Mana / (float)characterManager.MaxMana);
                SetText(textHealth, characterManager.MaxMana, characterManager.Mana);
            };

            this.gameObject.SetActive(false);
        }

        private void Setup()
        {
            this.gameObject.SetActive(true);

            SetUIProgressBar(barHealth, 1);
            SetUIProgressBar(manaHealth, 1);

            SetText(textHealth, characterManager.MaxHealth, characterManager.Health);
            SetText(textMana, characterManager.MaxMana, characterManager.Mana);

            nameCharacter.text = gameManager.CharacterData.characterName;
            icon.sprite = gameManager.CharacterData.icon;
        }

        private void SetUIProgressBar(UIProgressBar progressBar, float amount)
        {
            progressBar.fillAmount = amount;
        }

        private void SetText(TextMeshProUGUI textMeshPro, int max, int current)
        {
            textMeshPro.text = $"{current}/{max}";
        }
    }
}