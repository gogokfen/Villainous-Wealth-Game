using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfter : MonoBehaviour
{
    [SerializeField] float destroyAfterXSeconds;
    void Start()
    {
        Destroy(gameObject, destroyAfterXSeconds);
    }
}
