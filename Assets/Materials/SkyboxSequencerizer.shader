Shader "Unlit/SkyboxSequencerizer"
{
    Properties
    {
        _Color ("Color", Color) = (1, 1, 1, 1)
		_Loudness("Loudness", Range(0, 1)) = 0
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100
		Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

			fixed4 _Color;
			float _Loudness;
            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float maxLoudness = 1.5;
                float loudness = min(1.0, _Loudness / maxLoudness);
                
                float maxHeight = 0.8;
                float totalHeight = loudness * maxHeight;

                float start = totalHeight * 0.2;
                float end = start + (totalHeight * 0.35);

                float yPos = i.uv.y * maxHeight;

                if(yPos < end)
                {
                    if(yPos < start)
                    {
                        return _Color;
                    }

                    float c = end - start;
                    float actually = yPos - start;
                    float cOut = 1.0 - (actually / c);

                    return fixed4(_Color.r * cOut, _Color.g * cOut, _Color.b * cOut, min(0.25, cOut));
                }

                //fixed4 col = fixed4(_Color.r * colorMultiplier, _Color.g * colorMultiplier, _Color.b * colorMultiplier, 1);

                return fixed4(0, 0, 0, 0);
            }
            ENDCG
        }
    }
}
