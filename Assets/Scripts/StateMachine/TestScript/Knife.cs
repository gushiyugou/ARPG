using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knife : MonoBehaviour
{
    public GameObject knife;
    private void ShowKnife()
    {
        knife.SetActive(true);
    }

    private void HideKnife()
    {
        knife.SetActive(false);
    }
}
