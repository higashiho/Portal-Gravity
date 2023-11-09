using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LeverCol : MonoBehaviour
{
    [SerializeField, Tooltip("消すバリア対象")]
    private GameObject targetBarrier = default;

    private bool onBarrierBrack = true;

    private void Start()
    {
        targetBarrier = GameObject.FindWithTag("Barriers");
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {

        if(!onBarrierBrack) return;


        if(other.gameObject.tag == "Player" && other.GetComponent<PlayerController>().IsGrounded)
        {
            
            for(int i = targetBarrier.transform.childCount - 1; i > -1; i--)
            {
                if(targetBarrier.transform.GetChild(i).gameObject.activeSelf)
                {
                    targetBarrier.transform.GetChild(i).gameObject.SetActive(false);
                    onBarrierBrack = false;
                    break;
                }
            }

            var moveChild = this.transform.GetChild(1);

            moveChild.DORotate(moveChild.eulerAngles - Vector3.forward * 90f, 2).SetEase(Ease.Linear).SetLink(this.gameObject);
        }    
    }
}
