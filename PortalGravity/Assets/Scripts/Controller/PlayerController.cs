using UnityEngine;

public class PlayerController : BasePlayer
{
    private void Awake() {
        ObjectFactory.Player = this;
        initialize();
    }
    void Start()
    {
        setSubscribe();
        setMoveSubscribe();
    }


    void Update()
    {        
        // カメラ移動
        moveCamera();
    }
}
