using System;
using System.Collections.Generic;
using UnityEngine;

using CupiEngine.Input;

public class ShockWaveController : MonoBehaviour
{
    [SerializeField] ShockwaveSettings defaultSettings = ShockwaveSettings.Default;

    struct ShockwaveInstance
    {
        public Vector2 origin;
        public float startTime;
        public ShockwaveSettings settings;
    }

    const int MAX = 8;
    List<ShockwaveInstance> active = new();

    Vector4[] originData = new Vector4[MAX];
    Vector4[] paramsData = new Vector4[MAX];

    PlayerInputs playerInputs;

    public void TriggerShockwave(Vector2 screenUV, ShockwaveSettings? overrides = null)
    {
        var settings = overrides ?? defaultSettings;
        active.Add(new ShockwaveInstance { origin = screenUV, startTime = Time.time, settings = settings });
    }


    void Update()
    {
        active.RemoveAll(s =>
            (s.settings.speed * (Time.time - s.startTime)) - s.settings.size > s.settings.maxDistance);

        for (int i = 0; i < MAX; i++)
        {
            if (i < active.Count)
            {
                var inst = active[i];
                float elapsed = Time.time - inst.startTime;
                float waveDistanceFromCenter = elapsed * inst.settings.speed;

                originData[i] = new Vector4(inst.origin.x, inst.origin.y, 0, 0);
                paramsData[i] = new Vector4(inst.settings.size, inst.settings.strength, waveDistanceFromCenter, 0);
            }
            else
            {
                originData[i] = Vector4.zero;
                paramsData[i] = new Vector4(0, 0, -10, 0); // waveDist muy negativo -> ringMask siempre 0
            }
        }

        Shader.SetGlobalVectorArray("_ShockwaveOrigin", originData);
        Shader.SetGlobalVectorArray("_ShockwaveParams", paramsData);
        Shader.SetGlobalInt("_ShockwaveCount", Mathf.Min(active.Count, MAX));
    }

    private void Start()
    {
        playerInputs = new(TileModePlayStyle.OneKey);

        playerInputs.OnButtonPressed += OnClick;

        //TriggerShockwave(Vector2.one / 2.0f);
    }

    public void OnClick(CorrespondenciaTecla tecla,double time)
    {
        TriggerShockwave(Vector2.one / 2f);
    }
}

[Serializable]
public struct ShockwaveSettings
{
    public float size;
    public float strength;
    public float speed;        // qué tan rápido crece el radio del anillo
    public float maxDistance;  // cuándo se considera "terminado"

    public static ShockwaveSettings Default => new ShockwaveSettings
    {
        size = 0.05f,
        strength = -0.1f,
        speed = 0.6f,
        maxDistance = 0.6f
    };
}