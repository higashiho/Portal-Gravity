using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : BasePlayer
{
    // Start is called before the first frame update
    void Start()
    {
        setSubscribe();
        setMoveSubscribe();
    }
}
