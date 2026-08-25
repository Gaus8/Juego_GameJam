#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "AutoLight.cginc"

struct appdata
{
    float4 vertex : POSITION;
    float4 uv : TEXCOORD0;
    float3 normal : NORMAL;
};

struct v2f
{
    float4 pos : SV_POSITION;
    float2 uv : TEXCOORD0;
    float3 viewDir : TEXCOORD1;
    SHADOW_COORDS(2)
    float3 worldNormal : TEXCOORD3;
    float3 worldPos : TEXCOORD4;
};

sampler2D _MainTex;
float4 _MainTex_ST;

float4 _Color;
float4 _AmbientColor;
float4 _SpecularColor;
float _Glossiness;
float _Smoothness;
float4 _RimColor;
float _RimBlend;
float _RimThreshold;

v2f vert (appdata v)
{
    v2f o;
    o.pos = UnityObjectToClipPos(v.vertex);
    o.worldNormal = UnityObjectToWorldNormal(v.normal);
    o.worldPos = mul(unity_ObjectToWorld, v.vertex);
    o.viewDir = WorldSpaceViewDir(v.vertex);
    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
    TRANSFER_SHADOW(o)
    return o;
}

float4 frag (v2f i) : SV_Target
{
    float3 normal = normalize(i.worldNormal);
    float3 viewDir = normalize(i.viewDir);

    float3 lightDir = _WorldSpaceLightPos0.xyz;
    #if defined(POINT) || defined(SPOT)
        lightDir = normalize(_WorldSpaceLightPos0.xyz - i.worldPos);
    #endif

    UNITY_LIGHT_ATTENUATION(attenuation, i, i.worldPos);

    float NdotL = dot(lightDir, normal);

    float shadowValue = attenuation;
    float lightIntensity = smoothstep(0, _Smoothness, NdotL * shadowValue) * 0.75;
    float4 light = lightIntensity * _LightColor0;

    float3 halfVector = normalize(lightDir + viewDir);
    float NdotH = dot(normal, halfVector);
    float specularIntensity = pow(NdotH * lightIntensity, _Glossiness * _Glossiness);
    float specularIntensitySmooth = smoothstep(0.005, 0.01, specularIntensity);
    float4 specular = specularIntensitySmooth * _SpecularColor;

    float rimDot = 1 - dot(viewDir, normal);
    float rawRimIntensity = rimDot * pow(NdotL, _RimThreshold);
    float rimIntensity = smoothstep(_RimBlend - 0.01, _RimBlend + 0.01, rawRimIntensity);
    float4 rim = rimIntensity * _RimColor;

    float4 texColor = tex2D(_MainTex, i.uv);

    float4 ambient = 0;
    #if defined(UNITY_PASS_FORWARDBASE)
        ambient = _AmbientColor;
    #endif

    return (light + rim + specular + ambient) * attenuation * _Color * texColor;
}