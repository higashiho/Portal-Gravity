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
    private ReactiveProperty<float> isMoving = new ReactiveProperty<float>();

    [SerializeField, Tooltip("移動スピード")]
    private float moveSpeed = default;

    [SerializeField, Tooltip("ジャンプ力")]
    private float jumpForce = 5.0f;
    private bool isGrounded = true;
    public bool IsGrounded{get => isGrounded; set => isGrounded = value;}
    private Vector3 moveDirection;

    protected void setSubscribe()
    {
          this.UpdateAsObservable()
            .TakeUntilDestroy(this)
            .Subscribe(_ => 
            {
                isJumping.Value = Input.GetKeyDown(KeyCode.Space);
                isMoving.SetValueAndForceNotify(Input.GetAxis("Horizontal"));
            });
    }
    // 挙動
    protected void setMoveSubscribe()
    {

        isJumping
            // ボタンが押された時に、
            .Where(x => x)
            // 接地中であり、
            .Where(_ => IsGrounded)
            // 最後にジャンプしてから1秒以上経過しているなら、
            .ThrottleFirst(TimeSpan.FromSeconds(1))
            .Subscribe(_ =>
            {
                IsGrounded = false;
                // ジャンプ処理を実行する
                jump();
            });

                
        isMoving
            .Subscribe(x =>
            {
                // そっち方向に移動する
                    movement(x);
            });

           this.UpdateAsObservable()
            .TakeUntilDestroy(this)
            .Subscribe(_ => 
            { 
                this.transform.position += moveDirection * Time.deltaTime;
            });
    }
    
    //移動
    protected void movement(float moveValue)
    {
        moveDirection.x = moveValue * moveSpeed;
    }

    // ジャンプ
    protected void jump()
    {
        this.GetComponent<Rigidbody2D>().velocity = Vector3.up * jumpForce;
    }
}
