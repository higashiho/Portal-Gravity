using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
 
public class testStageMake : MonoBehaviour
{
   [SerializeField]
   private List<string> fileName = new List<string>();

    [SerializeField]
   private int waveNum = 0;
   
   private TextAsset csvFile;

   private List<string[]> waveDate = new List<string[]>();

   [SerializeField]
   private int maxCol = 18;

   //[SerializeField]
   //private GameObject player = null;

    [SerializeField]
   private GameObject[] stageItems = new GameObject[10];


   void Start()
   {
       CSVload();
   }

   void CSVload()
   {
       csvFile = Resources.Load(fileName[waveNum]) as TextAsset;
       
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
            for (int col = 0; col < maxCol; col++)
            {
                // Noneなら何も置かない
                if(waveDate[row][col] == "None") continue;

                // 何を置くかを読み取る
                int num = int.Parse(waveDate[row][col]);

                // 設置座標
                Vector2 spanPos = new Vector2(col - 8.5f, 5f - row);

                // 設置
                Instantiate(stageItems[num], spanPos, Quaternion.identity, this.transform);

            }
        }       
   }
}