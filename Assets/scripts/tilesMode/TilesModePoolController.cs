using UnityEngine;
using System.Collections.Generic;
using System;

public class TilesModePoolController : MonoBehaviour
{
    public List<GameObject> Prefabs;
    public int IntialInstances = 10;
    [SerializeField]
    public List<List<GameObject>> Instances;

    private void Start()
    {
        InstancePrefabs();
    }

    public GameObject RequestInstance(GameObject instance)
    {
        return instance;
    }

    private void InstancePrefabs()
    {
        Instances = new List<List<GameObject>>();
        Instances.Capacity = Prefabs.Count;
        print(Prefabs.Count);
        print(Instances.Count);
        for (int i = 0; i < Prefabs.Count; i++)
        {
            Instances.Add(new List<GameObject>());
            for (int j = 0; j < IntialInstances; j++)
            {
                var Instance = Instantiate(Prefabs[i],transform);
                Instance.SetActive(false);
                Instances[i].Add(Instance);
                print("hi");
            }
        }
    }
}
