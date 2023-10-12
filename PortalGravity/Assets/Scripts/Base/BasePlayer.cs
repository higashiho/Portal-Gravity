using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEditor.EditorTools;

public class BasePlayer : MonoBehaviour
{
    private ReactiveProperty<bool> isJumping = new ReactiveProperty<bool>();
    private ReactiveProperty<float> moving = new ReactiveProperty<float>();
    private ReactiveProperty<bool> isChangeGravity = new ReactiveProperty<bool>();

    private float moveSpeed = default;
    public float MoveSpeed{get => moveSpeed; set{ if(moveSpeed != value) moveSpeed = value;}}

    [SerializeField, Tooltip("接地時の移動スピード")]
    private float groundedMoveSpeed = default;
    [SerializeField, Tooltip("ジャンプ時のスピード")]
    private float jumpingMoveSpeed = default;

    [SerializeField, Tooltip("ジャンプ力")]
    private float jumpForce = 5.0f;
    [SerializeField, Tooltip("地面に触れているか")]
    private bool isGrounded = true;
    public bool IsGrounded{get => isGrounded; set => isGrounded = value;}
    [SerializeField, Tooltip("次フレームの移動追加値")]
    private Vector3 moveDirection;

    [SerializeField, Tooltip("重力変化対象オブジェクト")]
    private GameObject targetGravityBox = default;

    protected void setSubscribe()
    {
          this.UpdateAsObservable()
            .TakeUntilDestroy(this)
            .Subscribe(_ => 
            {
                isJumping.Value = Input.GetKeyDown(KeyCode.Space);
                moving.SetValueAndForceNotify(Input.GetAxis("Horizontal"));
                isChangeGravity.Value = Input.GetMouseButton(0);
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
                IsGrounded = !IsGrounded;
                // ジャンプ処理を実行する
                jump();
            });

                
        moving
            .TakeUntilDestroy(this)
            .Subscribe(x =>
            {
                if(!isGrounded)
                    MoveSpeed = jumpingMoveSpeed;
                else
                    MoveSpeed = groundedMoveSpeed;

                // そっち方向に移動する
                movement(x);
            });
        
        isChangeGravity
            .TakeUntilDestroy(this)
            .Where(x => x)
            .Subscribe(async x =>
            {
                // １秒待つ
                await UniTask.Delay(1000);

                // 重力変化
                if(targetGravityBox)
                    //対象の重力変化
                    MethodFactory.ChangeGravity(targetGravityBox);
                else
                {
                    //自身の重力変化
                    MethodFactory.ChangeGravity(this.gameObject);
                    IsGrounded = !IsGrounded;
                }
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
        moveDirection.x = moveValue * MoveSpeed;
    }

    // ジャンプ
    private void jump()
    {
        if(GetComponent<Rigidbody2D>().gravityScale != 0)
            this.GetComponent<Rigidbody2D>().velocity = Vector3.up * jumpForce;
        else
            this.GetComponent<Rigidbody2D>().velocity = Vector3.down * jumpForce;
    }


}
