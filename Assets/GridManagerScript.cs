using System.Collections.Generic;
using UnityEngine;

public class GridManagerScript : MonoBehaviour
{
    [Header("Grid size")]
    public int width;
    public int height;
    public int CellSize;

    [Header("Script References")]
    public GameObject cell;
    public List<GameObject> Grids = new List<GameObject>();

    private void Start()
    {
        for (int i = 0; i < width; i+=CellSize) 
        {
            for (int j = 0; j < height; j+=CellSize)
            {
                GameObject Tile = Instantiate(cell, new Vector3(transform.position.x+i, 1, transform.position.z+j), Quaternion.identity);
                Tile.transform.parent = transform;
                Grids.Add(Tile);
            }
        }
    }
}
