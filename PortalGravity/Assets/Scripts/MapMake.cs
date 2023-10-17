using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
 
namespace map
{
    public class MapMake
    {

    [SerializeField, Tooltip("ステージ生成CSVファイル")]
    private List<string> fileName = new List<string>();

    [SerializeField, Tooltip("CSVファイルのリストの何番目か")]
    private int waveNum = 0;
    
    // 生成するCSVファイル
    private TextAsset csvFile;

    // 読み込まれた文字
    private List<string[]> waveDate = new List<string[]>();


    //[SerializeField]
    //private GameObject player = null;

    [SerializeField, Tooltip("ステージに登場するアイテム・床・仕掛けの数")]
    private GameObject[] stageItems = new GameObject[10];


    public void CSVload(int fileNum)
    {
        csvFile = Resources.Load(fileName[fileNum]) as TextAsset;
        
        StringReader reader = new StringReader(csvFile.text);    
        
        // scvの空白まで読み込む
        while (reader.Peek() != -1)
        {
            // 文字を読み込む
            string line = reader.ReadLine();

            // 「 , 」が読み込まれたら、そこまでの文字を１文字とする 
            waveDate.Add(line.Split(','));
        }

        spawn();
    }

        void spawn()
        {
            for (int row = 0; row < waveDate.Count; row++)
            {
                for (int col = 0; col < Constant.MAX_COL; col++)
                {
                    // Noneなら何も置かない
                    if(waveDate[row][col] == "None") continue;

                    // 何を置くかを読み取る
                    int num = int.Parse(waveDate[row][col]);

                    // 設置座標
                    Vector2 spanPos = new Vector2(col - 8.5f, 5f - row);

                    // 設置
                    MonoBehaviour.Instantiate(stageItems[num], spanPos, Quaternion.identity);

                }
            }       
        }
    }
}
