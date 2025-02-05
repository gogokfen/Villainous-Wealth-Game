using UnityEngine;
using UnityEngine.TextCore.Text;
public class MapManager : MonoBehaviour
{
    public static MapManager instance;
    public GameObject[] mapElements;
    public GameObject warmupMap;
    public Transform location;
    public Transform[] startPositions;
    [SerializeField] GameObject[] warmupProtectors;
    [HideInInspector] public bool warmupRound;

    [SerializeField] GameObject completePiratePackage;
    private void Awake()
    {
        instance = this;
    }
    public void ResetMap()
    {
        //Destroy(FindAnyObjectByType<QualityLootDestructable>().gameObject);

        DestroyMap.instance.DestroyMapElements();


        // copied  this shuffle loop logic from the net to randomize starting positions
        int n = startPositions.Length;
        while (n > 1)
        {
            int k = Random.Range(0,n--);
            Transform temp = startPositions[n];
            startPositions[n] = startPositions[k];
            startPositions[k] = temp;
        }


        int mapVariationSelected = Random.Range(0, 5); //range is 0->4, does not include this V6
        Instantiate(mapElements[mapVariationSelected]);

        

        if (mapVariationSelected == 0 || mapVariationSelected == 2)
            completePiratePackage.SetActive(true);

        //Instantiate(prefabToSpawn, location.position, location.rotation);

    }

    public void Warmup()
    {
        if (!warmupRound)
        {
            // for (int i =0;i<Leaderboard.singleton.playerCount;i++)
            //     //warmupProtectors[i].SetActive(true);

            Leaderboard.singleton.DisableCharacterController();

            warmupRound = true;

            DestroyMap.instance.DestroyMapElements();
            Instantiate(warmupMap);
        }
    }
}
