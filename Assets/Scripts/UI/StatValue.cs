
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DreamLU
{
    public class StatValue : MonoBehaviour
    {
        private string ID;
        private string nameStat;
        private object value;
        private int limit;
        private bool haveOneDescription;

        public string _ID => ID;

        public string Name
        {
            get => nameStat;
            set => nameStat = value;
        }
        public object Value => value;

        public bool HasOneDescription => haveOneDescription;

        public void InitStatValue(string _id, object _value, bool _haveOneDescription)
        {
            ID = _id;
            value = _value;

            limit = ObjectToInt(value);
            haveOneDescription = _haveOneDescription;
        }

        public void SetText()
        {
            this.name = _ID;
            
            var txts = this.GetComponentsInChildren<TextMeshProUGUI>();
            if (txts != null)
            {
                txts[0].text = nameStat;
                txts[1].text = ObjectToInt(this.Value).ToString();
            }
        }

        public string FormatString(object objVal, string id)
        {
            if (objVal is float flt)
            {
                return Mathf.RoundToInt(flt).ToString();
            }
            return objVal.ToString();
        }

        public void UpdateValue(float _value)
        {
            //int intValue = ObjectToInt(_value);
            int intValue = Mathf.FloorToInt(_value);
            if (intValue > this.limit)
            {
                UpdateColor(Color.green);
            }
            else if (intValue == this.limit)
            {
                UpdateColor(Color.white);
            }
            else
            {
                UpdateColor(Color.red);
            }
            value = _value;
            SetText();
        }

        void UpdateColor(Color color)
        {
            var txts = this.GetComponentsInChildren<TextMeshProUGUI>();
            if (txts != null && ID != "currentHealth")
            {
                txts[0].color = color;
                txts[1].color = color;
            }
        }

        public void UpdateLimit(float _value)
        {
            // limit = ObjectToInt(_value);
            limit = Mathf.FloorToInt(_value);
        }
        
        public int ObjectToInt(object ob)
        {
            try
            {
                float floatObject = Convert.ToSingle(ob);
                // Debug.LogError(floatObject);
                int intObject = Mathf.RoundToInt(floatObject);

                return intObject;
            }
            catch (Exception e)
            {
                return 0;
            }
        }
    }
}