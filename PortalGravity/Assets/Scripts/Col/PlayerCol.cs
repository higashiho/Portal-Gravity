using UnityEngine;

public class PlayerCol : MonoBehaviour
{

    private void OnCollisionStay2D(Collision2D other) 
    {
        if(ObjectFactory.Instance == null || ObjectFactory.Instance.Player == null) return;
        
        if(other.gameObject.tag == "Ground" && !ObjectFactory.Instance.Player.IsGrounded ||
            other.gameObject.tag == "GravityBox" && !ObjectFactory.Instance.Player.IsGrounded ||
            other.gameObject.tag == "Spear" && !ObjectFactory.Instance.Player.IsGrounded  ||
            other.gameObject.tag == "GroundSting" && !ObjectFactory.Instance.Player.IsGrounded  
            )
        {
            //playerの重力加速度がない状態で上で当たっていたら
            if(ObjectFactory.Instance.Player.GetComponent<Rigidbody2D>().gravityScale == -1)
            {
                if(MethodFactory.GetColDir(other) == Enums.ColDir.UP)
                {
                    ObjectFactory.Instance.Player.IsGrounded = !ObjectFactory.Instance.Player.IsGrounded;
                    ObjectFactory.Instance.Player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                } 
            }
            //playerの重力加速度がある状態で下に当たっていたら
            else
            {
                if(MethodFactory.GetColDir(other) == Enums.ColDir.DOWN)
                {
                    ObjectFactory.Instance.Player.IsGrounded = !ObjectFactory.Instance.Player.IsGrounded;
                    ObjectFactory.Instance.Player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                }
            }
        }

        if(other.transform.parent != null && other.transform.parent.gameObject.tag == "Laser" && other.transform.parent.GetChild(1).gameObject.activeSelf)
        {
            for(int i = 0; i < other.transform.parent.GetChild(0).childCount; i++)
            {
                other.transform.GetChild(i).gameObject.SetActive(!other.transform.parent.GetChild(0).GetChild(i).gameObject.activeSelf);
            }
            other.transform.parent.GetChild(1).gameObject.SetActive(false);
        }

    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if(ObjectFactory.Instance == null || ObjectFactory.Instance.Player == null) return;
        
        if(other.transform.parent != null)
            ObjectFactory.Instance.Player.IsRetry.Value = other.transform.parent.gameObject.tag == "Sting" || 
                                            other.gameObject.tag == "GroundSting" && this.transform.position.y < other.transform.position.y ||
                                            other.gameObject.tag == "Spear"  && this.transform.position.y < other.transform.position.y
                                            ;
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if(ObjectFactory.Instance == null || ObjectFactory.Instance.Player == null) return;

        if(other.transform.parent != null && other.transform.parent.gameObject.tag == "Key")
        {
            other.transform.parent.gameObject.SetActive(false);
            ObjectFactory.Instance.Player.IsNextStages[(int)ObjectFactory.Instance.Map.UpdateMapNum.Value] = true;
        }   

        if(other.transform.parent != null)
            ObjectFactory.Instance.Player.IsRetry.Value = other.transform.parent.gameObject.tag == "Laser" || 
                                                other.transform.parent.gameObject.tag == "SmalSting";
        
    }
    private void OnCollisionExit2D(Collision2D other) 
    {
        if(ObjectFactory.Instance == null || ObjectFactory.Instance.Player == null) return;

        ObjectFactory.Instance.Player.IsGrounded = false;
    }
}
