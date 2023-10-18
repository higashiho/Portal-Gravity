
using Cysharp.Threading.Tasks;

public class GravityBoxController : BaseGravityBox
{
    private async void Awake() 
    {
        await UniTask.WaitUntil(() => ObjectFactory.Instance != null);
        ObjectFactory.Instance.GravityBox = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        setSubscribe();
    }
}
