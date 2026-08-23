#ifndef CUSTOM_LIGHTING_INCLUDED
#define CUSTOM_LIGHTING_INCLUDED

#if defined(SHADERGRAPH_PREVIEW)
void MainLight_half(out half3 Direction)
{
    Direction = half3(0, 1, 0);
}
#else
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

void MainLight_half(out half3 Direction)
{
    Light light = GetMainLight();
    Direction = light.direction;
}
#endif

#endif