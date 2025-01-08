using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ShopStall : MonoBehaviour
{
    [SerializeField] GameObject shopStall;
    //[SerializeField] Animator shopStallAnim;
    [SerializeField] Animator circleFade;
    [SerializeField] GameObject cameraZoom;
    public bool shoppingTime = false;

    void Start()
    {
        //StartCoroutine(StallTime());
    }
    public IEnumerator StallTime()
    {
        circleFade.gameObject.SetActive(true);
        shopStall.SetActive(true);
        yield return new WaitForSeconds(3.5f);
        CameraManager.instance.ShopStallFocus();
        cameraZoom.SetActive(true);
        circleFade.SetTrigger("FadeIn");
        yield return new WaitForSeconds(3f);
        cameraZoom.SetActive(false);
        shopStall.SetActive(false);
        shoppingTime = true;
        circleFade.SetTrigger("FadeOut");
        yield return new WaitForSeconds(1f);
        circleFade.gameObject.SetActive(false);
    }
}
