using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityBoxCol : MonoBehaviour
{
    private GravityBoxController gravityBox = default;
    private void OnCollisionEnter2D(Collision2D other) 
    {
        gravityBox ??= this.GetComponent<GravityBoxController>();

        if(!gravityBox.IsGrounded)
        {
            if(other.gameObject.tag == "Ground" || other.transform.parent.name == "Sting")
            {
                gravityBox.IsGrounded = !gravityBox.IsGrounded;
            }

        }
    }
}
