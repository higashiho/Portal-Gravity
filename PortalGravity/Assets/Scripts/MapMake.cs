using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
 
namespace map
{
    public class MapMake
    {
        // 読み込まれた文字
        private List<string[]> waveDate = new List<string[]>();

        // 生成するCSVファイル
        private TextAsset csvFile;

    public void CSVload(string fileName,GameObject[] instancObjects, GameObject parent)
    {
        csvFile = Resources.Load(fileName) as TextAsset;
        
        StringReader reader = new StringReader(csvFile.text);    
        
        // scvの空白まで読み込む
        while (reader.Peek() != -1)
        {
            // 文字を読み込む
            string line = reader.ReadLine();

            // 「 , 」が読み込まれたら、そこまでの文字を１文字とする 
            waveDate.Add(line.Split(','));
        }

        spawn(instancObjects, parent);
    }

        void spawn(GameObject[] instancObject, GameObject parent)
        {
            for (int row = 0; row < waveDate.Count; row++)
            {
                for (int col = 0; col < waveDate[row].Length; col++)
                {
                    // Noneなら何も置かない
                    if(waveDate[row][col] == "None") continue;

                    // 何を置くかを読み取る
                    int num = int.Parse(waveDate[row][col]);

                    // 設置座標
                    Vector2 spanPos = new Vector2(col - 8.5f, 5f - row);

                    // 設置
                    MonoBehaviour.Instantiate(instancObject[num], spanPos, Quaternion.identity, parent.transform);

                }
            }       
        }
    }
}
