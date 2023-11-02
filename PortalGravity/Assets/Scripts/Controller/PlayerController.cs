
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
}
