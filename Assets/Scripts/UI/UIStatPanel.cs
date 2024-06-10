using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using VContainer;

namespace DreamLU
{
    public class UIStatPanel : MonoBehaviour
    {
        [SerializeField] private Transform contentTransform;
        [SerializeField] private StatValue statValuePrefab;
        [SerializeField] private StatManifest _statManifest;

        protected bool _initialized;
        private IPauseGame _pauseGame;
        private ICharacterManager _characterManager;

        private void Awake()
        {
            Initialization();
        }

        public void ClearContents()
        {
            HelperUtilities.ClearChildren(contentTransform,false);
        }
        
        /// <summary>
        /// Create a new line gameObject for a stat
        /// </summary>
        /// <param name="statValue"></param>
        /// <param name="value"></param>
        public void CreateStatValue(StructStatValue statValue, object value)
        {
            var lineObject = Instantiate(statValuePrefab, this.contentTransform);

            lineObject.InitStatValue(statValue.key, value, statValue.haveOneDescription);
            lineObject.Name = statValue.name;
            lineObject.SetText();
        }

        public GameObject CreateSpace(int height)
        {
            GameObject obj = new GameObject("Spacer", typeof(RectTransform), typeof(CanvasRenderer));
            obj.layer = this.gameObject.layer;
            obj.transform.SetParent(this.contentTransform);
            obj.transform.localScale = new Vector3(1f, 1f, 1f);
            obj.transform.localPosition = Vector3.zero;

            var layoutComp = obj.AddComponent<LayoutElement>();
            layoutComp.minHeight = height;

            // Set the pivot to top left
            (obj.transform as RectTransform).pivot = new Vector2(0f, 1f);

            return obj;
        }

        public virtual void Initialization()
        {
            if (_initialized)
            {
                return;
            }

            _initialized = true;
            _characterManager = CoreLifetimeScope.SharedContainer.Resolve<ICharacterManager>();
            _pauseGame = CoreLifetimeScope.SharedContainer.Resolve<IPauseGame>();

            _pauseGame.OnPauseGame += CreatePanel;
            _pauseGame.OnUnpauseGame += Hide;

            Hide();
        }

        private void OnDestroy()
        {
            _pauseGame.OnPauseGame -= CreatePanel;
            _pauseGame.OnUnpauseGame -= Hide;
        }

        public void CreatePanel()
        {
            ClearContents();

            // We grab default data from characterData instead of a character stat
            Dictionary<string, object> listStat = CharacterStat.GetListStat(_characterManager.CharacterStat);
            var list = _statManifest.list;

            foreach (var smallList in list)
            {
                foreach (var id in smallList.listStat)
                {
                    if (id.key == "maxHealth")
                    {
                        CreateStatValue(id, Mathf.Floor(CharacterManager.Instance.MaxHealth).ToString());
                    }
                    else if (id.key == "maxMana")
                    {
                        CreateStatValue(id, Mathf.Floor(CharacterManager.Instance.MaxMana).ToString());
                    }
                    else if (id.key == "maxShield")
                    {
                        CreateStatValue(id, Mathf.Floor(CharacterManager.Instance.MaxShield).ToString());
                    }
                    else
                    {
                        CreateStatValue(id, listStat[id.key]);
                    }
                }
                CreateSpace(20);
            }
            
            this.gameObject.SetActive(true);
        }

        void Hide()
        {
            ClearContents();
            this.gameObject.SetActive(false);
        }
    }

}