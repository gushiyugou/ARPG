using System.Collections.Generic;
using UnityEngine;

// 音效管理器单例类
public class SoundManager : MonoBehaviour
{
    private static SoundManager _instance;
    public static SoundManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject soundManagerObject = new GameObject("SoundManager");
                _instance = soundManagerObject.AddComponent<SoundManager>();
                DontDestroyOnLoad(soundManagerObject);
            }
            return _instance;
        }
    }

    [System.Serializable]
    public class SoundClip
    {
        public string name;
        public AudioClip clip;
        [Range(0f, 1f)]
        public float volume = 1f;
        [Range(0.5f, 2f)]
        public float pitch = 1f;
        public bool loop = false;
    }

    [SerializeField]
    private List<SoundClip> soundClips = new List<SoundClip>();

    private Dictionary<string, AudioSource> audioSourcePool = new Dictionary<string, AudioSource>();
    private Dictionary<string, SoundClip> soundClipDictionary = new Dictionary<string, SoundClip>();

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
        Debug.Log("开始初始化音效字典");

        // 将音效列表转换为字典以便快速查找
        foreach (var soundClip in soundClips)
        {
            if(soundClip == null)
            {
                Debug.LogWarning("音效列表中存在空元素");
                continue;
            }
            
            if (!soundClipDictionary.ContainsKey(soundClip.name))
            {
                soundClipDictionary.Add(soundClip.name, soundClip);
            }
            else
            {
                Debug.LogWarning($"音效名称重复: {soundClip.name}");
            }
        }
    }

    
    // 播放音效
    public void PlaySound(string soundName)
    {
        if (soundClipDictionary.ContainsKey(soundName))
        {
            SoundClip soundClip = soundClipDictionary[soundName];
            AudioSource audioSource = GetAvailableAudioSource(soundName);

            audioSource.clip = soundClip.clip;
            audioSource.volume = soundClip.volume;
            audioSource.pitch = soundClip.pitch;
            audioSource.loop = soundClip.loop;

            audioSource.Play();
        }
        else
        {
            Debug.LogWarning($"未找到音效: {soundName}");
        }
    }

    // 停止播放音效
    public void StopSound(string soundName)
    {
        if (audioSourcePool.ContainsKey(soundName))
        {
            AudioSource audioSource = audioSourcePool[soundName];
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }

    // 暂停播放音效
    public void PauseSound(string soundName)
    {
        if (audioSourcePool.ContainsKey(soundName))
        {
            AudioSource audioSource = audioSourcePool[soundName];
            if (audioSource.isPlaying)
            {
                audioSource.Pause();
            }
        }
    }

    // 恢复播放音效
    public void ResumeSound(string soundName)
    {
        if (audioSourcePool.ContainsKey(soundName))
        {
            AudioSource audioSource = audioSourcePool[soundName];
            if (!audioSource.isPlaying)
            {
                audioSource.UnPause();
            }
        }
    }

    // 获取可用的AudioSource
    private AudioSource GetAvailableAudioSource(string soundName)
    {
        if (audioSourcePool.ContainsKey(soundName))
        {
            AudioSource existingSource = audioSourcePool[soundName];
            //if (!existingSource.isPlaying)
            //{
            //    return existingSource;
            //}
            return existingSource;

        }

        // 创建新的AudioSource
        AudioSource newAudioSource = gameObject.AddComponent<AudioSource>();
        audioSourcePool[soundName] = newAudioSource;
        return newAudioSource;
    }

    // 添加音效到管理器
    public void AddSoundClip(SoundClip soundClip)
    {
        if (!soundClipDictionary.ContainsKey(soundClip.name))
        {
            soundClips.Add(soundClip);
            soundClipDictionary.Add(soundClip.name, soundClip);
        }
        else
        {
            Debug.LogWarning($"音效名称已存在: {soundClip.name}");
        }
    }

    // 从管理器中移除音效
    public void RemoveSoundClip(string soundName)
    {
        if (soundClipDictionary.ContainsKey(soundName))
        {
            SoundClip soundClip = soundClipDictionary[soundName];
            soundClips.Remove(soundClip);
            soundClipDictionary.Remove(soundName);

            if (audioSourcePool.ContainsKey(soundName))
            {
                AudioSource audioSource = audioSourcePool[soundName];
                if (audioSource != null)
                {
                    Destroy(audioSource);
                }
                audioSourcePool.Remove(soundName);
            }
        }
    }
}
