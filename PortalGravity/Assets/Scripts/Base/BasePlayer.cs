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

    [SerializeField, Tooltip("移動スピード")]
    private float moveSpeed = default;

    [SerializeField, Tooltip("ジャンプ力")]
    private float jumpForce = 5.0f;
    private bool isGrounded = true;
    public bool IsGrounded{get => isGrounded; set => isGrounded = value;}
    private Vector3 moveDirection;

    [SerializeField, Tooltip("重力変化対象オブジェクト")]
    private GameObject targetGravityBox = default;

    [SerializeField, Tooltip("playerが触れているオブジェクト")]
    private GameObject colObject = default;
    public GameObject ColObject{get => colObject; set => colObject = value;}

    // 逆向きの重力スケール
    private float gravityScale = -1.0f; 

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
            .Subscribe(x =>
            {
                if(targetGravityBox)
                    //対象の重力変化
                    changeGravity(targetGravityBox);
                else
                    //自身の重力変化
                    changeGravity(this.gameObject);
            });

           this.UpdateAsObservable()
            .TakeUntilDestroy(this)
            .Subscribe(_ => 
            { 
                // 以下重力反転時==============================================================================
                if(targetGravityBox && targetGravityBox.GetComponent<Rigidbody2D>().gravityScale == 0f)
                {
                    moveDirection.y += Physics.gravity.y * Time.deltaTime;
                }
                if(this.GetComponent<Rigidbody2D>().gravityScale == 0f && ColObject == null)
                {
                    moveDirection.y += -Physics.gravity.y * Time.deltaTime;
                }
                // 以上重力反転時==============================================================================


                this.transform.position += moveDirection * Time.deltaTime;
            });
    }
    
    //移動
    private void movement(float moveValue)
    {
        moveDirection.x = moveValue * moveSpeed;
    }

    // ジャンプ
    private void jump()
    {
        if(GetComponent<Rigidbody2D>().gravityScale != 0)
            this.GetComponent<Rigidbody2D>().velocity = Vector3.up * jumpForce;
        else
            this.GetComponent<Rigidbody2D>().velocity = Vector3.down * jumpForce;
    }


    // 重力反転
    private void changeGravity(GameObject target)
    {
        target.GetComponent<Rigidbody2D>().velocity = Vector3.up * jumpForce;
        target.GetComponent<Rigidbody2D>().gravityScale = 0f;
    }
}
