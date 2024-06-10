namespace DreamLU
{
    public enum DifficultyID
    {
        Tier0 = 0,
        Tier1,
        Tier2,
        Tier3,
        Tier4,
        Tier5,
        Tier6,
        Tier7,
        Tier8,
        Tier9,
    }
    
    [System.Serializable]
    public struct DifficultyCharacterData
    {
        /// <summary>
        /// Character Id
        /// </summary>
        public CharacterID characterID;

        /// <summary>
        /// The highest difficulty the character overcomes
        /// </summary>
        public DifficultyID maxDifficulty;
    }
}