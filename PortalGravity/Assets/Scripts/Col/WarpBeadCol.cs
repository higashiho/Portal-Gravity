using UnityEngine;

public class WarpBeadCol : MonoBehaviour
{

    // 生成座標のオフセット
    private Vector3 offset = new Vector3(0, 0.5f, 0);

    private void OnTriggerEnter2D(Collider2D other) 
    {

        if(other.gameObject.tag != "Player")
        {
            ObjectFactory.Player.transform.position = this.transform.position;
            ObjectFactory.Player.transform.position +=  ObjectFactory.Player.GetComponent<Rigidbody2D>().gravityScale != 0 ? offset : -offset;

            this.gameObject.SetActive(false);
        }
    }
}
