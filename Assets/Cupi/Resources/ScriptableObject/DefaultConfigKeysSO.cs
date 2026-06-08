using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "DefaultConfigKeysSO", menuName = "Scriptable Objects/DefaultConfigKeysSO")]
public class DefaultConfigKeysSO : ScriptableObject
{
    [Header("Two Keys Mode")]
    // a y d y lo mismo en flechas

    public InputActionReference TwoKeysModeKeyOne;
    public InputActionReference TwoKeysModeKeyTwo;

    [Header("Three Keys Mode")]
    // asd y lo mismo en flechas
    public InputActionReference ThreeKeysModeKeyOne;
    public InputActionReference ThreeKeysModeKeyTwo;
    public InputActionReference ThreeKeysModeKeyThree;

    [Header("Four Keys Mode")]
    //lo mismo que fnf xd
    public InputActionReference FourKeysModeKeyOne;
    public InputActionReference FourKeysModeKeyTwo;
    public InputActionReference FourKeysModeKeyThree;
    public InputActionReference FourKeysModeKeyFour;

    [Header("Five Keys Mode")]
    //lo mismo pero la tercera es espacio
    public InputActionReference FiveKeysModeKeyOne;
    public InputActionReference FiveKeysModeKeyTwo;
    public InputActionReference FiveKeysModeKeyThree;
    public InputActionReference FiveKeysModeKeyFour;
    public InputActionReference FiveKeysModeKeyFive;

    [Header("Six Keys Mode")]
    //a s d j k l
    public InputActionReference SixKeysModeKeyOne;
    public InputActionReference SixKeysModeKeyTwo;
    public InputActionReference SixKeysModeKeyThree;
    public InputActionReference SixKeysModeKeyFour;
    public InputActionReference SixKeysModeKeyFive;
    public InputActionReference SixKeysModeKeySix;

}
