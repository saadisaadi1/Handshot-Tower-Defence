using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    void Start()
    {
        Invoke("DestroyObject", 2);
    }
    public void DestroyObject()
    {
        Destroy(gameObject);
    }
}
