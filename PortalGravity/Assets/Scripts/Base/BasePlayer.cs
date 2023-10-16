using System;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Cysharp.Threading.Tasks;

public class BasePlayer : MonoBehaviour
{
    private ReactiveProperty<bool> isJumping = new ReactiveProperty<bool>();
    private ReactiveProperty<float> moving = new ReactiveProperty<float>();
    private ReactiveProperty<bool> isChangeGravity = new ReactiveProperty<bool>();
    private ReactiveProperty<bool> isWarpBeadShot = new ReactiveProperty<bool>();
    private ReactiveProperty<bool> isChangeAbility = new ReactiveProperty<bool>();

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

    [SerializeField, Tooltip("弾挙動管理クラス")]
    protected WarpBeatController warpBead;

    protected void initialize()
    {
        warpBead ??= this.transform.GetChild(0).GetComponent<WarpBeatController>();
    }

    protected void setSubscribe()
    {
          this.UpdateAsObservable()
            .TakeUntilDestroy(this)
            .Subscribe(_ => 
            {
                isJumping.Value = Input.GetKeyDown(KeyCode.Space);
                moving.SetValueAndForceNotify(Input.GetAxis("Horizontal"));
                isChangeGravity.Value = ability == Enums.PlayerAbility.GRAVITY && IsGrounded && Input.GetMouseButtonDown(0);
                isWarpBeadShot.Value = ability == Enums.PlayerAbility.WARP && !warpBead.Bead.activeSelf && Input.GetMouseButtonDown(0);
                isChangeAbility.Value = Input.GetKeyDown(KeyCode.E); 
            });

        this.UpdateAsObservable()
            .TakeUntilDestroy(this)
            .Subscribe(_ => 
            {
                cstMouseRay();
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
                        target.GetComponent<GravityBoxController>().CangeGravity();
                    }   
                }         
                else
                {
                    //自身の重力変化
                    MethodFactory.ChangeGravity(this.gameObject);
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
                // 以下重力反転時==============================================================================
                if(this.GetComponent<Rigidbody2D>().gravityScale == 0f && !IsGrounded)
                {
                    moveDirection.y += -Physics.gravity.y * Time.deltaTime;
                }
                else
                {
                    moveDirection.y = 0;
                }
                // 以上重力反転時==============================================================================


                this.transform.position += moveDirection * Time.deltaTime;
            });
    }
    
    //移動
    private void movement(float moveValue)
    {
        if(!IsGrounded)
            moveDirection.x = moveValue * MoveSpeed;
        else
            moveDirection.x = moveValue * ((int)MoveSpeed << 1);

    }

    // ジャンプ
    private void jump()
    {
        moveDirection.x = 0;

        if(GetComponent<Rigidbody2D>().gravityScale != 0)
            this.GetComponent<Rigidbody2D>().velocity = Vector2.up *  jumpForce;
        else
            this.GetComponent<Rigidbody2D>().velocity = Vector2.down *  jumpForce;
    }

    // マウスカーソルの位置から「レイ」を飛ばして、何かのコライダーに当たるかどうかをチェック
    private void cstMouseRay()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        var hit = Physics2D.Raycast(ray.origin, ray.direction);
        if (hit.collider)
        {
            if(hit.collider.gameObject.name == "GravityBox")
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
        warpBead.Bead.SetActive(true);
        warpBead.Bead.transform.position = this.transform.position;

        warpBead.SetVec.Value = Camera.main.ScreenToWorldPoint(Input.mousePosition).x - this.transform.position.x;
    }
}
