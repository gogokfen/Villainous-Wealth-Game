using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StormManager : MonoBehaviour
{
    [SerializeField] GameObject closingStorm;
    bool stormEnabled = false;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time>80 || Input.GetKeyDown(KeyCode.Backspace))
        {
            stormEnabled = true;
            closingStorm.SetActive(true);
        }

        if (stormEnabled)
        {
            Vector3 newScale = new Vector3(closingStorm.transform.localScale.x / (1 + (Time.deltaTime*0.05f)), 1, closingStorm.transform.localScale.z / (1 + (Time.deltaTime*0.05f)));
            closingStorm.transform.localScale = newScale;

            //Camera.main.transform.LookAt(closingStorm.transform);
            Camera.main.transform.Translate(Vector3.forward * (Time.deltaTime/5f)); //2
        }
    }
}
