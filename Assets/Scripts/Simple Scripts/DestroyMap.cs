using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyMap : MonoBehaviour
{
    public static DestroyMap instance;
    private void Awake() 
    {
        instance = this;
    }

    public void DestroyMapElements()
    {
        Destroy(gameObject);
    }
}
