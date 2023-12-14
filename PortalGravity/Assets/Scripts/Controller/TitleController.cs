using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // シーン管理用のネームスペースを追加

public class TitleController : MonoBehaviour
{
    [SerializeField]
    private GameObject tipObject;
    [SerializeField]
    private GameObject titleObject;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // エンターキーが押されたとき
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            // InGameSceneに移動
            SceneManager.LoadScene("InGame");
        }

        if(Input.GetKeyDown(KeyCode.Tab))
        {
            tipObject.SetActive(!tipObject.activeSelf);
            titleObject.SetActive(!titleObject.activeSelf);
        }
    }
}