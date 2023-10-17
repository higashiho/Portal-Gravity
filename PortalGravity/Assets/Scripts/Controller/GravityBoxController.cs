
public class GravityBoxController : BaseGravityBox
{
    private void Awake() 
    {
        ObjectFactory.GravityBox = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        setSubscribe();
    }
}
