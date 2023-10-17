using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEditor.EditorTools;
using UnityEngine;

public class BaseMap : MonoBehaviour
{
    // 現在のplayerがいるステージ
    private ReactiveProperty<Enums.MapNum> updateMapNum = new ReactiveProperty<Enums.MapNum>(Enums.MapNum.STAGE_1);
    private ReactiveProperty<Enums.MapNum> UpdateMapNum => updateMapNum;

    protected void setSubscribe()
    {
        UpdateMapNum
            .TakeUntilDestroy(this)
            .Where(x => x != Enums.MapNum.DEFAULT)
            .Subscribe(x => 
            {


            });
    }

    public void NextMaps()
    {
        UpdateMapNum.Value++;
    }
}
