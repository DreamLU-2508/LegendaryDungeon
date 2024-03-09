using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Serialization;

namespace DreamLU
{
    [CreateAssetMenu(fileName = "CharacterData", menuName ="Database/CharacterSystem/CharacterData")]
    public class CharacterData : ScriptableObject
    {
        public CharacterID characterID;
        public string characterName;
        public string description;

        public CharacterStat characterStat;

        public Sprite icon;
        public Sprite handSprite;
        public AssetReference characterPrefab;

        [FormerlySerializedAs("weaponID")] [Header("Starting Weapon")]
        public ItemID itemID;

        [Header("Skill")]
        public CharacterActionID defaultAction;
        public CharacterActionID ultimateAction;

        [Header("Card Slot")] public int cardSlot = 3;
    }

}