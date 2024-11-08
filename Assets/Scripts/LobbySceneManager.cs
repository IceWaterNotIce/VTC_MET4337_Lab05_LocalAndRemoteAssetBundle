using UnityEngine;

public class LobbySceneManager : MonoBehaviour
{
    private BundleLoader bundleLoader;

    [SerializeField]
    private Transform characterSpawnPt;

    // Start is called before the first frame update
    void Start()
    {
        bundleLoader = GameObject.Find("BundleLoader").GetComponent<BundleLoader>();

        bundleLoader.GetPrefabFromBundles<GameObject>("model", "Character", UpdateCharacter);
    }

    private void UpdateCharacter(GameObject prefab)
    {
        GameObject.Instantiate(prefab, characterSpawnPt.position, characterSpawnPt.rotation, characterSpawnPt);
    }
}


