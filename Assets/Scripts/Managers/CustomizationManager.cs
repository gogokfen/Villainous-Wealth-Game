using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomizationManager : MonoBehaviour
{
    public static CustomizationManager instance;
    public int roundAmount = 2;
    private void Awake() 
    {
        DontDestroyOnLoad(gameObject);
        instance = this;
    }
}
