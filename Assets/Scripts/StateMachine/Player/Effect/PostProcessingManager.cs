using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessingManager : SingletonMono<PostProcessingManager>
{
    private Volume postProcessingVolume;
    private ChromaticAberration chromaticAberration;
    [SerializeField]private float speed;

    private void Start()
    {
        postProcessingVolume = GetComponent<Volume>();
        postProcessingVolume.profile.TryGet<ChromaticAberration>(out chromaticAberration);
    }

    public void ChromaticAberrationEF(float value)
    {
        StopAllCoroutines();//关闭之前的协程
        StartCoroutine(StartChromaticAberrationEF(value));
    }

    private IEnumerator StartChromaticAberrationEF(float value)
    {
        while(chromaticAberration.intensity.value < value)
        {
            yield return null;
            chromaticAberration.intensity.value += Time.deltaTime * speed;
        }
        while (chromaticAberration.intensity.value > 0)
        {
            yield return null;
            chromaticAberration.intensity.value -= Time.deltaTime * speed;
        }
    }

}
