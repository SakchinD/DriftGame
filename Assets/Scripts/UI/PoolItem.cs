using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolItem : MonoBehaviour
{
    public string Name;
    public virtual void SetInfo(params string[] values) { }

    public void InvokeItem()
    {
        gameObject.SetActive(true);
    }

    public void ResetItem()
    {
        gameObject.SetActive(false);
    }
}
