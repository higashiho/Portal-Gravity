using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;
using UniRx.Triggers;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;

public class BasePlayer : MonoBehaviour
{
    private ReactiveProperty<float> moving = new ReactiveProperty<float>();
    private ReactiveProperty<bool> isJumping = new ReactiveProperty<bool>();
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
    
    public Vector3 RetryPos{get; private set;} = new Vector3(0,0,0);

    // 重力変化挙動中か
    private bool nowChangeGravity = false;


    private ReactiveProperty<bool> updateMapOpierationFall = new ReactiveProperty<bool>();

    private ReactiveProperty<bool> updateMapOpierationRise = new ReactiveProperty<bool>();

    protected ReactiveProperty<Enums.MapOrientation> updateMapOpieration = new ReactiveProperty<Enums.MapOrientation>(Enums.MapOrientation.DEFAULT);


    private Rigidbody2D playerRigidBody = new Rigidbody2D();



    // 各ステージのカメラの座標の配列
    protected Vector3[] cameraStagePos = new Vector3[4];

    [SerializeField]
    protected TextMeshProUGUI gameClearText = default;
    


    private bool[] isNextStages = new bool[]
    {
        false, false, false, false
    };
    public bool[] IsNextStages{get => isNextStages; set => isNextStages = value;}

    private TextMeshProUGUI abilityText = default;
    protected async void initialize()
    {
        ObjectFactory.Instance = new ObjectFactory();
        RetryPos = this.transform.position;

        abilityText = this.gameObject.GetComponentInChildren<TextMeshProUGUI>();

        cameraStagePos = new Vector3[]
        {
            new Vector3(0,0,Camera.main.transform.position.z),
            new Vector3(Constant.CAMERA_STAGE2_POS_X, 0, Camera.main.transform.position.z),
            new Vector3(Constant.CAMERA_STAGE3_POS_X, 0, Camera.main.transform.position.z),
            new Vector3(Constant.CAMERA_STAGE3_POS_X, Constant.CAMERA_STAGE3_DOWN_POS_Y, Camera.main.transform.position.z)      
        };

        playerRigidBody = this.GetComponent<Rigidbody2D>();
        
        this.GetComponent<SpriteRenderer>().color = Ability switch
        {
            Enums.PlayerAbility.WARP => Color.red,
            Enums.PlayerAbility.GRAVITY => Color.blue,
            _ => default
        };
                    
        abilityText.color = new Color(abilityText.color.r, abilityText.color.g, abilityText.color.b , 1);
        DOTween.Kill(abilityText);
        abilityText.text = ability.ToString();
        
        await UniTask.Delay(300);
        abilityText.DOFade(0, 0.7f).SetEase(Ease.OutQuad).SetLink(this.gameObject);

    }


