using System;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Cysharp.Threading.Tasks;
using DG.Tweening;

public class BasePlayer : MonoBehaviour
{
    private ReactiveProperty<bool> isJumping = new ReactiveProperty<bool>();
    private ReactiveProperty<float> moving = new ReactiveProperty<float>();
    private ReactiveProperty<bool> isChangeGravity = new ReactiveProperty<bool>();
    private ReactiveProperty<bool> isWarpBeadShot = new ReactiveProperty<bool>();
    private ReactiveProperty<bool> isChangeAbility = new ReactiveProperty<bool>();
    private ReactiveProperty<bool> isRetry = new ReactiveProperty<bool>();
    public ReactiveProperty<bool> IsRetry => isRetry;

    private ReactiveProperty<bool> isUpdateRetrayPos = new ReactiveProperty<bool>();

    [SerializeField, Tooltip("移動スピード")]
    private float moveSpeed = default;
    public float MoveSpeed{get => moveSpeed; set{ if(moveSpeed != value) moveSpeed = value;}}

    [SerializeField, Tooltip("ジャンプ力")]
    private float jumpForce = default;
    [SerializeField, Tooltip("地面に触れているか")]
    private bool isGrounded = default;
    public bool IsGrounded{get => isGrounded; set => isGrounded = value;}
    [SerializeField, Tooltip("次フレームの移動追加値")]
    private Vector3 moveDirection;

    [SerializeField, Tooltip("重力変化対象オブジェクト")]
    private GameObject targetGravityBox = default;

    [SerializeField, Tooltip("重力かワープか")]
    private Enums.PlayerAbility ability = Enums.PlayerAbility.GRAVITY;
    public Enums.PlayerAbility Ability{get => ability;}

    [SerializeField, Tooltip("Retry座標")]
    private Vector3 retryPos = new Vector3(0,0,0);
    public Vector3 RetryPos{get => retryPos; set => retryPos = value;}
    


    private bool[] isNextStages = new bool[]
    {
        false, false, false
    };
    public bool[] IsNextStages{get => isNextStages; set => isNextStages = value;}
    protected void initialize()
    {
        RetryPos = this.transform.position;
    }


    protected void setSubscribe()
    {
        
          this.UpdateAsObservable()
            .TakeUntilDestroy(this)
            .Subscribe(_ => 
            {
                isJumping.Value = Input.GetKeyDown(KeyCode.Space);
                moving.SetValueAndForceNotify(Input.GetAxis("Horizontal"));
                isChangeAbility.Value = Input.GetKeyDown(KeyCode.E); 
                IsRetry.Value = MethodFactory.CheckOnCamera(this.gameObject);
                ObjectFactory.WarpBeat.IsOnCamera.Value = !MethodFactory.CheckOnCamera(ObjectFactory.WarpBeat.gameObject);
                isUpdateRetrayPos.Value = IsNextStages[(int)ObjectFactory.Map.UpdateMapNum.Value] && 
                                            this.transform.position.x >= Camera.main.transform.position.x + 8.5f;

                if(ability == Enums.PlayerAbility.WARP && !ObjectFactory.WarpBeat.Bead.activeSelf)
                {
                    ObjectFactory.WarpBeat.IsShotStart.Value = Input.GetMouseButtonDown(0);
                    ObjectFactory.WarpBeat.IsChangeForce.SetValueAndForceNotify(Input.GetMouseButton(0));
                    isWarpBeadShot.Value =  Input.GetMouseButtonUp(0);
                }
                else if(Ability == Enums.PlayerAbility.GRAVITY)
                {
                    isChangeGravity.Value = IsGrounded && Input.GetMouseButtonDown(0);
                }
            });

        this.UpdateAsObservable()
            .TakeUntilDestroy(this)
            .Subscribe(_ => 
            {
                cstMouseRay();
            });

        IsRetry
            .TakeUntilDestroy(this)
            .Where(x => x)
            .Subscribe(_ =>
            {
                StageRetry();
            });

        isUpdateRetrayPos
            .TakeUntilDestroy(this)
            .Where(x => x)
            .Subscribe(_ =>
            {
                // Retrypos更新
                retryPos = this.transform.position;
                retryPos.x++;
                this.GetComponent<Rigidbody2D>().gravityScale = 1;
                ObjectFactory.WarpBeat.Rb.gravityScale = 1;

                // マップ生成挙動＋カメラ移動挙動
                Camera.main.transform.position = new Vector3(
                    this.transform.position.x + Constant.CAMERA_DRAW_LEFT_POS, 
                    Camera.main.transform.position.y, 
                    Camera.main.transform.position.z
                );
                this.transform.position = retryPos;
            });
    }
    // 挙動
    protected void setMoveSubscribe()
    {

        isJumping
            .TakeUntilDestroy(this)
            // ボタンが押された時に、
            .Where(x => x)
            // 接地中であり、
            .Where(_ => IsGrounded)
            // 最後にジャンプしてから1秒以上経過しているなら、
            .ThrottleFirst(TimeSpan.FromSeconds(1))
            .Subscribe(_ =>
            {
                // ジャンプ処理を実行する
                jump();
            });

                
        moving
            .TakeUntilDestroy(this)
            .Subscribe(x =>
            {
                // そっち方向に移動する
                movement(x);
            });
        
        isChangeGravity
            .TakeUntilDestroy(this)
            .Where(x => x)
            .Subscribe(async x =>
            {
                var target = targetGravityBox;
                // １秒待つ
                await UniTask.Delay(1000);

                // 重力変化
                if(target)
                {
                    //対象の重力変化
                    if(target.name == "GravityBox")
                    {
                        target.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;           
                        target.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
                        MethodFactory.ChangeGravity(target);
                    }   
                }         
                else
                {
                    this.transform.DOLocalRotate(Vector3.right * 180, 1).SetEase(Ease.Linear).OnStart(() => {
                        //自身の重力変化
                        MethodFactory.ChangeGravity(this.gameObject);
                        MethodFactory.ChangeGravity(ObjectFactory.WarpBeat.gameObject);
                    }).SetLink(this.gameObject);
                }
            });
        
        isWarpBeadShot
            .TakeUntilDestroy(this)
            .Where(x => x)
            .Subscribe(_ => 
            {

                shotWarpBead();
            });

        isChangeAbility
            .TakeUntilDestroy(this)
            .Where(x => x)
            .Subscribe(_ => 
            {
                
                if(ability == Enums.PlayerAbility.GRAVITY)
                    ability = Enums.PlayerAbility.WARP;
                else
                    ability = Enums.PlayerAbility.GRAVITY;
            });

        this.UpdateAsObservable()
            .TakeUntilDestroy(this)
            .Subscribe(_ => 
            { 
                this.transform.position += moveDirection * Time.deltaTime;
            
                if(this.transform.position.x <= RetryPos.x)
                    this.transform.position = new Vector3(RetryPos.x, this.transform.position.y, this.transform.position.z);
            });
    }
    
