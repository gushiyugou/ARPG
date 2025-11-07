using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    
    void Start()
    {
        MonoManager.Instance.AddUpdateListener(TestUpdate);
        Coroutine coroutine = MonoManager.Instance.StartCoroutine(Do());
        MonoManager.Instance.StopCoroutine(coroutine);

    }




    private void TestUpdate()
    {
        Debug.Log(1);

        if (Input.GetMouseButtonDown(0))
        {
            MonoManager.Instance.RemoveUpdateListener(TestUpdate);
        }
    }

    private IEnumerator Do()
    {
        yield return new WaitForSeconds(3F);
        Debug.Log("–≠≥Ã≤‚ ‘ÕÍ±œ£°");
    }

    
}
