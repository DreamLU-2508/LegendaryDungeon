using Sirenix.OdinInspector;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace DreamLU
{
    public class StatisticsValue : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI nameValue;
        [SerializeField] private TextMeshProUGUI value;

        public void SetName(string name)
        {
            nameValue.text = name;
        }

        public void SetValue(string _value)
        {
            value.text = _value;
        }
    }
}