    protected void setSubscribe()
    {
        
          this.UpdateAsObservable()
            .TakeUntilDestroy(this)
            .Where(_ => !gameClearText.gameObject.activeSelf)
            .Subscribe(_ => 
            {
                if(ObjectFactory.Instance == null || ObjectFactory.Instance.Player == null) return;
               
                Debug.DrawRay(gameObject.transform.position, Vector3.right * Constant.PLAYER_RAY_DISTANCE, Color.blue, 0.1f);
                isJumping.Value = Input.GetKeyDown(KeyCode.Space);
                moving.SetValueAndForceNotify(Input.GetAxis("Horizontal"));
                isChangeAbility.Value = Input.GetKeyDown(KeyCode.E); 
                IsRetry.Value = MethodFactory.CheckOnCamera(this.gameObject);
                ObjectFactory.Instance.WarpBeat.IsOnCamera.Value = !MethodFactory.CheckOnCamera(ObjectFactory.Instance.WarpBeat.gameObject);
                isUpdateRetrayPos.Value = IsNextStages[(int)ObjectFactory.Instance.Map.UpdateMapNum.Value] &&
                                            this.transform.position.x >= Camera.main.transform.position.x + 8.5f;
                updateMapOpierationFall.Value = updateMapOpieration.Value == Enums.MapOrientation.BOTTOM;
                updateMapOpierationRise.Value = updateMapOpieration.Value == Enums.MapOrientation.TOP;

                if(ability == Enums.PlayerAbility.WARP && !ObjectFactory.Instance.WarpBeat.Bead.activeSelf)
                {
                    ObjectFactory.Instance.WarpBeat.IsShotStart.Value = Input.GetMouseButtonDown(0);
                    ObjectFactory.Instance.WarpBeat.IsChangeForce.SetValueAndForceNotify(Input.GetMouseButton(0));
                    isWarpBeadShot.Value =  Input.GetMouseButtonUp(0);
                }
                else if(Ability == Enums.PlayerAbility.GRAVITY && IsGrounded)
                {
                    isChangeGravity.Value = !nowChangeGravity && Input.GetMouseButtonDown(0);
                }

                
                foreach(var spear in ObjectFactory.Instance.Spears)
                {
                    if(spear.GetComponent<Rigidbody2D>().gravityScale == 0)
                    {
                        spear.IsFall.Value = this.transform.position.x > spear.transform.position.x - spear.transform.localScale.x &&
                                                this.transform.position.x < spear.transform.position.x + spear.transform.localScale.x ;

                    }

                }
            });

        this.UpdateAsObservable()
            .TakeUntilDestroy(this)
            .Where(_ => !gameClearText.gameObject.activeSelf)
            .Subscribe(_ => 
            {
                cstMouseRay();
            });

        IsRetry
            .TakeUntilDestroy(this)
            .Where(_ => !gameClearText.gameObject.activeSelf)
            .Where(x => x)
            .Subscribe(_ =>
            {
                // プレイヤーがステージ３UPで左上のから画面外に出ようとするとリトライ
                if(Camera.main.transform.position.y == cameraStagePos[2].y &&
                   this.transform.position.x <= Camera.main.transform.position.x &&
                   this.transform.position.y >= Camera.main.transform.position.y + Camera.main.orthographicSize ||
                   Camera.main.transform.position.y == cameraStagePos[2].y &&
                   this.transform.position.x >= Camera.main.transform.position.x)
                {
                    StageRetry();

                    return;
                }
                    
                // 画面外に出た時にステージDOWNの判定を拾ったら
                if(updateMapOpieration.Value == Enums.MapOrientation.BOTTOM)
                {
                    // ステージ３DOWNに移動した後のプレイヤーの座標
                    Vector3 stage3FallAfterPos = new Vector3(
                        this.transform.position.x,
                        this.transform.position.y - 1.0f,
                        this.transform.position.z
                    );

                    this.transform.DOMove(stage3FallAfterPos, Constant.CAMERA_MOVE_TIME).SetEase(Ease.InCubic)
                    .OnStart(() =>
                    {
                        playerRigidBody.velocity = Vector2.zero;
                    });

                    return;
                }
                
                

                    // 画面外に出た時にステージUPの判定を拾ったら
                if(updateMapOpieration.Value == Enums.MapOrientation.TOP)
                {
                    // ステージ３UPに移動した後のプレイヤーの座標
                    Vector3 stage3RiseAfterPos = new Vector3(
                        this.transform.position.x,
                        this.transform.position.y + 1.0f,
                        this.transform.position.z
                    );

                    this.transform.DOMove(stage3RiseAfterPos, Constant.CAMERA_MOVE_TIME).SetEase(Ease.InCubic)
                    .OnStart(() =>
                    {
                        playerRigidBody.velocity = Vector2.zero;
                    });

                    return;
                }            
                
                StageRetry();
               
            });

        isUpdateRetrayPos
            .TakeUntilDestroy(this)
            .Where(_ => !gameClearText.gameObject.activeSelf)
            .Where(x => x)
            .Subscribe(async _ =>
            {
                if((int)ObjectFactory.Instance.Map.UpdateMapNum.Value + 1 >= Constant.RETRY_POS_Y.Length)
                {
                    gameClearText.gameObject.SetActive(true);
                    await UniTask.WaitUntil(() => Input.GetKeyDown(KeyCode.Return));
                    SceneManager.LoadScene("Title");
                    return;
                }


                // Retrypos更新
                RetryPos = new Vector3(
                    Mathf.Round(this.transform.position.x * 10) / 10 + Vector3.right.x,
                    Constant.RETRY_POS_Y[(int)ObjectFactory.Instance.Map.UpdateMapNum.Value + 1],
                    this.transform.position.z
                );
                

                
                this.GetComponent<Rigidbody2D>().gravityScale = 1;
                ObjectFactory.Instance.WarpBeat.Rb.gravityScale = 1;
                this.transform.eulerAngles = Vector3.zero;


                setStageObject();
                

                // 次のステージを生成
                ObjectFactory.Instance.Map.MapStateincrement();

                // カメラを次のステージに移動させる
                moveToNextStage((int)ObjectFactory.Instance.Map.UpdateMapNum.Value);
            });


        // マップ上か下かのイベント    
        updateMapOpierationFall
            .TakeUntilDestroy(this)
            .Where(_ => !gameClearText.gameObject.activeSelf)
            .Where(x => x)
            .Subscribe(x => 
            {
                Debug.Log("fall");
                moveToNextStage((int)Enums.MapNum.STAGE_3 + 1);
            });

        updateMapOpierationRise
            .TakeUntilDestroy(this)
            .Where(_ => !gameClearText.gameObject.activeSelf)
            .Where(x => x)
            .Subscribe(x =>
            {
                Debug.Log("rise");
                moveToNextStage((int)Enums.MapNum.STAGE_3);
            });
    }
    // 挙動
    protected void setMoveSubscribe()
    {

        isJumping
            .TakeUntilDestroy(this)
            // ボタンが押された時に、
            .Where(_ => !gameClearText.gameObject.activeSelf)
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
            .Where(_ => !gameClearText.gameObject.activeSelf)
            .Subscribe(x =>
            {
                // そっち方向に移動する
                movement(x);
            });
        
        isChangeGravity
            .TakeUntilDestroy(this)
            .ThrottleFirst(TimeSpan.FromSeconds(1))
            .Where(_ => !gameClearText.gameObject.activeSelf)
            .Where(x => x)
            .Subscribe(async x =>
            {
                var target = targetGravityBox;
                nowChangeGravity = true;
                // １秒待つ
                await UniTask.Delay(1000);

                // 重力変化
                if(target)
                {
                    switch(target.tag)
                    {
                        case "ReverseSting":
                            int playerLayer = 1 << 8;
                            RaycastHit2D hit = Physics2D.Raycast(gameObject.transform.position, Vector3.right, Constant.PLAYER_RAY_DISTANCE, ~playerLayer);
                    
                            if(hit.collider.tag == target.tag)
                            {
                                //対象の重力変化
                                target.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;           
                                target.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
                                MethodFactory.ChangeGravity(target);
                            
                            }
                        break;
                        default:
                            //対象の重力変化
                            target.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;           
                            target.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
                            MethodFactory.ChangeGravity(target);
                        break;
                    }

                }         
                else
                {
                    
                    this.transform.DOLocalRotate(
                            this.GetComponent<Rigidbody2D>().gravityScale == 1 ? Vector3.right * 180 : Vector3.zero,
                            1
                        ).SetEase(Ease.Linear).OnStart(() => {
                        //自身の重力変化
                        MethodFactory.ChangeGravity(this.gameObject);
                        MethodFactory.ChangeGravity(ObjectFactory.Instance.WarpBeat.gameObject);
                    }).SetLink(this.gameObject);
                }

                nowChangeGravity = false;
            });
        
        isWarpBeadShot
            .TakeUntilDestroy(this)
            .Where(_ => !gameClearText.gameObject.activeSelf)
            .Where(x => x)
            .Subscribe(_ => 
            {
                shotWarpBead();
            });

        isChangeAbility
            .TakeUntilDestroy(this)
            .Where(_ => !gameClearText.gameObject.activeSelf)
            .Where(x => x)
            .Subscribe(async _ => 
            {
                ObjectFactory.Instance.WarpBeat.Lr.positionCount = 0;
                if(ability == Enums.PlayerAbility.GRAVITY)
                {
                    ability = Enums.PlayerAbility.WARP;
                    this.GetComponent<SpriteRenderer>().color = Color.red;
                    
                    abilityText.color = new Color(abilityText.color.r, abilityText.color.g, abilityText.color.b , 1);
                    DOTween.Kill(abilityText);
                    abilityText.text = ability.ToString();
                    
                    await UniTask.Delay(300);
                    abilityText.DOFade(0, 0.7f).SetEase(Ease.OutQuad).SetLink(this.gameObject);
                
                }
                else
                {
                    ability = Enums.PlayerAbility.GRAVITY;
                    this.GetComponent<SpriteRenderer>().color = Color.blue;
                    
                    abilityText.color = new Color(abilityText.color.r, abilityText.color.g, abilityText.color.b , 1);
                    DOTween.Kill(abilityText);
                    abilityText.text = ability.ToString();
                    
                    await UniTask.Delay(300);
                    abilityText.DOFade(0, 0.7f).SetEase(Ease.OutQuad).SetLink(this.gameObject);
                }
            });
    
        this.UpdateAsObservable()
            .Where(_ => !gameClearText.gameObject.activeSelf)
            .TakeUntilDestroy(this)
            .Subscribe(_ => 
            { 
                this.transform.position += moveDirection * Time.deltaTime;
            
                if(this.transform.position.x <= RetryPos.x)
                    this.transform.position = new Vector3(RetryPos.x, this.transform.position.y, this.transform.position.z);
                else if(this.transform.position.x >= Camera.main.transform.position.x + Constant.CAMERA_DRAW_WIDE)
                    this.transform.position = new Vector3(
                        Camera.main.transform.position.x + Constant.CAMERA_DRAW_WIDE, 
                        this.transform.position.y, 
                        this.transform.position.z
                    );

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
            if(hit.collider.gameObject.tag == "GravityBox" || hit.collider.gameObject.tag == "ReverseSting")
                targetGravityBox = hit.collider.gameObject;
            else if(hit.collider.transform.parent != null && hit.collider.transform.parent.gameObject.tag == "GroundSting" || 
                        hit.collider.transform.parent != null && hit.collider.transform.parent.gameObject.tag == "Spear")
                targetGravityBox = hit.collider.transform.parent.gameObject;
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
        ObjectFactory.Instance.WarpBeat.Bead.transform.eulerAngles = Vector3.zero;
        ObjectFactory.Instance.WarpBeat.Bead.SetActive(true);
        ObjectFactory.Instance.WarpBeat.Bead.transform.position = this.transform.position;

        ObjectFactory.Instance.WarpBeat.transform.parent = null;
        ObjectFactory.Instance.WarpBeat.SetVec.Value = ObjectFactory.Instance.WarpBeat.Force;
    }

    // ゲームオーバー要素を取られた
    public void StageRetry()
    {
        // TO:DOシーンの読み込み直しかステージ生成し直しかを用調整
        // 読み込み直しの場合はIsNextStagesをstaticにするのがよいかも。
        // ステージ生成挙動が完成したら変更予定
        this.transform.eulerAngles = Vector3.zero;
        this.transform.position = RetryPos;
        IsNextStages[(int)ObjectFactory.Instance.Map.UpdateMapNum.Value] = false;
        ObjectFactory.Instance.WarpBeat.Resets(Vector3.zero);
        this.GetComponent<Rigidbody2D>().gravityScale = 1;
        ObjectFactory.Instance.WarpBeat.Rb.gravityScale = 1;

        setStageObject();

        ObjectFactory.Instance.Map.DeleteStageObject();

        // // ステージ生成し直し
         ObjectFactory.Instance.Map.UpdateMapNum.SetValueAndForceNotify(ObjectFactory.Instance.Map.UpdateMapNum.Value);
    }



    /// <summary>
    /// 次のステージへ移動
    /// </summary>
    private void moveToNextStage(int num)
    {
        Camera.main.transform.DOMove(cameraStagePos[num], Constant.CAMERA_MOVE_TIME).SetEase(Ease.OutSine)
        .OnComplete(() =>
        {
            // 前のステージのオブジェクトを非表示
            ObjectFactory.Instance.Map.DeleteStageObject();
        });

        // ステージ３上下移動の時はリトライしない
        // todo:条件を細分化
        if((int)ObjectFactory.Instance.Map.UpdateMapNum.Value == (int)Enums.MapNum.STAGE_3 &&
            this.transform.position.x <= Camera.main.transform.position.x)
            return;

        this.transform.DOMove(RetryPos, Constant.CAMERA_MOVE_TIME).SetEase(Ease.InCubic);
    }



    /// <summary>
    /// ステージオブジェクトを格納する
    /// </summary>
    private void setStageObject()
    {
        // 生成したステージオブジェクトを格納する配列
        Transform[] mapchiledlen = new Transform[ObjectFactory.Instance.Map.transform.childCount];

        ObjectFactory.Instance.Map.beforeStageObjects = new Transform[ObjectFactory.Instance.Map.transform.childCount];

        // ステージのオブジェクトを登録
        for(int i = 0; i < mapchiledlen.Length; i++)
        {
            mapchiledlen[i] = ObjectFactory.Instance.Map.transform.GetChild(i);

            ObjectFactory.Instance.Map.beforeStageObjects[i] = mapchiledlen[i];
        }
    }

    /// <summary>
    /// プレイヤーがステージ３の上か下かを判断
    /// </summary>
    protected void judgeStage3TOPorUNDER()
    {

        if(updateMapOpieration.Value == Enums.MapOrientation.DEFAULT &&
           Camera.main.transform.position.x == cameraStagePos[2].x)
        {
            updateMapOpieration.Value = Enums.MapOrientation.TOP;
        }

        if(this.transform.position.x <= Camera.main.transform.position.x &&
           this.transform.position.y < Camera.main.transform.position.y - Camera.main.orthographicSize &&
           updateMapOpieration.Value == Enums.MapOrientation.TOP)
        {
            updateMapOpieration.Value = Enums.MapOrientation.BOTTOM;
            return;
        }

        if(this.transform.position.x <= Camera.main.transform.position.x &&
           this.transform.position.y > Camera.main.transform.position.y + Camera.main.orthographicSize &&
            updateMapOpieration.Value == Enums.MapOrientation.BOTTOM)
        {
            updateMapOpieration.Value = Enums.MapOrientation.TOP;
        }
    }
}
