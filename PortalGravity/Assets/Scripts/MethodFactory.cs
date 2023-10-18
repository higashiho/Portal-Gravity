using UnityEngine;

public class MethodFactory : MonoBehaviour
{
    // どの面で当たってるか確認用
    public static Enums.ColDir GetColDir(Collision2D other)
    {
        
        if (other.contacts.Length > 0)
        {
            // 衝突した点の法線ベクトルを取得
            Vector2 normal = other.contacts[0].normal;
            
            // float微ずれ修正
            normal.x = Mathf.RoundToInt(normal.x);
            normal.y = Mathf.RoundToInt(normal.y);

            // 衝突方向の判定
            if (normal == Vector2.up)
            {
                return Enums.ColDir.DOWN;
            }
            if (normal == Vector2.down)
            {
                return Enums.ColDir.UP;
            }
            if (normal == Vector2.right)
            {
                return Enums.ColDir.RIGHT;
            }
            if (normal == Vector2.left)
            {
                return Enums.ColDir.LEFT;
            }
        }
        return Enums.ColDir.DEFAULT;
    }

    
    // 重力反転
    public static void ChangeGravity(GameObject target)
    {

        if(target.GetComponent<Rigidbody2D>().gravityScale != -1f)
            target.GetComponent<Rigidbody2D>().gravityScale = -1f;
        else
            target.GetComponent<Rigidbody2D>().gravityScale = 1f;
    }

    // 画面内にいるか確認
    public static bool CheckOnCamera(GameObject checkingObject)
    {
        return checkingObject.transform.position.x < ObjectFactory.Player.RetryPos.x - 0.5f  ||
                checkingObject.transform.position.y < Camera.main.ScreenToWorldPoint(Vector3.down * Screen.height).y || 
                checkingObject.transform.position.y > Camera.main.ScreenToWorldPoint(Vector3.up * Screen.height).y;
    }
}