    //移動
    private void movement(float moveValue)
    {
        if(IsGrounded)
            moveDirection.x = moveValue * MoveSpeed;
        else
            moveDirection.x = moveValue * ((int)MoveSpeed >> 1);

    }

    // ジャンプ
    private void jump()
    {
        moveDirection.x = 0;

        var force = this.GetComponent<Rigidbody2D>().gravityScale == 1 ? jumpForce : -jumpForce;

        this.GetComponent<Rigidbody2D>().velocity = Vector2.up * force;
    }
    

    // マウスカーソルの位置から「レイ」を飛ばして、何かのコライダーに当たるかどうかをチェック
    private void cstMouseRay()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        var hit = Physics2D.Raycast(ray.origin, ray.direction);
        if (hit.collider)
        {
            if(hit.collider.gameObject.tag == "GravityBox")
                targetGravityBox = hit.collider.gameObject;
            else
                targetGravityBox = null;
        }
        else
        {
            targetGravityBox = null;
        }
    }

    // ワープ弾発射挙動
    private void shotWarpBead()
    {
        ObjectFactory.WarpBeat.Bead.SetActive(true);
        ObjectFactory.WarpBeat.Bead.transform.position = this.transform.position;

        ObjectFactory.WarpBeat.transform.parent = null;
        ObjectFactory.WarpBeat.SetVec.Value = ObjectFactory.WarpBeat.Force;
    }

    // ゲームオーバー要素を取られた
    public void StageRetry()
    {
        // TO:DOシーンの読み込み直しかステージ生成し直しかを用調整
        // 読み込み直しの場合はIsNextStagesをstaticにするのがよいかも。
        // ステージ生成挙動が完成したら変更予定

        this.transform.position = RetryPos;
        IsNextStages[(int)ObjectFactory.Map.UpdateMapNum.Value] = false;
        ObjectFactory.WarpBeat.Resets(Vector3.zero);
        this.GetComponent<Rigidbody2D>().gravityScale = 1;

        // // ステージ生成し直し
        // ObjectFactory.Map.UpdateMapNum.SetValueAndForceNotify(ObjectFactory.Map.UpdateMapNum.Value);
    }
}
