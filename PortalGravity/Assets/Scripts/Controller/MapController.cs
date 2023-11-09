using System.Diagnostics;
using UnityEngine;

using Cysharp.Threading.Tasks;

public class MapController : BaseMap
{
    private async void Awake() 
    {
        await UniTask.WaitUntil(() => ObjectFactory.Instance != null);
        
        ObjectFactory.Instance.Map = this;
        
    }
    void Start()
    {
        setSubscribe();
    }

    void Update()
    {
        
    }
}
