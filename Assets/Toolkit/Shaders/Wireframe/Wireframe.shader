Shader "Toolkit/Wireframe/Opaque"
{
	Properties
	{
		_Color ("Color", color) = (0, 0, 0, 1)
		_Thickness ("Thickness", range(0, 1)) = 0.1
	}
	SubShader
	{
		Tags{ "Queue" = "Geometry" "RenderType" = "Geometry" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma geometry geom
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2g
			{
				float4 vertex : SV_POSITION;
			};

			struct g2f
			{
				float4 vertex : SV_POSITION;
				float3 bary : TEXCOORD1;
			};

			float _Thickness;
			float4 _Color;

			v2g vert (appdata v)
			{
				v2g o;
                o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}

			[maxvertexcount(3)]
			void geom(triangle v2g i[3], inout TriangleStream<g2f> triangleStream){
				g2f o;
				o.vertex = i[0].vertex;
				o.bary = float3(1,0,0);
				triangleStream.Append(o);

				o.vertex = i[1].vertex;
				o.bary = float3(0,0,1);
				triangleStream.Append(o);

				o.vertex = i[2].vertex;
				o.bary = float3(0,1,0);
				triangleStream.Append(o);
			}
			
			fixed4 frag (g2f i) : SV_Target
			{
				if(i.bary.x > _Thickness & i.bary.y > _Thickness & i.bary.z > _Thickness)
				{
					discard;
				}
				return _Color;
			}
			ENDCG
		}
	}
}
