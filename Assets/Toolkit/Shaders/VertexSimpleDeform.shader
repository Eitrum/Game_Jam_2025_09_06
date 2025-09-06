Shader "Custom/VertexSimpleDeform"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
		[ShowAsVector3] _Offset("Offset", vector) = (0, 0, 0, 0)
		[ShowAsVector3] _Direction("Direction", vector) = (1, 0, 0, 0)
		_Strength("Strength", Range(-1, 1)) = 0.1
	}
		SubShader
		{
			Tags { "RenderType" = "Opaque" }
			LOD 200

			CGPROGRAM
			#pragma surface surf Standard fullforwardshadows vertex:vert
			#pragma target 3.0

			sampler2D _MainTex;
			float3 _Offset;
			float3 _Direction;
			float _Strength;

			struct Input
			{
				float2 uv_MainTex;
			};

			void vert(inout appdata_full v, out Input o)
			{
				UNITY_INITIALIZE_OUTPUT(Input, o);
				float3 p = v.vertex.xyz + _Offset;
				float t = ((p.x * _Direction.x) + (p.y * _Direction.y) + (p.z * _Direction.z)) / (_Direction.x * _Direction.x + _Direction.y * _Direction.y + _Direction.z * _Direction.z);
				float3 np = float3(_Offset.x + _Direction.x * t, _Offset.y + _Direction.y * t, _Offset.z + _Direction.z * t);
				v.vertex.xyz = lerp(p - _Offset, np, t * _Strength);
			}

			half _Glossiness;
			half _Metallic;
			fixed4 _Color;


			void surf(Input IN, inout SurfaceOutputStandard o)
			{
				fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
				o.Albedo = c.rgb;
				o.Metallic = _Metallic;
				o.Smoothness = _Glossiness;
				o.Alpha = c.a;
			}
			ENDCG
		}
			FallBack "Diffuse"
}
