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

    // 画面領域の境界座標
    private static Vector3 screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
    // 画面内にいるか確認
    public static bool CheckOnCamera(GameObject checkingObject)
    {
        return checkingObject.transform.position.x < -screenBounds.x || checkingObject.transform.position.x > screenBounds.x ||
                checkingObject.transform.position.y < -screenBounds.y || checkingObject.transform.position.y > screenBounds.y;
    }
}
