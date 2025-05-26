Shader "Custom/OuterOutlineURP_Full"
{
    Properties
    {
        _MainTex("Sprite Texture", 2D) = "white" {}
        _OuterOutlineColor("Outer Outline Color", Color) = (0, 1, 0, 0.8)
        _OutlineThickness("Outline Thickness", Float) = 2.0
    }

    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            Name "OuterOutline"
            Tags { "LightMode" = "Universal2D" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            float4 _OuterOutlineColor;
            float _OutlineThickness;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float baseAlpha = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv).a;

                // Eğer sprite'ın kendisiyse, onu çiz
                if (baseAlpha > 0.1)
                {
                    return SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);
                }

                // Sprite'ın dışında: komşu piksellerde alpha varsa, burası outline olur
                float2 directions[16] = {
                    float2(1,0), float2(-1,0), float2(0,1), float2(0,-1),
                    float2(1,1), float2(-1,1), float2(1,-1), float2(-1,-1),
                    float2(2,0), float2(-2,0), float2(0,2), float2(0,-2),
                    float2(2,2), float2(-2,2), float2(2,-2), float2(-2,-2)
                };

                float visible = 0.0;

                for (int i = 0; i < 16; i++)
                {
                    float2 offset = directions[i] * (_OutlineThickness / 512.0); // 512 = approx texture size
                    float2 sampleUV = IN.uv + offset;

                    float a = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, sampleUV).a;
                    visible += step(0.1, a);
                }

                if (visible > 0.0)
                {
                    return _OuterOutlineColor; // transparan olabilir
                }

                return float4(0, 0, 0, 0); // tamamen saydam
            }

            ENDHLSL
        }
    }

    FallBack "Sprites/Default"
}
