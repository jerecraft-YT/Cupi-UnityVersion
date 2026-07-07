using UnityEngine;
using System.Collections.Generic;

public class PoolController : MonoBehaviour
{
    public static PoolController instance;

    [SerializeField] private List<PrefabNote> _prefabs;
    private List<List<GameObject>> _instances = new();
    private List<GameObject> _groupPool = new();

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
        for (int index = 0; index < _prefabs.Count; index++)
        {
            if (_prefabs[index].tipoObjetoPool == tipoObjetoPool)
            {
                return index;
            }
        }
        return -1;
    }

    private GameObject SearchActiveInstance(int index)
    {
        foreach (GameObject objectPool in _instances[index])
        {
            if (!objectPool.activeSelf)
            {
                objectPool.SetActive(true);
                return objectPool;
            }
        }

        //print($"no hay instancias suficientes de {tipoNota}, se creara una nueva");

        AddInstance(index);
        GameObject newObject = _instances[index][_instances[index].Count - 1];
        newObject.SetActive(true);

        return newObject;
    }

    private void AddInstance(int indexPrefab)
    {
        if (_prefabs[indexPrefab].prefab == null) return;

        GameObject instance = Instantiate(_prefabs[indexPrefab].prefab, _groupPool[indexPrefab].transform);
        instance.SetActive(false);
        _instances[indexPrefab].Add(instance);
    }

    private void AddGroupPool(int indexPrefab)
    {
        if (_prefabs[indexPrefab].prefab == null) return;

        GameObject groupInstance = new(_prefabs[indexPrefab].tipoObjetoPool.ToString());
        groupInstance.transform.SetParent(transform);

        _groupPool.Add(groupInstance);
    }

    private void InstancePrefabs()
    {
        if (_prefabs.Count == 0)
        {
            Debug.LogError("no hay prefabs establecidos");
            return;
        }

        for (int i = 0; i < _prefabs.Count; i++)
        {
            _instances.Add(new List<GameObject>());

            AddGroupPool(i);

            for (int j = 0; j < _prefabs[i].intialInstances; j++)
            {
                if (_prefabs[i].prefab == null) break;

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
        return _groupPool[index];
    }
}
