using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectPoolManager : SingletonMono<GameObjectPoolManager>
{
   public Dictionary<string ,Dictionary<Type,GameObject>> poolDic = new Dictionary<string ,Dictionary<Type,GameObject>>();
}
