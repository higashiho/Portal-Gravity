using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakingGroundCol : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Player" && Mathf.Abs(other.gameObject.GetComponent<Rigidbody2D>().velocity.y) >= Mathf.Abs(Physics2D.gravity.y) - 1f)
        {
            other.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            this.gameObject.SetActive(false);
        }
    }
}
