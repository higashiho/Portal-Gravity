using UnityEngine;

public class PlayerCol : MonoBehaviour
{

    private void OnCollisionStay2D(Collision2D other) 
    {
        if(other.gameObject.tag == "Ground" && !ObjectFactory.Player.IsGrounded)
        {
            //playerの重力加速度がない状態で上で当たっていたら
            if(ObjectFactory.Player.GetComponent<Rigidbody2D>().gravityScale == 0)
            {
                if(MethodFactory.GetColDir(other) == Enums.ColDir.UP)
                {
                    ObjectFactory.Player.IsGrounded = !ObjectFactory.Player.IsGrounded;
                    ObjectFactory.Player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                } 
            }
            //playerの重力加速度がある状態で下に当たっていたら
            else
            {
                if(MethodFactory.GetColDir(other) == Enums.ColDir.DOWN)
                {
                    ObjectFactory.Player.IsGrounded = !ObjectFactory.Player.IsGrounded;
                    ObjectFactory.Player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                }
            }
        }
    }
    private void OnCollisionExit2D(Collision2D other) 
    {
        ObjectFactory.Player.IsGrounded = false;
    }
}
