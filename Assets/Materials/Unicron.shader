Shader "Unlit/Unicron"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_CellNumber("Cell Number", float) = -1.0
		_PlayheadProgress("Playhead", float) = 10.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

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

            sampler2D _MainTex;
            float4 _MainTex_ST;
			float _CellNumber;
			float _PlayheadProgress;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {

				if(abs(i.uv.x - _PlayheadProgress) <= 0.003)
				{
					return fixed4(.9, .9, .9, 1);
				}
				/*
				// I don't even remember why, but it had something to do with the first one strattling the seam or something
				if(vertical < 0.0625/2.0)
				{
					vertical = 1.0 - vertical;
				}

				vertical = fmod(vertical, 0.0625); // 1 / 16 == .0625
				float _out = 1;

				float test = _CellNumber;
				if(test == 0)
				{
					test = 16.0;
				}

				if (abs(vertical - 0.16) < 0.1)
				{
					_out = 0;
					float v2 = i.uv.x;
					if(v2 < 0.0625/2.0)
					{
						v2 = 1.0 - v2;
					}
					float c = v2 / 0.0625;
					if( abs(test - c) < 0.5)
					{
						return fixed4(0, 1, 0, 1);
					}
				}else
				{
					_out = 1;
				}
				*/

				float _out = 0.51;

                return fixed4(_out, _out, _out, 1);
            }
            ENDCG
        }
    }
}