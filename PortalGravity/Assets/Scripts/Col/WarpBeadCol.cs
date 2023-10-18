using Cysharp.Threading.Tasks;
using UnityEngine;

public class WarpBeadCol : MonoBehaviour
{

    // 生成座標のオフセット
    private Vector3 offset = new Vector3(0, 0.5f, 0);

    private void OnTriggerEnter2D(Collider2D other) 
    {

        if(other.gameObject.tag != "Player")
        {
            if(ObjectFactory.Instance == null || ObjectFactory.Instance.Player == null) return;
            
            ObjectFactory.Instance.WarpBeat.Resets(offset);

        }
    }
}
