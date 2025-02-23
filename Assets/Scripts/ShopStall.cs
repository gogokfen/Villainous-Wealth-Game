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
        Debug.Log("debug1");
        circleFade.gameObject.SetActive(true);
        Debug.Log("debug2");
        circleFade.SetTrigger("FadeIn");
        Debug.Log("debug3");
        yield return new WaitForSecondsRealtime(3f);
        Debug.Log("debug4");
        fadingSecond = false;
        Debug.Log("debug5");
        circleFade.SetTrigger("FadeOut");
        Debug.Log("debug6");
        yield return new WaitForSecondsRealtime(1f);
        Debug.Log("debug7");
        circleFade.gameObject.SetActive(false);
        Debug.Log("debug8");
    }
}
