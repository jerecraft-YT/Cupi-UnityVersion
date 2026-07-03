using UnityEngine;
using System.Collections.Generic;

public class PoolController : MonoBehaviour
{
    public static PoolController instance;
    [SerializeField] private List<PrefabNote> Prefabs;
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

    private int GetPrefabIndex(TipoObjetoPool tipoObjetoPool)
    {
        for (int index = 0; index < Prefabs.Count; index++)
        {
            if (Prefabs[index].tipoObjetoPool == tipoObjetoPool)
            {
                return index;
            }
        }
        return -1;
    }

    private GameObject SearchActiveInstance(int index)
    {
        foreach (GameObject objectPool in Instances[index])
        {
            if (!objectPool.activeSelf)
            {
                objectPool.SetActive(true);
                return objectPool;
            }
        }

        //print($"no hay instancias suficientes de {tipoNota}, se creara una nueva");

        AddInstance(index);
        GameObject newObject = Instances[index][Instances[index].Count - 1];
        newObject.SetActive(true);

        return newObject;
    }

    private void AddInstance(int indexPrefab)
    {
        if (Prefabs[indexPrefab].prefab == null) return;

        GameObject instance = Instantiate(Prefabs[indexPrefab].prefab, groupPool[indexPrefab].transform);
        instance.SetActive(false);
        Instances[indexPrefab].Add(instance);
    }

    private void AddGroupPool(int indexPrefab)
    {
        if (Prefabs[indexPrefab].prefab == null) return;

        GameObject groupInstance = new GameObject();
        groupInstance.transform.SetParent(transform);
        groupInstance.name = Prefabs[indexPrefab].tipoObjetoPool.ToString();
        groupPool.Add(groupInstance);
    }

    private void InstancePrefabs()
    {
        if (Prefabs.Count == 0)
        {
            Debug.LogError("no hay prefabs establecidos");
            return;
        }

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
    public GameObject RequestInstance(TipoObjetoPool tipoObjetoPool)
    {
        int index = GetPrefabIndex(tipoObjetoPool);

        if (index == -1)
        {
            Debug.LogError($"instancia  {tipoObjetoPool} no existe en la pool actual");
            return null;
        }
        return SearchActiveInstance(index);
    }

    public GameObject RequestGroupPool(TipoObjetoPool tipoObjetoPool)
    {
        int index = GetPrefabIndex(tipoObjetoPool);

        if (index == -1)
        {
            Debug.LogError($"instancia  {tipoObjetoPool} no existe en la pool actual");
            return null;
        }
        return groupPool[index];
    }
}
