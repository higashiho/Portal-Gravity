
public class WarpBeatController : BaseWarpBead
{
    private void Awake() 
    {
        ObjectFactory.WarpBeat = this;
        initialize();
        setSubscribe();
    }
}
