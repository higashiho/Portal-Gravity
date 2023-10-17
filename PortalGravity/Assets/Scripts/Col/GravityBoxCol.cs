using UnityEngine;

public class GravityBoxCol : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D other) 
    {

        if(!ObjectFactory.GravityBox.IsGrounded)
        {
            if(other.gameObject.tag == "Ground" || other.transform.parent.name == "Sting")
            {
                ObjectFactory.GravityBox.IsGrounded = !ObjectFactory.GravityBox.IsGrounded;
            }

        }
    }
}
