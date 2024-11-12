using UnityEngine;
public class MapManager : MonoBehaviour
{
    public static MapManager instance;
    public GameObject prefabToSpawn;
    public Transform location;
    private void Awake()
    {
        instance = this;
    }
    public void ResetMap()
    {
        //Vector3 position = prefabToSpawn.transform.position;
        //Quaternion rotation = prefabToSpawn.transform.rotation;
        Destroy(FindAnyObjectByType<QualityLootDestructable>().gameObject);
        Instantiate(prefabToSpawn, location.position, location.rotation);
        Debug.Log("i triggered");
    }
}
