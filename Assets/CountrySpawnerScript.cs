using System.Collections;
using UnityEngine;

public class CountrySpawnerScript : MonoBehaviour
{
    [Header("Script References")]
    public GameObject CountryPrefab;
    public int StartAmount;

    [Header("GridSystem")]
    public GridManagerScript GridScript;

    private void Start()
    {
        StartCoroutine(StartDelay());
    }

    GameObject GetRandomTile()
    {
        int RandomIndex = UnityEngine.Random.Range(0, GridScript.Grids.Count);
        TileScript GridTileScript = GridScript.Grids[RandomIndex].GetComponent<TileScript>();

        if (GridTileScript.isBuildable)
            return GridScript.Grids[RandomIndex];

        return GetRandomTile();
    }

    private void SpawnCountry(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            Instantiate(CountryPrefab, GetRandomTile().transform.position, Quaternion.identity);
        }
    }

    IEnumerator StartDelay()
    {
        yield return new WaitForSeconds(1);
        SpawnCountry(StartAmount);
    }
}
