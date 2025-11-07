using System.Collections;
using UnityEngine;

public class AnimationSoundTrigger : MonoBehaviour
{
    // 动画事件调用的方法
    public void PlaySound(string soundName)
    {
        SoundManager.Instance.PlaySound(soundName);
    }

    // 动画事件调用的方法 - 带延迟
    public void PlaySoundDelayed(string soundName, float delay)
    {
        StartCoroutine(PlaySoundWithDelay(soundName, delay));
    }

    private IEnumerator PlaySoundWithDelay(string soundName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SoundManager.Instance.PlaySound(soundName);
    }

    // 动画事件调用的停止音效方法
    public void StopSound(string soundName)
    {
        SoundManager.Instance.StopSound(soundName);
    }

    // 动画事件调用的暂停音效方法
    public void PauseSound(string soundName)
    {
        SoundManager.Instance.PauseSound(soundName);
    }

    // 动画事件调用的恢复音效方法
    public void ResumeSound(string soundName)
    {
        SoundManager.Instance.ResumeSound(soundName);
    }
}

