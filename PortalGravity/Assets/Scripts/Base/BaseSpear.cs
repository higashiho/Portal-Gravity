using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class BaseSpear : MonoBehaviour
{
    private ReactiveProperty<bool> isFall = new ReactiveProperty<bool>();
    public ReactiveProperty<bool> IsFall => isFall;
    protected void setSubscribe()
    {
        IsFall
            .TakeUntilDestroy(this)
            .Where(x => x)
            .Subscribe(_ => 
            {
                this.GetComponent<Rigidbody2D>().gravityScale = 1;
            });
    }
}
