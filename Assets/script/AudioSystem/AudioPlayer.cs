using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;

// 定义生物类型枚举
public enum CreatureType
{
    Test,
    GardenGhost,
    Megaspore,
    Snake
    //...
}
public enum SoundType
{
    Attack,
    Death,
    Move,
    Idle
}

// 生物音效配对结构
[Serializable]
public struct CreatureSoundConfig
{
    public CreatureType creatureType;
    public SoundProfile[] soundProfiles;
}
[Serializable]
public struct SoundProfile
{
    public SoundType soundType;
    public AudioClip[] clips;
}
public class AudioPlayer : MonoBehaviour
{
    public static AudioPlayer Instance { get; private set; }

    [SerializeField] private AudioSource _bgmSource;
    [SerializeField] private AudioSource _sfxSource;
    [SerializeField] private List<SceneSoundConfig> _sceneProfiles;

    private SceneSoundConfig _currentProfile;

    private string currentSceneName;
    private int currentPhase;
    private int targetPhase;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }
        else
        {
            Destroy(gameObject);
        }
        currentPhase = -1;
        targetPhase = -1;
        _bgmSource.loop = true;
    }
    private void OnSceneUnloaded(Scene scene)
    {
        if (scene.name == currentSceneName)
        {
            _bgmSource.Stop();
            _currentProfile = null;
            currentSceneName = null;
        }
    }

    private void Update()
    {
        HandleBGMTransition();
    }

    private void HandleBGMTransition()
    {
        if (_bgmSource.clip == null || !_bgmSource.isPlaying) return;

        // 当需要切换阶段且当前循环播放结束时
        if (targetPhase != currentPhase)
        {
            if(_bgmSource.time >= _bgmSource.clip.length - 0.1f||targetPhase == 0)
            ExecuteBGMTransition();
        }
    }

    private void ExecuteBGMTransition()
    {
        AudioClip newClip = GetPhaseClip(targetPhase);
        if (newClip == null)
        {
            Debug.LogWarning($"阶段 {targetPhase} 缺少BGM配置");
            return;
        }

        // 执行切换
        currentPhase = targetPhase;
        _bgmSource.clip = newClip;
        _bgmSource.Play();
    }
    private AudioClip GetPhaseClip(int phase)
    {
        if (_currentProfile == null) return null;

        return phase switch
        {
            0 => _currentProfile.combatbgm_0,
            1 => _currentProfile.combatbgm_1,
            2 => _currentProfile.combatbgm_2,
            3 => _currentProfile.combatbgm_3,
            _ => _currentProfile.bgm
        };
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string sceneName = scene.name;
        currentSceneName = sceneName;
        _currentProfile = _sceneProfiles.Find(p => p.sceneName == sceneName);

        if (_currentProfile != null)
        {
            PlayBGM(_currentProfile.bgm);
        }
        else
        {
            Debug.LogWarning($"场景没有音效配置 {sceneName}");
        }
    }

    private void PlayBGM(AudioClip clip)
    {
        if (_bgmSource.clip == clip) return;

        _bgmSource.clip = clip;
        _bgmSource.Play();
    }
    public void SetPhase(int phase)
    {
        if(phase == targetPhase) return;
        targetPhase = phase;
        if(targetPhase ==3)
            _bgmSource.loop = false;
    }
    public void PlayCreatureSound(CreatureType cType, SoundType sType, Vector3 position = default)
    {
        if (_currentProfile == null) return;

        AudioClip clip = _currentProfile.GetRandomClip(cType, sType);
        if (clip == null) return;

        if (position != default) // 3D音效
        {
            AudioSource.PlayClipAtPoint(clip, position);
        }
        else // 2D音效
        {
            _sfxSource.PlayOneShot(clip);
        }
    }
    // 通用音效播放
    public void PlaySFX(AudioClip clip)
    {
        _sfxSource.PlayOneShot(clip);
    }
}