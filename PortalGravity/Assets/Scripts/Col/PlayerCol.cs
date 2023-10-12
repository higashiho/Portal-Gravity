using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCol : MonoBehaviour
{
    private PlayerController player = default;

    private void OnCollisionEnter2D(Collision2D other) 
    {
        player ??= this.GetComponent<PlayerController>();
            

        if(other.gameObject.name == "yuka" && !player.IsGrounded)
        {
            //playerの下で当たっていたら
            if(MethodFactory.GetColDir(other) == Enums.ColDir.DOWN)
            {
                Debug.Log("in");
                player.IsGrounded = !player.IsGrounded;
            }
        }
    }
}
