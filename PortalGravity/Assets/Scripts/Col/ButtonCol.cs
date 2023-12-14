using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.Mathematics;

public class ButtonCol : MonoBehaviour
{
    [SerializeField, Tooltip("変化対象オブジェクト")]
    private GameObject changeTagetObj;


    private void Start()
    {
        changeTagetObj = this.gameObject.tag switch
        {
            "RustyButton" => GameObject.FindWithTag("Barriers"),
            "Button" => GameObject.FindWithTag("Door"),
            _ => null,
        };
    }
    private void OnTriggerEnter2D(Collider2D other) 
    {

        if(!changeTagetObj || !changeTagetObj.activeSelf) return;

        switch(changeTagetObj.tag)
        {
            case "Door":
                if(other.gameObject.tag == "Player" && Mathf.Abs(other.gameObject.GetComponent<Rigidbody2D>().velocity.y) >= Mathf.Abs(Physics2D.gravity.y) - 1f)
                {
                    changeTagetObj.transform.DOMoveY(changeTagetObj.transform.position.y + changeTagetObj.transform.childCount, 1).SetEase(Ease.Linear).SetLink(this.gameObject).
                        OnStart(() => changeTagetObj = default);
                    this.gameObject.SetActive(false);
                }
            break;
            case "Barriers":
                if(other.gameObject.tag == "GravityBox" && this.gameObject.activeSelf)
                {
                    for(int i = changeTagetObj.transform.childCount - 1; i > -1; i--)
                    {
                        if(changeTagetObj.transform.GetChild(i).gameObject.activeSelf)
                        {
                            changeTagetObj.transform.GetChild(i).gameObject.SetActive(false);
                            break;
                        }
                    }
                    this.gameObject.SetActive(false);
                }
            break;
            default:
            break;
        }
    }
}
