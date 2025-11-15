using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioClipConfig
{
    private static AudioClipConfig instance = new AudioClipConfig();
    public static AudioClipConfig Instance => instance;
    
    public Dictionary<string, string> configDic = new Dictionary<string, string>();
    private AudioClipConfig() { }

    public string runStop = "Audio/maoYouAduio/run_end";
    public void AddAudioPath()
    {
        configDic.Add("RunStop", runStop);
    }

    public string GetAudioPath(string name)
    {
        if (configDic.ContainsKey(name))
        {
            return configDic[name];
        }
        return null;
    }
}
