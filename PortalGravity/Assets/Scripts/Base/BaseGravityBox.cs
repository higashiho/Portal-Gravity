using UnityEngine;

public class BaseGravityBox : MonoBehaviour
{
    [SerializeField, Tooltip("重力変化フラグ")]
    private bool isChangeGravityFlag = false;
    public bool IsChangeGravityFlag{get => isChangeGravityFlag; set => isChangeGravityFlag = value;}

    [SerializeField, Tooltip("地面に触れているか")]
    private bool isGrounded = true;
    public bool IsGrounded{get => isGrounded; set => isGrounded = value;}
    [SerializeField, Tooltip("次フレームの移動追加値")]
    private Vector3 moveDirection;

    protected void setSubscribe()
    {
    }
}
