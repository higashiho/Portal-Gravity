
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
