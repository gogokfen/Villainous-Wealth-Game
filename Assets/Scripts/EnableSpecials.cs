using UnityEngine;
public class EnableSpecials : MonoBehaviour
{
    [SerializeField] GameObject specialPickups;
    void Start()
    {
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            specialPickups.SetActive(true);
        }
    }
}
