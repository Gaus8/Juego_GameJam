Shader "Bytesized/Toon"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _MainTex("Main Texture", 2D) = "white" {}
        _HatchTex("Hatch Texture", 2D) = "white" {}
        _HatchTiling("Hatch Tiling", Float) = 1
        _HatchStrength("Hatch Strength", Range(0, 1)) = 1
        _HatchContrast("Hatch Contrast", Range(0.5, 4)) = 2
        _ShadowTint("Shadow Tint", Range(0, 1)) = 0.45
        _HatchEdge("Hatch Edge", Range(0, 2)) = 0.55
        _HatchSoftness("Hatch Softness", Range(0.001, 1)) = 0.08
        _LightIntensity("Light Intensity", Range(0, 4)) = 1.4
        _AmbientFill("Ambient Fill", Range(0, 1)) = 0.35
        _ShadowFill("Shadow Fill", Range(0, 2)) = 1
        _ToonMid("Toon Mid", Range(0.01, 1)) = 0.08
        _ToonCore("Toon Highlight", Range(0.01, 1)) = 0.4
        _LightReach("Light Reach", Range(1, 20)) = 10
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Geometry"
        }

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile_fragment _ _SHADOWS_SOFT _SHADOWS_SOFT_LOW _SHADOWS_SOFT_MEDIUM _SHADOWS_SOFT_HIGH
            #pragma multi_compile _ _FORWARD_PLUS
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 positionWS : TEXCOORD1;
                float3 normalWS : TEXCOORD2;
                float4 shadowCoord : TEXCOORD3;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_HatchTex);
            SAMPLER(sampler_HatchTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _Color;
                float _HatchTiling;
                float _HatchStrength;
                float _HatchContrast;
                float _ShadowTint;
                float _HatchEdge;
                float _HatchSoftness;
                float _LightIntensity;
                float _AmbientFill;
                float _ShadowFill;
                float _ToonMid;
                float _ToonCore;
                float _LightReach;
            CBUFFER_END

            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);

                VertexPositionInputs posInputs = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs norInputs = GetVertexNormalInputs(input.normalOS);

                output.positionCS = posInputs.positionCS;
                output.positionWS = posInputs.positionWS;
                output.normalWS = norInputs.normalWS;
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                output.shadowCoord = GetShadowCoord(posInputs);
                return output;
            }

            float SampleHatch(float3 positionWS, float3 normalWS)
            {
                float3 uv = positionWS * _HatchTiling;
                float3 weights = abs(normalWS);
                weights /= max(dot(weights, 1.0), 1e-5);

                float x = SAMPLE_TEXTURE2D_LOD(_HatchTex, sampler_HatchTex, uv.zy, 0).r;
                float y = SAMPLE_TEXTURE2D_LOD(_HatchTex, sampler_HatchTex, uv.xz, 0).r;
                float z = SAMPLE_TEXTURE2D_LOD(_HatchTex, sampler_HatchTex, uv.xy, 0).r;
                float hatch = x * weights.x + y * weights.y + z * weights.z;
                return saturate(pow(max(hatch, 1e-4), _HatchContrast));
            }

            float ToonRamp(float value)
            {
                float s = max(_HatchSoftness, 0.001);
                float midEdge = min(_ToonMid, _ToonCore);
                float coreEdge = max(_ToonMid, _ToonCore);
                float mid = smoothstep(midEdge, midEdge + s, value);
                float core = smoothstep(coreEdge, coreEdge + s, value);
                return saturate(mid * 0.55 + core * 0.45);
            }

            float ToonAttenuation(float atten)
            {
                float k = rcp(max(_LightReach, 0.01));
                return saturate(atten / (atten + k));
            }

            float ToonLightAmount(Light light, float3 normal)
            {
                float nDotL = saturate(dot(light.direction, normal));
                float intensity = max(max(light.color.r, light.color.g), light.color.b);
                float raw = nDotL * light.distanceAttenuation * light.shadowAttenuation * saturate(intensity);
                return ToonRamp(raw);
            }

            float3 LightDiffuse(Light light, float3 normal, bool stretchRange)
            {
                float nDotL = saturate(dot(light.direction, normal));
                float atten = light.distanceAttenuation * light.shadowAttenuation;
                if (stretchRange)
                    atten = ToonAttenuation(atten);
                return light.color * ToonRamp(nDotL * atten);
            }

            float4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);

                float3 normal = normalize(input.normalWS);
                float3 baseColor = _Color.rgb * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv).rgb;

                float4 shadowCoord = TransformWorldToShadowCoord(input.positionWS);
                Light mainLight = GetMainLight(shadowCoord);

                float mainAmount = ToonLightAmount(mainLight, normal);
                float3 mainDiffuse = LightDiffuse(mainLight, normal, false);
                float3 extraDiffuse = 0;

                #if defined(_ADDITIONAL_LIGHTS)
                uint pixelLightCount = GetAdditionalLightsCount();
                InputData inputData = (InputData)0;
                inputData.positionWS = input.positionWS;
                inputData.normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(input.positionCS);

                LIGHT_LOOP_BEGIN(pixelLightCount)
                    Light extraLight = GetAdditionalLight(lightIndex, input.positionWS, half4(1, 1, 1, 1));
                    extraDiffuse += LightDiffuse(extraLight, normal, true);
                LIGHT_LOOP_END
                #endif

                float extraAmount = max(max(extraDiffuse.r, extraDiffuse.g), extraDiffuse.b);
                float hatchMask = saturate(smoothstep(_HatchEdge, _HatchEdge + _HatchSoftness, 1.0 - mainAmount));
                hatchMask *= 1.0 - saturate(extraAmount * 0.5);

                float hatch = SampleHatch(input.positionWS, normal);
                hatch = lerp(1.0, hatch, _HatchStrength);

                float3 shadowColor = baseColor * _ShadowTint * hatch;
                float3 directLight = mainDiffuse + extraDiffuse;
                float3 litColor = baseColor * (_AmbientFill + directLight * _LightIntensity);

                float3 color = lerp(litColor, shadowColor, hatchMask);
                color += baseColor * extraDiffuse * _LightIntensity * _ShadowFill * hatchMask;

                return float4(color, 1);
            }
            ENDHLSL
        }

        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }

            ZWrite On
            ZTest LEqual
            ColorMask 0
            Cull Back

            HLSLPROGRAM
            #pragma vertex ShadowVert
            #pragma fragment ShadowFrag
            #pragma multi_compile_instancing
            #pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"

            float3 _LightDirection;
            float3 _LightPosition;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            float4 GetShadowPositionHClip(Attributes input)
            {
                float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
                float3 normalWS = TransformObjectToWorldNormal(input.normalOS);

            #if defined(_CASTING_PUNCTUAL_LIGHT_SHADOW)
                float3 lightDirectionWS = normalize(_LightPosition - positionWS);
            #else
                float3 lightDirectionWS = _LightDirection;
            #endif

                float4 positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, lightDirectionWS));
            #if UNITY_REVERSED_Z
                positionCS.z = min(positionCS.z, UNITY_NEAR_CLIP_VALUE);
            #else
                positionCS.z = max(positionCS.z, UNITY_NEAR_CLIP_VALUE);
            #endif
                return positionCS;
            }

            Varyings ShadowVert(Attributes input)
            {
                Varyings output = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                output.positionCS = GetShadowPositionHClip(input);
                return output;
            }

            half4 ShadowFrag(Varyings input) : SV_Target
            {
                return 0;
            }
            ENDHLSL
        }

        Pass
        {
            Name "DepthOnly"
            Tags { "LightMode" = "DepthOnly" }

            ZWrite On
            ColorMask R
            Cull Back

            HLSLPROGRAM
            #pragma vertex DepthVert
            #pragma fragment DepthFrag
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            Varyings DepthVert(Attributes input)
            {
                Varyings output = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                return output;
            }

            half DepthFrag(Varyings input) : SV_Target
            {
                return input.positionCS.z;
            }
            ENDHLSL
        }
    }
    FallBack Off
}
