using DuloGames.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace DreamLU
{
    public class UIDisplayHUD : MonoBehaviour
    {
        [SerializeField] private Transform root;
        
        [SerializeField] private UIProgressBar barHealth;
        [SerializeField] private UIProgressBar barShield;
        [SerializeField] private UIProgressBar barMana;

        [Header("Text")]
        [SerializeField] private TextMeshProUGUI textHealth;
        [SerializeField] private TextMeshProUGUI textShield;
        [SerializeField] private TextMeshProUGUI textMana;
        [SerializeField] private TextMeshProUGUI nameCharacter;

        [Header("Image")]
        [SerializeField] private Image icon;


        private CharacterManager characterManager;
        private LDGameManager gameManager;
        private GameStateMachine _gameStateMachine;

        private void Awake()
        {
            characterManager = FindObjectOfType<CharacterManager>();
            gameManager = FindObjectOfType<LDGameManager>();
            _gameStateMachine = GameStateMachine.Instance;

            _gameStateMachine.onStateEnter += OnStateEnter;
            characterManager.OnInitCharacter += Setup;
            characterManager.OnUpdateHealth += () =>
            {
                SetUIProgressBar(barHealth, (float)characterManager.Health / (float)characterManager.MaxHealth);
                SetText(textHealth, characterManager.MaxHealth, characterManager.Health);
            };

            characterManager.OnUpdateShield += () =>
            {
                SetUIProgressBar(barShield, (float)characterManager.Shield / (float)characterManager.MaxShield);
                SetText(textShield, characterManager.MaxShield, characterManager.Shield);
            };
            
            characterManager.OnUpdateMana += () =>
            {
                SetUIProgressBar(barMana, (float)characterManager.Mana / (float)characterManager.MaxMana);
                SetText(textMana, characterManager.MaxMana, characterManager.Mana);
            };

            root.gameObject.SetActive(false);
        }

        private void Setup()
        {
            SetUIProgressBar(barHealth, 1);
            SetUIProgressBar(barShield, 1);

            SetText(textHealth, characterManager.MaxHealth, characterManager.Health);
            SetText(textShield, characterManager.MaxHealth, characterManager.Shield);

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

        public void OnStateEnter(StateID stateID)
        {
            if (stateID == StateID.Normal || stateID == StateID.StageVictory || stateID == StateID.Victory)
            {
                root.gameObject.SetActive(true);
            }
            else
            {
                root.gameObject.SetActive(false);
            }
        }
    }
}