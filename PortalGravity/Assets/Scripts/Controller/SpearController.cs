using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearController : BaseSpear
{
    private void Awake()
    {
        ObjectFactory.Spear = this;
    }
    private void Start() 
    {
        
    }
}
