using UnityEngine;
using UnityEngine.InputSystem;

public class LoadAndSaveData : MonoBehaviour
{
    public LoadAndSaveData instance;
    public DefaultConfigKeysSO defaultConfigKeys;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        defaultConfigKeys = Resources.Load<DefaultConfigKeysSO>("ScriptableObject/DefaultConfigKeysSO");

        DontDestroyOnLoad(gameObject);
    }

    public static InputActionReference loadActualInput()
    {
        return null;
    }
}
