using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class FactoryObject : MonoBehaviour
{
    public int MaxWorkers = 3;
    public List<GameObject> workers = new List<GameObject>();

    private void Update()
    {
        for (int i = 0; i < workers.Count; i++)
        {
            if (workers[i] == null) workers.Remove(workers[i]);
        }
    }
}
