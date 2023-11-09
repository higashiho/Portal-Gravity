using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LeverCol : MonoBehaviour
{
    [SerializeField, Tooltip("消すバリア対象")]
    private GameObject targetBarrier = default;

    private void OnTriggerEnter2D(Collider2D other) 
    {
        targetBarrier ??= GameObject.FindGameObjectWithTag("Barriers").transform.GetChild(1).gameObject;

        if(!targetBarrier || !targetBarrier.activeSelf) return;


        if(other.gameObject.tag == "Player" && ObjectFactory.Instance.Player.IsGrounded)
        {
            targetBarrier.SetActive(false);
            this.transform.GetChild(1).DOMoveY(-this.transform.position.y, 1).SetEase(Ease.Linear).SetLink(this.gameObject);
            this.transform.GetChild(1).DORotate(Vector3.forward * -this.transform.position.y, 1).SetEase(Ease.Linear).SetLink(this.gameObject);
            this.transform.GetChild(2).DOMoveY(-this.transform.position.y, 1).SetEase(Ease.Linear).SetLink(this.gameObject);
        }    
    }
}
