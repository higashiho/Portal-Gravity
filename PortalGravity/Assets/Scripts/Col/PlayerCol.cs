using UnityEngine;

public class PlayerCol : MonoBehaviour
{
    private PlayerController player = default;

    private void OnCollisionStay2D(Collision2D other) 
    {
        player ??= this.GetComponent<PlayerController>();

        if(other.gameObject.tag == "Ground" && !player.IsGrounded)
        {
            //playerの重力加速度がない状態で上で当たっていたら
            if(player.GetComponent<Rigidbody2D>().gravityScale == 0)
            {
                if(MethodFactory.GetColDir(other) == Enums.ColDir.UP)
                {
                    player.IsGrounded = !player.IsGrounded;
                    player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                } 
            }
            //playerの重力加速度がある状態で下に当たっていたら
            else
            {
                if(MethodFactory.GetColDir(other) == Enums.ColDir.DOWN)
                {
                    player.IsGrounded = !player.IsGrounded;
                    player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                }
            }
        }
    }
    private void OnCollisionExit2D(Collision2D other) 
    {
        player.IsGrounded = false;
    }
}
