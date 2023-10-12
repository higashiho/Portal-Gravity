using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCol : MonoBehaviour
{
    private PlayerController player = default;

    private void OnCollisionEnter2D(Collision2D other) 
    {
        player ??= this.GetComponent<PlayerController>();

        if(player.ColObject != other.gameObject)
               
        if(other.gameObject.tag == "Ground" && !player.IsGrounded)
        {
            player.ColObject = other.gameObject;
            //playerの下で当たっていたら
            if(MethodFactory.GetColDir(other) == Enums.ColDir.DOWN)
            {
                player.IsGrounded = !player.IsGrounded;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D other) 
    {
        if(other.gameObject.tag == "Ground")
            player.ColObject = null;
    }
}
