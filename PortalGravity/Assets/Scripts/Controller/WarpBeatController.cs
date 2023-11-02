
using Cysharp.Threading.Tasks;

public class WarpBeatController : BaseWarpBead
{
    private async void Awake() 
    {
        await UniTask.WaitUntil(() => ObjectFactory.Instance != null);
        ObjectFactory.Instance.WarpBeat = this;
        initialize();
        setSubscribe();
    }
}
