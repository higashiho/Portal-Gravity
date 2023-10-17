using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEditor.EditorTools;
using UnityEngine;

public class BaseMap : MonoBehaviour
{
    // 現在のplayerがいるステージ
    private ReactiveProperty<Enums.MapNum> updateMapNum = new ReactiveProperty<Enums.MapNum>();
    private ReactiveProperty<Enums.MapNum> UpdateMapNum => updateMapNum;
}
