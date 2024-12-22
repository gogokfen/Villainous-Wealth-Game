using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StormManager : MonoBehaviour
{
    [SerializeField] GameObject closingStorm;
    bool stormEnabled = false;
    [SerializeField] float closingTimer;
    private float startingTimer;
    private float timer;
    void Start()
    {
        startingTimer = closingTimer;
    }

    // Update is called once per frame
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

            //Camera.main.transform.LookAt(closingStorm.transform);
            //Camera.main.transform.Translate(Vector3.forward * (Time.deltaTime/5f)); //2
        }
        timer += Time.deltaTime;
    }
    public void ResetStorm()
    {
        closingStorm.SetActive(false);
        stormEnabled = false;
        closingStorm.transform.localScale = Vector3.one;
        //closingTimer = Time.time + startingTimer;
        timer = 0f;
    }
}
