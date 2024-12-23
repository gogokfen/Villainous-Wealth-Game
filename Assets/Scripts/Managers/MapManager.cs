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
        //Destroy(FindAnyObjectByType<QualityLootDestructable>().gameObject);
        DestroyMap.instance.DestroyMapElements();
        Instantiate(prefabToSpawn, location.position, location.rotation);
    }
}
