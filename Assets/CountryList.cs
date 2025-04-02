using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class CountryList : MonoBehaviour
{
    public List<GameObject> Countries = new List<GameObject>();
    [Header("Deaths")]
    public int worldDeaths = 0;
    public List<string> names = new List<string>();
}
