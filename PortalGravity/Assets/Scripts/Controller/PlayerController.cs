
public class PlayerController : BasePlayer
{
    private void Awake() {
        initialize();
    }
    void Start()
    {
        setSubscribe();
        setMoveSubscribe();
    }
}
