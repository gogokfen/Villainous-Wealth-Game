using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemovePickup : MonoBehaviour
{
    [SerializeField] bool isCoinSackPickup;
    void Start()
    {
        Destroy(gameObject, 20);
    }

    private void OnDestroy()
    {
        if (isCoinSackPickup)
            PickupManager.singleton.RemovePickupFromList(gameObject);
        else
            PickupManager.singleton.RemovePickupFromPickups(gameObject);
    }

}
