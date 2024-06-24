using UnityEngine;

[CreateAssetMenu(fileName = "MusicTrack_", menuName = "Database/Sounds/MusicTrack")]
public class MusicTrackSO : ScriptableObject
{
    public string musicName;
    public AudioClip musicClip;
    [Range(0, 1)]
    public float musicVolume = 1f;
}
