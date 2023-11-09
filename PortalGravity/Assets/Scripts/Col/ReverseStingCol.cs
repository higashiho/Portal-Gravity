using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReverseStingCol : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D other) 
    {
        if(other.transform.parent.parent != null && other.transform.parent.parent.tag == "Barriers")
        {
            other.transform.parent.gameObject.SetActive(false);
            this.gameObject.SetActive(false);
        }
    }
}
