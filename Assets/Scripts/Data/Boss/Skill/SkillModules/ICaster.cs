namespace DreamLU
{
    public interface ICaster
    {
        public int SkillCount { get; }
        public SkillBase GetSkill(int id);
        public object GetExternalProvider(System.Type providerType);
        
        public bool IsExecutingSkill { get; set; }
    }
}