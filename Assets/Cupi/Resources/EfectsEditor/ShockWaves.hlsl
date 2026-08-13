#ifndef SHOCKWAVES_INCLUDED
#define SHOCKWAVES_INCLUDED
#if defined(__INTELLISENSE__)
float4 _ScreenParams;
#endif

#define MAX_SHOCKWAVES 8

float4 _ShockwaveOrigin[MAX_SHOCKWAVES];
float4 _ShockwaveParams[MAX_SHOCKWAVES];
int _ShockwaveCount;

void GetShockwaveOffset_float(float2 UV, out float Offset)
{
    Offset = 0;

    float aspect = _ScreenParams.x / _ScreenParams.y; // width / height

    for (int idx = 0; idx < MAX_SHOCKWAVES; idx++)
    {
        if (idx >= _ShockwaveCount)
            break;

        float2 ringSpawn = _ShockwaveOrigin[idx].xy;
        float size = _ShockwaveParams[idx].x;
        float strength = _ShockwaveParams[idx].y;
        float waveDist = _ShockwaveParams[idx].z;

        float2 diff = UV - ringSpawn;
        diff.x *= aspect; // <-- corrige el estiramiento horizontal
        float dist = length(diff);

        float edge1 = waveDist - size;
        float edge2 = waveDist + size;
        float ring = smoothstep(edge1, edge2, dist);
        float ringMask = ring * (1 - ring);

        float outerFade = 1 - smoothstep(0.45, 0.5, dist);

        Offset += ringMask * outerFade * strength;
    }
}
#endif