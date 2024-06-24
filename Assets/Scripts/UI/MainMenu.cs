using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace DreamLU.UI
{
    public class MainMenu : MonoBehaviour
    {
        private IUIMain _uiMain;

        private void Awake()
        {
            _uiMain = CoreLifetimeScope.SharedContainer.Resolve<IUIMain>();
        }

        public void StartGame()
        {
            _uiMain.OnCharacterSelect();
        }

        public void OptionsGame()
        {
            LDGameManager.Instance.ShowUISettings();
        }

        public void ExitGame()
        {
            Debug.LogError("Exit Game");
        }

        public void Show()
        {
            this.gameObject.SetActive(true);
        }

        public void Hide()
        {
            this.gameObject.SetActive(false);
        }
    }
}
