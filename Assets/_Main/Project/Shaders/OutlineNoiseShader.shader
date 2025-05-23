Shader "Custom/SpriteDualOutlineCircularFixed"
{
    Properties
    {
        _MainTex("Sprite", 2D) = "white" {}
        _Color("Sprite Color", Color) = (1,1,1,1)
        _OutlineColor("Outline Color", Color) = (0,1,0,1)
        _ExpandAlpha("Alpha Expand Amount", Float) = 2.0
        _OutlineThickness("Outline Thickness", Float) = 1.5
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100

        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize;
            fixed4 _Color;
            fixed4 _OutlineColor;
            float _ExpandAlpha;
            float _OutlineThickness;

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float sampleCircleAlpha(float2 uv, float radius, int steps)
            {
                float sum = 0.0;
                float angleStep = 6.2831853 / steps;
                for (int i = 0; i < steps; i++)
                {
                    float angle = i * angleStep;
                    float2 offset = float2(cos(angle), sin(angle)) * radius;
                    sum += tex2D(_MainTex, uv + offset).a;
                }
                return sum / steps;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float baseAlpha = tex2D(_MainTex, i.uv).a;

                // 1. Ana sprite varsa onu çiz
                if (baseAlpha > 0.1)
                {
                    fixed4 col = tex2D(_MainTex, i.uv) * _Color;
                    return col;
                }

                // Ortalama texel boyutu
                float avgTexel = (_MainTex_TexelSize.x + _MainTex_TexelSize.y) * 0.5;

                // Dinamik örnekleme sayısı (maksimum 64'e kadar)
                int sampleCount = int(ceil((_ExpandAlpha + _OutlineThickness) * 4.0));
                sampleCount = clamp(sampleCount, 16, 64);

                // 2. Şeffaf genişleme alanı
                float expandRadius = avgTexel * _ExpandAlpha;
                float expandedAlpha = sampleCircleAlpha(i.uv, expandRadius, sampleCount);
                if (expandedAlpha > 0.03)
                {
                    return float4(0,0,0,0); // Şeffaf boşluk
                }

                // 3. Dış outline alanı
                float outlineRadius = avgTexel * (_ExpandAlpha + _OutlineThickness);
                float outlineAlpha = sampleCircleAlpha(i.uv, outlineRadius, sampleCount);
                if (outlineAlpha > 0.03)
                {
                    return _OutlineColor;
                }

                return float4(0,0,0,0); // Boş piksel
            }

            ENDCG
        }
    }
}
