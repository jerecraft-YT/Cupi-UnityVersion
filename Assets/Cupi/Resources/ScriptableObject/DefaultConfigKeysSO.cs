using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "DefaultConfigKeysSO", menuName = "Scriptable Objects/DefaultConfigKeysSO")]
public class DefaultConfigKeysSO : ScriptableObject
{
    [Header("TwoKeyMode")]
    public InputActionReference Left;
    public InputActionReference Right;
}
