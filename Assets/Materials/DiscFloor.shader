Shader "Custom/DiscFloor"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
		_Loudness ("Loudness", Range(0, 1)) = 0
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        struct Input
        {
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
		half _Loudness;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = _Color;
            o.Albedo = fixed3(0, 0, 0);

			if (length(float3(IN.uv_MainTex.x, IN.uv_MainTex.y, 0)) > _Loudness)
			{
				o.Albedo = c.rgb;
			}

			//float H = 1.0 - i.uv.y;
			//float R = abs(H * 6.0f - 3.0f) - 1.0f;
			//float G = 2.0f - abs(H * 6.0f - 2.0f);
			//float B = 2.0f - abs(H * 6.0f - 4.0f);

			//col = fixed4(col.r * R, col.g * G, col.b * B, 1.0);

            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
