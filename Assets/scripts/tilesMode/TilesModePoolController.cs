using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public class PrefabNote
{
    public GameObject prefab;
    public TipoNota tipoNota;
    public int IntialInstances;

    public PrefabNote(GameObject prefab,TipoNota tipoNota, int IntialInstances)
    {
        this.prefab = prefab;
        this.tipoNota = tipoNota;
        this.IntialInstances = IntialInstances;
    }
}

public class TilesModePoolController : MonoBehaviour
{
    public static TilesModePoolController instance;
    public List<PrefabNote> Prefabs;
    private List<List<GameObject>> Instances = new List<List<GameObject>>();
    private List<GameObject> groupPool = new List<GameObject>();

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        InstancePrefabs();
    }


    public GameObject RequestInstance(TipoNota notaGet)
    {
        int index = GetPrefabIndex(notaGet);

        if (index == -1)
        {
            Debug.LogError($"instancia  {notaGet} no existe en la pool actual");
            return null;
        }
        return SearchActiveInstance(index,notaGet);
    }

    public GameObject RequestGroupPool(TipoNota notaGet)
    {
        int index = GetPrefabIndex(notaGet);

        if (index == -1)
        {
            Debug.LogError($"instancia  {notaGet} no existe en la pool actual");
            return null;
        }
        return groupPool[index];
    }

    private int GetPrefabIndex(TipoNota notaGet)
    {
        for (int index = 0; index < Prefabs.Count; index++)
        {
            if (Prefabs[index].tipoNota == notaGet)
            {
                return index;
            }
        }
        return -1;
    }

    private GameObject SearchActiveInstance(int index,TipoNota tipoNota)
    {
        foreach (GameObject objectPool in Instances[index])
        {
            if (!objectPool.activeSelf)
            {
                objectPool.SetActive(true);
                return objectPool;
            }
        }

        print($"no hay instancias suficientes de {tipoNota}, se creara una nueva");

        AddInstance(index);
        GameObject newObject = Instances[index][Instances[index].Count - 1];
        newObject.SetActive(true);

        return newObject;
    }

    private void AddInstance(int indexPrefab)
    {
        if (Prefabs[indexPrefab].prefab == null) return;

        var instance = Instantiate(Prefabs[indexPrefab].prefab, groupPool[indexPrefab].transform);
        instance.SetActive(false);
        Instances[indexPrefab].Add(instance);
    }

    private void AddGroupPool(int indexPrefab)
    {
        GameObject groupInstance = new GameObject();
        groupInstance.transform.SetParent(transform);
        groupInstance.name = Prefabs[indexPrefab].tipoNota.ToString();
        groupPool.Add(groupInstance);
    }

    private void InstancePrefabs()
    {

        for (int i = 0; i < Prefabs.Count; i++)
        {
            Instances.Add(new List<GameObject>());

            AddGroupPool(i);

            for (int j = 0; j < Prefabs[i].IntialInstances; j++)
            {
                if (Prefabs[i].prefab == null) break;

                AddInstance(i);
            }
        }
    }
}
