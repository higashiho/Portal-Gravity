using UnityEngine;
using UniRx;

public class BaseWarpBead : MonoBehaviour
{
    [SerializeField, Tooltip("生成する弾")]
    protected GameObject bead = default;
    public GameObject Bead => bead;

    [SerializeField, Tooltip("発射の力")]
    private float shotPower = default;

    
    private ReactiveProperty<float> setVec = new ReactiveProperty<float>();
    public ReactiveProperty<float> SetVec => setVec;

    protected void initialize()
    {
        bead = this.gameObject;
        bead.SetActive(false);
    }

    protected void setSubscribe()
    {
        setVec
            .TakeUntilDestroy(this)
            .Subscribe(x =>
            {
                if(x > 0)
                    this.GetComponent<Rigidbody2D>().velocity = new Vector2(shotPower / 2 + Mathf.Pow(Constant.SHOT_GRADIENT * Mathf.Abs(x), 0.2f), shotPower);
                else
                    this.GetComponent<Rigidbody2D>().velocity = new Vector2(-shotPower / 2 + Mathf.Pow(Constant.SHOT_GRADIENT * Mathf.Abs(x), 0.2f), shotPower);
            });
    }
}
