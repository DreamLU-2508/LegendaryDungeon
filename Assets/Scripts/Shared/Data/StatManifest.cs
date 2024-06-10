using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DreamLU
{

    [CreateAssetMenu(menuName = "Database/Stat")]
    public class StatManifest: ScriptableObject
    {
        public List<ListStat> list;

        public List<StructStatValue> allStat;
    }

    [Serializable]
    public struct StructStatValue
    {
        public string key;
        public string name;
        public bool haveOneDescription;

        public StructStatValue(string _key, string _name)
        {
            this.key = _key;
            this.name = _name;
            this.haveOneDescription = false;
        }
    }

    [Serializable]
    public class ListStat
    {
        public List<StructStatValue> listStat;
    }

}
