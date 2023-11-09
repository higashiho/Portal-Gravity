using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakingGroundCol : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.tag == "Player" && other.gameObject.GetComponent<Rigidbody2D>().velocity.y <= Physics2D.gravity.y - 1.0f)
        {
            this.gameObject.SetActive(false);
        }
    }
}
