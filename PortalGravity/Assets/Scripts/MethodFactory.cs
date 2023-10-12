using System.Collections;
using System.Collections.Generic;
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
            // 衝突方向の判定
            if (normal == Vector2.up)
            {
                return Enums.ColDir.DOWN;
            }
            else if (normal == Vector2.down)
            {
                return Enums.ColDir.UP;
            }
            else if (normal == Vector2.right)
            {
                return Enums.ColDir.RIGHT;
            }
            else if (normal == Vector2.left)
            {
                return Enums.ColDir.LEFT;
            }
        }
        return Enums.ColDir.DEFAULT;
    }
}
