using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TmpMove : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var tmpPos = Vector3.zero;
        var speedPlus = 1;
        if (Input.GetKey(KeyCode.Space))

            speedPlus = 2;
        if (Input.GetKey(KeyCode.W))
            tmpPos = Camera.main.transform.forward;
        if (Input.GetKey(KeyCode.A))
            tmpPos = -Camera.main.transform.right;
        if (Input.GetKey(KeyCode.S))
            tmpPos = -Camera.main.transform.forward;
        if (Input.GetKey(KeyCode.D))
            tmpPos = Camera.main.transform.right;  


        var camera = Camera.main.transform.eulerAngles;
        if (Input.GetKey(KeyCode.UpArrow))
            camera.x -= 100 * Time.deltaTime;
        if (Input.GetKey(KeyCode.LeftArrow))
            camera.y -= 100 * Time.deltaTime;
        if (Input.GetKey(KeyCode.DownArrow))
            camera.x += 100 * Time.deltaTime;
        if (Input.GetKey(KeyCode.RightArrow))
            camera.y += 100 * Time.deltaTime;


        this.transform.position += tmpPos.normalized * (7 * speedPlus) * Time.deltaTime;

        Camera.main.transform.eulerAngles = camera;

    }
}
