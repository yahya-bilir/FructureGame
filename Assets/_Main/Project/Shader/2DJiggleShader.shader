Shader "Custom/2DFullJiggle_XY"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        _JiggleStrengthX ("Jiggle Strength X", Float) = 0.02
        _JiggleStrengthY ("Jiggle Strength Y", Float) = 0.02
        _JiggleSpeed ("Jiggle Speed", Float) = 5.0
        _JiggleFrequency ("Jiggle Frequency", Float) = 10.0
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "IgnoreProjector" = "True"
            "PreviewType" = "Sprite"
            "CanUseSpriteAtlas" = "True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 texcoord : TEXCOORD0;
                fixed4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;

            float _JiggleStrengthX;
            float _JiggleStrengthY;
            float _JiggleSpeed;
            float _JiggleFrequency;

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                float3 pos = IN.vertex.xyz;

                float wiggle = sin(_Time.y * _JiggleSpeed + pos.x * _JiggleFrequency);
                pos.x += wiggle * _JiggleStrengthX;

                float bobble = sin(_Time.y * _JiggleSpeed + pos.y * _JiggleFrequency);
                pos.y += bobble * _JiggleStrengthY;

                OUT.vertex = UnityObjectToClipPos(float4(pos, 1.0));
                OUT.texcoord = TRANSFORM_TEX(IN.texcoord, _MainTex);
                OUT.color = IN.color * _Color;
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                fixed4 c = tex2D(_MainTex, IN.texcoord) * IN.color;
                return c;
            }

            ENDCG
        }
    }
}
