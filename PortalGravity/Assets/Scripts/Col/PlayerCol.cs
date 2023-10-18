using UnityEngine;

public class PlayerCol : MonoBehaviour
{

    private void OnCollisionStay2D(Collision2D other) 
    {
        if(other.gameObject.tag == "Ground" && !ObjectFactory.Player.IsGrounded)
        {
            //playerの重力加速度がない状態で上で当たっていたら
            if(ObjectFactory.Player.GetComponent<Rigidbody2D>().gravityScale == -1)
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
    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.transform.parent != null && other.gameObject.name == "Sting")
            ObjectFactory.Player.StageRetry();
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.transform.parent != null && other.transform.parent.gameObject.name == "Key")
        {
            other.gameObject.SetActive(false);
            ObjectFactory.Player.IsNextStages[(int)ObjectFactory.Map.UpdateMapNum.Value] = true;
        }   
    }
    private void OnCollisionExit2D(Collision2D other) 
    {
        ObjectFactory.Player.IsGrounded = false;
    }
}
