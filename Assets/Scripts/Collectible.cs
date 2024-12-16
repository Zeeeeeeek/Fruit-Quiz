using System;
using UnityEngine;
using UnityEngine.Events;

public class Collectible : MonoBehaviour
{
    
    public int index;
    public UnityEvent<int> onCollect;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        Destroy(gameObject);
        onCollect.Invoke(index);
    }
}
