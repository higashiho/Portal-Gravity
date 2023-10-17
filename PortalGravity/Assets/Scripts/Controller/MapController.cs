using System.Collections;
using System.Collections.Generic;
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
}
