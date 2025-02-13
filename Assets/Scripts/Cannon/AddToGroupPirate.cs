using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AddToGroupPirate : MonoBehaviour
{
    private void OnEnable()
    {
        if (SceneManager.GetActiveScene().name != "OsherScene")
            CameraManager.instance.AddToGroup(gameObject);
    }
}
