using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ≥°æ∞“Ù–ß≈‰÷√£®ScriptableObject£©
[CreateAssetMenu(fileName = "SceneSoundProfile", menuName = "Demo/Scene Sound Profile")]
public class SceneSoundConfig : ScriptableObject
{
    public string sceneName;
    public AudioClip bgm;
    public AudioClip combatbgm_0;
    public AudioClip combatbgm_1;
    public AudioClip combatbgm_2;
    public AudioClip combatbgm_3;
    public AudioClip victory;
    public List<CreatureSoundConfig> creatureConfigs;

    private Dictionary<CreatureType, Dictionary<SoundType, AudioClip[]>> _soundMatrix;

    public AudioClip GetRandomClip(CreatureType cType, SoundType sType)
    {
        InitializeMatrix();

        if (_soundMatrix.TryGetValue(cType, out var typeDict))
        {
            if (typeDict.TryGetValue(sType, out var clips))
            {
                if (clips.Length == 0) return null;
                return clips[Random.Range(0, clips.Length)];
            }
        }
        return null;
    }
    private void InitializeMatrix()
    {
        if (_soundMatrix != null) return;

        _soundMatrix = new Dictionary<CreatureType, Dictionary<SoundType, AudioClip[]>>();

        foreach (var config in creatureConfigs)
        {
            var typeDict = new Dictionary<SoundType, AudioClip[]>();
            foreach (var profile in config.soundProfiles)
            {
                typeDict[profile.soundType] = profile.clips;
            }
            _soundMatrix[config.creatureType] = typeDict;
        }
    }
}
