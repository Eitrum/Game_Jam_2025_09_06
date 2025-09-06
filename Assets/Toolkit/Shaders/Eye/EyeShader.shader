Shader "Toolkit/EyeShader"
{
	Properties
	{
		_Color("Eye Color", Color) = (1,1,1,1)
		_MainTex("Eye Texture", 2D) = "white" {}
		_MaskTex("Eye Mask", 2D) = "white" {}
		_BloodVeins("Blood Veins", 2D) = "white" {}
		_BloodColor("Blood Color", Color) = (.9, .2, .2, 0.4)
		_BloodAmount("Blood Amount", Range(0,1)) = 0.0
		_BloodRadius("Blood Radius", Range(0,1)) = 1.0
		_VeinRadius("Vein Radius", Range(0,2)) = 1.0
		_Glossiness("Smoothness", Range(0,1)) = 0.5
	}
		SubShader
		{
			Tags { "RenderType" = "Opaque" }
			LOD 200

			CGPROGRAM

			#pragma surface surf Standard fullforwardshadows
			#pragma target 3.0

			sampler2D _MainTex;
			sampler2D _MaskTex;
			sampler2D _BloodVeins;

			struct Input
			{
				float2 uv_MainTex;
			};

			half _Glossiness;
			half _BloodAmount;
			half _BloodRadius;
			half _VeinRadius;
			fixed4 _Color;
			fixed4 _BloodColor;

			void surf(Input IN, inout SurfaceOutputStandard o)
			{
				fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
				fixed4 mask = tex2D(_MaskTex, IN.uv_MainTex);
				fixed4 veins = tex2D(_BloodVeins, IN.uv_MainTex);

				half len = length((IN.uv_MainTex - .5) * 2.);

				c = lerp(c, c * _Color, 1 - mask.g);
				c = lerp(c, _BloodColor,_BloodColor.a * mask.g * _BloodAmount * max(0,len - _BloodRadius));
				c.rgb += _BloodColor.rgb * (_BloodColor.a * veins.a * _BloodAmount * mask.g * max(0, len - _VeinRadius));

				o.Albedo = c.rgb;
				o.Metallic = 0;
				o.Smoothness = _Glossiness;
				o.Alpha = c.a;
			}
			ENDCG
		}
			FallBack "Diffuse"
}
