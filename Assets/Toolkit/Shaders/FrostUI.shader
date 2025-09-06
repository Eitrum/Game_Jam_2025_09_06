// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Toolkit/FrostUI"
{
	Properties{

		_Size("_Size", Range(0, 0.1)) = 0.005
	}

		SubShader
	{
		Tags { "Queue" = "Transparent" }

		GrabPass
		{

		}

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct v2f
			{
				float4 grabPos : TEXCOORD0;
				float4 pos : SV_POSITION;
			};

			v2f vert(appdata_base v) {
				v2f o;
			  o.pos = UnityObjectToClipPos(v.vertex);
				o.grabPos = ComputeGrabScreenPos(o.pos);
				return o;
			}

			Float _Size;
			sampler2D _GrabTexture;

			half4 frag(v2f i) : SV_Target
			{
				half4 bgcolor = float4(0,0,0,0);
				for (float x = -1; x <= 1; x++) {
					for (float y = -1; y <= 1; y++) {
						bgcolor += tex2Dproj(_GrabTexture, i.grabPos + float4(_Size * x,_Size * y,0.0f,0));
					}
				}
				bgcolor /= 9;
				return bgcolor;
			}
			ENDCG
		}
	}
}

