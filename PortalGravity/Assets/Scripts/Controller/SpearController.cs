using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class SpearController : BaseSpear
{
    private async void Awake()
    {
        await UniTask.WaitUntil(() => ObjectFactory.Instance != null);
        ObjectFactory.Instance.Spears.Add(this);
    }
    private void Start() 
    {
        setSubscribe();
    }
}
