using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class PlayerModle : ModelBase
{
    private Action<int> atkEndAduio;

    public void AddAtkEndAudio(Action<int> atkEndAduio)
    {
        this.atkEndAduio = atkEndAduio;
    }

    private void AtkEndAudio(int audioIndex)
    {
        atkEndAduio?.Invoke(audioIndex);
    }
}
