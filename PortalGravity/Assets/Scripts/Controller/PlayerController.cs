using UnityEngine;
using Cysharp.Threading.Tasks;

public class PlayerController : BasePlayer
{
    private void Awake() {
        initialize();
        ObjectFactory.Instance.Player = this;
        
    }
    void Start()
    {
        setSubscribe();
        setMoveSubscribe();
    }

    void Update()
    {
        if(ObjectFactory.Instance.Map.UpdateMapNum.Value == Enums.MapNum.STAGE_3)
        {
            judgeStage3TOPorUNDER();
        }
    }

}
