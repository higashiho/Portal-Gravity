using UnityEngine;
using UniRx;
using UnityEditor.EditorTools;
using UniRx.Triggers;
using Cysharp.Threading.Tasks;

public class BaseWarpBead : MonoBehaviour
{
    [SerializeField, Tooltip("生成する弾")]
    protected GameObject bead = default;
    public GameObject Bead => bead;
    private Rigidbody2D rb;
    private LineRenderer lr;

    [SerializeField, Tooltip("打つ強さ")]
    private float power = 2.0f;

    [SerializeField, Tooltip("発射時マウス座標初期地点")]
    private Vector2 startPos;
    [SerializeField, Tooltip("発射時マウス座標最終地点")]
    private Vector2 endPos;
    private Vector2 force;
    public Vector2 Force => force;
    private ReactiveProperty<Vector2> setVec = new ReactiveProperty<Vector2>();
    public ReactiveProperty<Vector2> SetVec => setVec;

    private ReactiveProperty<bool> isShotStart = new ReactiveProperty<bool>(false);
    public ReactiveProperty<bool> IsShotStart => isShotStart;
    private ReactiveProperty<bool> isChangeForce = new ReactiveProperty<bool>(false);
    public ReactiveProperty<bool> IsChangeForce => isChangeForce;

    protected void initialize()
    {
        bead = this.gameObject;
        bead.SetActive(false);

        lr = this.transform.parent.GetComponent<LineRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    protected void setSubscribe()
    {
        setVec
            .TakeUntilDestroy(this)
            .Subscribe(x =>
            {
                rb.isKinematic = false;
                rb.AddForce(x, ForceMode2D.Impulse);
            
            });

        IsShotStart
            .TakeUntilDestroy(this)
            .Where(x => x)
            .Subscribe(_ => 
            {
                setStartPos();
            });
        IsChangeForce
            .TakeUntilDestroy(this)
            .Where(x => x)
            .Subscribe(x => 
            {
                setForce();
            });
    }

    public void ClearLineRenderer()
    {
        lr.positionCount = 0;
    }
    private void setStartPos()
    {
        rb.velocity = Vector3.zero;
        rb.isKinematic = true;
        startPos = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
    
    // プレイヤーのクリック位置から発射力を計算し、弾道を表示
    private void setForce()
    {
        // マウスクリック位置をワールド座標に変換して終点 (endPos) を設定
        endPos = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // 発射力を計算
        force = Vector2.ClampMagnitude((startPos - endPos), Constant.SHOT_MAXPOWER) * power;

        // 物体の弾道を計算
        Vector2[] trajectory = plot(rb, (Vector2)this.transform.parent.transform.position, force, 400);

        // LineRendererの位置点数を設定
        lr.positionCount = trajectory.Length;

        // 弾道上の位置情報をLineRendererに設定
        Vector3[] positions = new Vector3[trajectory.Length];
        for (int i = 0; i < trajectory.Length; i++)
        {
            positions[i] = trajectory[i];
        }
        lr.SetPositions(positions);
    }


    // 物体の運動をシミュレートして、一連のフレームで物体の位置を計算
    private Vector2[] plot(Rigidbody2D rigidbody, Vector2 pos, Vector2 force, int steps)
    {
        // ステップごとの位置情報を格納する配列
        Vector2[] results = new Vector2[steps];

        // 物理エンジンの時間ステップを計算
        float timestep = Time.fixedDeltaTime / Physics2D.velocityIterations;

        // 重力の影響を計算
        Vector2 gravityAccel = Physics2D.gravity * rigidbody.gravityScale * timestep * timestep;

        // ドラッグの影響を計算
        float drag = 1f - timestep * rigidbody.drag;

        // ステップごとの移動ベクトルを計算
        Vector2 moveStep = force * timestep;

        // 指定されたステップ数分のループ
        for (int i = 0; i < steps; i++)
        {
            // 重力の影響を加算
            moveStep += gravityAccel;

            // ドラッグによる減衰を適用
            moveStep *= drag;

            // 位置を更新
            pos += moveStep;

            // 位置情報を結果配列に保存
            results[i] = pos;
        }

        // 各ステップでの位置情報を含む配列を返す
        return results;
    }
}
