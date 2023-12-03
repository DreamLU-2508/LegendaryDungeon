using UnityEngine;

namespace DreamLU.UI
{
    public class UIMain : MonoBehaviour, IUIMain
    {
        [SerializeField] private MainMenu menu;
        [SerializeField] private CharacterSelect characterSelect;

        private LDGameManager _gameManager;

        private void Awake()
        {
            _gameManager = FindObjectOfType<LDGameManager>();
            _gameManager.stateMachine.onStateEnter += OnStateEnter;
        }

        void OnStateEnter(StateID stateID)
        {
            if(stateID == StateID.GameStart)
            {
                menu.Show();
                characterSelect.Hide();
            }
            else
            {
                menu.Hide();
                characterSelect.Hide();
            }
        }

        public void OnCharacterSelect()
        {
            characterSelect.Show();
            menu.Hide();
        }

        public void SelectCharacter(CharacterData characterData)
        {
            _gameManager.HandleConfirmedSelectCharacter(characterData);
        }
    }

}