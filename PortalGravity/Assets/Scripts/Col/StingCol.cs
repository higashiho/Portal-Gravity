using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StingCol : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D other) 
    {
        if(this.GetComponent<Rigidbody2D>().gravityScale == 0) return;

        if(other.gameObject.tag == "Ground")
        {
            this.gameObject.SetActive(false);
        }
    }
}
