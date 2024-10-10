using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QualityLoot : MonoBehaviour
{
    //pickup layer is 6, character layer is 8

    [SerializeField] Slider pickupChannel;
    [SerializeField] Image  fillColor;

    bool full = false;
    void Start()
    {
        pickupChannel.value = 0;
    }

    void Update()
    {
        if (pickupChannel.value==1)
        {
            full = true;
            gameObject.layer = 6;
            fillColor.color = Color.yellow;
        }
    }


    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 8)
        {
            pickupChannel.value += Time.deltaTime / 5;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 8 && !full)
        {
            pickupChannel.value = 0;
        }
    }
}
