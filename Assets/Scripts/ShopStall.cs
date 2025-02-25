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
    //public bool shoppingTime = false;
    public bool fadingFirst;
    public bool fadingSecond;

    bool firstTimeTrigger;

    public IEnumerator StallTime()
    {
        if (fadingFirst) yield break;
        fadingFirst = true;
        circleFade.gameObject.SetActive(true);
        shopStall.SetActive(true);

        if (!firstTimeTrigger) 
        {
            yield return new WaitForSeconds(3.5f);
            firstTimeTrigger = true;
        }
        else yield return new WaitForSeconds(1.5f);

        CameraManager.instance.ShopStallFocus();
        cameraZoom.SetActive(true);
        circleFade.SetTrigger("FadeIn");
        yield return new WaitForSeconds(3f);
        cameraZoom.SetActive(false);
        shopStall.SetActive(false);
        fadingFirst = false;
        circleFade.SetTrigger("FadeOut");
        yield return new WaitForSeconds(1f);
        circleFade.gameObject.SetActive(false);
    }

    public IEnumerator StallTimeOver()
    {
        fadingSecond = true;
        circleFade.gameObject.SetActive(true);
        circleFade.SetTrigger("FadeIn");
        yield return new WaitForSecondsRealtime(3f);
        fadingSecond = false;
        circleFade.SetTrigger("FadeOut");
        yield return new WaitForSecondsRealtime(1f);
        circleFade.gameObject.SetActive(false);
    }
}
