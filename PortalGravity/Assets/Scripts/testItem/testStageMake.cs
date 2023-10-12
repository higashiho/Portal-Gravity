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
       }

       StartCoroutine(spawn());
   }

    IEnumerator spawn()
   {
       for (int line = 0; line < waveDate.Count; line++)
       {
           for (int column = 0; column < maxColumn; column++)
           {
               int num = int.Parse(waveDate[line][column]);
               if (num >= 0)
               {
                   
                   Vector2 spanPos = new Vector2(column - 8.4f, line - 4.5f);

                   Instantiate(stagePrefab, spanPos, Quaternion.identity);
               }
           }

           yield return new WaitForSeconds(0.5f);
       }
       
   }
}