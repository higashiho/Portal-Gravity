using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using map;
using UniRx;
using UnityEditor.EditorTools;
using UnityEngine;

public class BaseMap : MonoBehaviour
{

    [SerializeField, Tooltip("ステージに登場するアイテム・床・仕掛けの数")]
    private GameObject[] stageItems = new GameObject[30];

    [SerializeField, Tooltip("ステージ生成CSVファイル")]
    private List<string> fileName = new List<string>();

    private MapMake make = new MapMake();

    // 現在のplayerがいるステージ
    private ReactiveProperty<Enums.MapNum> updateMapNum = new ReactiveProperty<Enums.MapNum>(Enums.MapNum.STAGE_1);
    public ReactiveProperty<Enums.MapNum> UpdateMapNum => updateMapNum;

    protected void setSubscribe()
    {
        UpdateMapNum
            .TakeUntilDestroy(this)
            .Where(x => x != Enums.MapNum.DEFAULT)
            .Subscribe(x => 
            {
                make.CSVload(fileName[(int)x], stageItems, this.gameObject);

            });
    }

    // 次のステージのステートにすることで、次のステージを生成する
    public void NextMaps()
    {
        UpdateMapNum.Value++;
    }

    // 前のステージのオブジェクトを非アクティブにする
    public void DeleteStageObject()
    {
        foreach(Transform stageObject in this.gameObject.transform)
        {
            stageObject.gameObject.SetActive(false);
        }
    }
}
