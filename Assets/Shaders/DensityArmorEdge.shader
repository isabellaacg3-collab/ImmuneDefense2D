Shader "ImmuneDefense/DensityArmorEdge"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite", 2D) = "white" {}
        _EdgeWidth ("Edge width in texels", Float) = 28
    }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "Queue"="Transparent" "RenderType"="Transparent" "CanUseSpriteAtlas"="False" }
        Pass
        {
            Tags { "LightMode"="SRPDefaultUnlit" }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);
            float4 _MainTex_TexelSize;

            CBUFFER_START(UnityPerMaterial)
                float _EdgeWidth;
            CBUFFER_END

            struct Attributes
            {
                float3 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                half4 color : COLOR;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                half4 color : COLOR;
            };

            Varyings Vert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS);
                output.uv = input.uv;
                output.color = input.color;
                return output;
            }

            half AlphaAt(float2 uv)
            {
                return SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv).a;
            }

            half4 Frag(Varyings input) : SV_Target
            {
                float2 uv = input.uv;
                half center = AlphaAt(uv);
                float2 stepUV = _MainTex_TexelSize.xy * _EdgeWidth;
                float2 diagonal = stepUV * 0.7071;
                half edge = max(AlphaAt(uv + float2(stepUV.x, 0)), AlphaAt(uv - float2(stepUV.x, 0)));
                edge = max(edge, max(AlphaAt(uv + float2(0, stepUV.y)), AlphaAt(uv - float2(0, stepUV.y))));
                edge = max(edge, max(AlphaAt(uv + diagonal), AlphaAt(uv - diagonal)));
                edge = max(edge, max(AlphaAt(uv + float2(diagonal.x, -diagonal.y)), AlphaAt(uv + float2(-diagonal.x, diagonal.y))));
                half outline = saturate(edge - center) * (1.0h - smoothstep(0.1h, 0.65h, center));
                return half4(input.color.rgb, outline * input.color.a);
            }
            ENDHLSL
        }
    }
    Fallback Off
}
