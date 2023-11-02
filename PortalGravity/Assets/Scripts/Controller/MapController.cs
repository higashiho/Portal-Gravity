using System.Diagnostics;
using UnityEngine;

public class MapController : BaseMap
{
    private void Awake() {
        ObjectFactory.Map = this;
    }
    void Start()
    {
        setSubscribe();
    }

    void Update()
    {
        
    }
}
