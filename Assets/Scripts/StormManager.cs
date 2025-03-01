using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StormManager : MonoBehaviour
{
    [SerializeField] GameObject closingStorm;
    private bool stormEnabled = false;
    [SerializeField] float closingTimer;
    private float timer;

    void Update()
    {
        if (timer > closingTimer || Input.GetKeyDown(KeyCode.Backspace))
        {
            stormEnabled = true;
            closingStorm.SetActive(true);
        }

        if (stormEnabled)
        {
            Vector3 newScale = new Vector3(closingStorm.transform.localScale.x / (1 + (Time.deltaTime*0.05f)), 1, closingStorm.transform.localScale.z / (1 + (Time.deltaTime*0.05f)));
            closingStorm.transform.localScale = newScale;
        }
        timer += Time.deltaTime;
    }
    public void ResetStorm()
    {
        closingStorm.SetActive(false);
        stormEnabled = false;
        closingStorm.transform.localScale = Vector3.one;
        timer = 0f;
    }
}
