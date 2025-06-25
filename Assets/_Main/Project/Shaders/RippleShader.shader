Shader "Custom/RippleShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _RippleStrength ("Ripple Strength", Float) = 0.02
        _RippleSpeed ("Ripple Speed", Float) = 2.0
        _RippleFrequency ("Ripple Frequency", Float) = 10.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        Lighting Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float _RippleStrength;
            float _RippleSpeed;
            float _RippleFrequency;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float time = _Time.y * _RippleSpeed;

                // UV'yi sinüsle boz: dalga efekti
                float2 rippleUV = i.uv;
                rippleUV.x += sin((i.uv.y + time) * _RippleFrequency) * _RippleStrength;
                rippleUV.y += cos((i.uv.x + time) * _RippleFrequency) * _RippleStrength;

                fixed4 col = tex2D(_MainTex, rippleUV);
                return col;
            }
            ENDCG
        }
    }
}
