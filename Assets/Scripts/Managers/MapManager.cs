using UnityEngine;
using UnityEngine.TextCore.Text;
public class MapManager : MonoBehaviour
{
    public static MapManager instance;
    public GameObject prefabToSpawn;
    public Transform location;
    public Transform[] startPositions;
    [SerializeField] GameObject[] warmupProtectors;
    private int protectorIndex;
    [HideInInspector] public bool warmupRound;
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

    public void Warmup()
    {
        if (!warmupRound)
        {
            CharacterControl[] characters = GameObject.FindObjectsOfType<CharacterControl>();
            foreach (CharacterControl character in characters)
            {
                warmupProtectors[protectorIndex].SetActive(true);
                character.GetComponent<CharacterController>().enabled = false;
                protectorIndex++;
            }
            warmupRound = true;
        }
    }
}
