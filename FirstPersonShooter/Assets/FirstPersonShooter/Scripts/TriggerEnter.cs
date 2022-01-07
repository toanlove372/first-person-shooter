using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerEnter : MonoBehaviour
{
    public event Action<GameObject> onEnter;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger enter: " + other.gameObject.name, other.gameObject);
        this.onEnter?.Invoke(other.gameObject);
    }
}
