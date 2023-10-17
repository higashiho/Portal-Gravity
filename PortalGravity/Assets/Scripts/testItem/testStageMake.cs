using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
 
public class testStageMake : MonoBehaviour
{
    [SerializeField]
   private List<string> fileName = new List<string>();
   int waveNum = 0;
   TextAsset csvFile;
   List<string[]> waveDate = new List<string[]>();
   [SerializeField]
   private int maxColumn = 10;
   [SerializeField]
   private int maxLine = 10;

   [SerializeField]
   private GameObject stagePrefab = null;

   void Start()
   {
       CSVload();
   }

   void CSVload()
   {
       csvFile = Resources.Load(fileName[waveNum]) as TextAsset;
       
       StringReader reader = new StringReader(csvFile.text);

       while (reader.Peek() != -1)
       {
           string line = reader.ReadLine();
           waveDate.Add(line.Split(','));

        spawn();
           
       }

       
   }

    void spawn()
   {
       for (int line = waveDate.Count; line < 0; line--)
       {
           for (int column = 0; column < maxColumn; column++)
           {
               int num = int.Parse(waveDate[line][column]);

               Debug.Log(num);

               Vector2 spanPos = new Vector2(column - 8.5f, line - waveDate.Count + 5.0f);
               
               if (num == 1)
               {
                   Instantiate(stagePrefab, spanPos, Quaternion.identity);
               }
           }
       }
       
   }
}