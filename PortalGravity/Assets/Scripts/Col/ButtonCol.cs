using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ButtonCol : MonoBehaviour
{
    [SerializeField, Tooltip("変化対象オブジェクト")]
    private GameObject changeTagetObj;
    private void OnTriggerEnter2D(Collider2D other) 
    {
        changeTagetObj ??= this.gameObject.tag switch
        {
            "RustyButton" => GameObject.FindGameObjectWithTag("Barriers").transform.GetChild(0).gameObject,
            "Button" => GameObject.FindGameObjectWithTag("Barriers").transform.GetChild(2).gameObject,
            _ => null,
        };

        if(!changeTagetObj || !changeTagetObj.activeSelf) return;

        switch(changeTagetObj.tag)
        {
            case "Door":
                if(other.gameObject.tag == "Player" && other.gameObject.GetComponent<Rigidbody2D>().velocity.y <= Physics2D.gravity.y - 1.0f)
                {
                    changeTagetObj.transform.DOMoveY(Vector3.up.y, 1).SetEase(Ease.Linear).SetLink(this.gameObject).
                        OnStart(() => changeTagetObj = default);
                    this.gameObject.SetActive(false);
                }
            break;
            case "Barrier":
                if(other.gameObject.tag == "GravityBox")
                {
                    changeTagetObj.SetActive(false);
                    this.gameObject.SetActive(false);
                }
            break;
            default:
            break;
        }
    }
}
