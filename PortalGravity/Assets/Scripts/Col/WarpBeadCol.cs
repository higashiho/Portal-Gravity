using UnityEngine;

public class WarpBeadCol : MonoBehaviour
{
    private PlayerController player;
    private WarpBeatController warpBeat;

    // 生成座標のオフセット
    private Vector3 offset = new Vector3(0, 0.5f, 0);

    private void OnTriggerEnter2D(Collider2D other) 
    {
        player ??= this.transform.parent.GetComponent<PlayerController>();
        warpBeat ??= this.GetComponent<WarpBeatController>();

        if(other.gameObject.tag != "Player")
        {
            player.transform.position = this.transform.position;
            player.transform.position += player.GetComponent<Rigidbody2D>().gravityScale != 0 ? offset : -offset;

            this.gameObject.SetActive(false);
        }
    }
}
