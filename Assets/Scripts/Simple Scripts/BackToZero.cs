using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackToZero : MonoBehaviour
{
    [SerializeField] GameObject objectToMove;

    public void Return()
    {
        objectToMove.transform.localPosition = Vector3.zero;
        Destroy(objectToMove.GetComponent<Rigidbody>()); //only for coinshot
    }
}
