using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;
    public  static AudioManager Instance => instance;
    [SerializeField] private AudioSource publicAudioSource;
    private Dictionary<string,AudioClip> audioClips = new Dictionary<string,AudioClip>();

    private void Awake()
    {
        if(publicAudioSource == null)
        {
            
            Debug.Log("请为音效管理器指定AudioSource组件，否则将使用AudioManager自身的播放器");
            publicAudioSource = GetComponent<AudioSource>();
        }
        AudioClipConfig.Instance.AddAudioPath();
        AddAudioClip(AudioClipConfig.Instance.GetAudioPath("RunStop"));
    }

    public void Play(string name)
    {
        publicAudioSource.PlayOneShot(audioClips[name]);
    }
    public void AddAudioClip(string name)
    {
        AudioClip audio = Resources.Load<AudioClip>(AudioClipConfig.Instance.GetAudioPath(name));
        if (!audioClips.ContainsKey(name))
        {
            audioClips.Add(name, audio);
        }
    }

    

    
    



}
