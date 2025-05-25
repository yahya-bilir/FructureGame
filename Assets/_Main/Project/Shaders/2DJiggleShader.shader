Shader "Custom/Particles_JiggleShine"
{
    Properties
    {
        [NoScaleOffset]_MainTex("Main Texture", 2D) = "white" {}
        [HDR]_Color("HDR Tint", Color) = (1,1,1,1)
        _JiggleStrengthX("Jiggle Strength X", Float) = 0.02
        _JiggleStrengthY("Jiggle Strength Y", Float) = 0.02
        _JiggleSpeed("Jiggle Speed", Float) = 5.0
        _JiggleFrequency("Jiggle Frequency", Float) = 10.0
    }

    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" "Queue" = "Transparent" "RenderType" = "Transparent" }
        Cull Off ZWrite Off ZTest LEqual
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            uniform float4 _Color;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _JiggleStrengthX;
            float _JiggleStrengthY;
            float _JiggleSpeed;
            float _JiggleFrequency;

            v2f vert(appdata v)
            {
                v2f o;
                float3 pos = v.vertex.xyz;

                float jiggleX = sin(_Time.y * _JiggleSpeed + pos.y * _JiggleFrequency) * _JiggleStrengthX;
                float jiggleY = sin(_Time.y * _JiggleSpeed + pos.x * _JiggleFrequency) * _JiggleStrengthY;
                pos.x += jiggleX;
                pos.y += jiggleY;

                o.vertex = TransformObjectToHClip(pos);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                half4 texColor = tex2D(_MainTex, i.uv);
                half4 result = texColor * _Color;
                result.a = texColor.a * _Color.a;
                return result;
            }

            ENDHLSL
        }
    }

    FallBack "Sprites/Default"
}
