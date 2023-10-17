
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
}
