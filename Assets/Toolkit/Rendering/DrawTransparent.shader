Shader "Toolkit/Draw/Transparent"
{
	Properties
	{
	}
	SubShader
	{
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
		LOD 100

		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

		 Pass
		 {
			 CGPROGRAM
			 #pragma vertex vert
			 #pragma fragment frag
			 #pragma multi_compile_instancing
			 #include "UnityCG.cginc"

			 struct appdata
			 {
				 float4 vertex : POSITION;
				 UNITY_VERTEX_INPUT_INSTANCE_ID
			 };

			 struct v2f
			 {
				 float4 vertex : SV_POSITION;
				 UNITY_VERTEX_INPUT_INSTANCE_ID
			 };

			 float _ColorArray[1024];

			 v2f vert(appdata v)
			 {
				 v2f o;
				 UNITY_SETUP_INSTANCE_ID(v);
				 UNITY_TRANSFER_INSTANCE_ID(v, o);
				 o.vertex = UnityObjectToClipPos(v.vertex);
				 return o;
			 }

			 fixed4 frag(v2f i) : SV_Target
			 {
				 UNITY_SETUP_INSTANCE_ID(i);
 #ifdef INSTANCING_ON
				 float value = _ColorArray[unity_InstanceID];
				 fixed4 col = fixed4(
					 ((value / 2097152)) / 127.0,
					 ((value / 16384) % 128) / 127.0,
					 ((value / 128) % 128) / 127.0,
					 ((value) % 128) / 127.0);
				 if (col.a <= 0) {
					 col.rgb = -col.a;
					 col.a = 1;
				 }
				  return col;
  #else
				  fixed4 col = fixed4(1, 1, 1, 1);
				  return col;
  #endif
			  }
			  ENDCG
		  }
	}
}
