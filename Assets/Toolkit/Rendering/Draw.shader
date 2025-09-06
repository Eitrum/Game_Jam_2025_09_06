Shader "Toolkit/Draw/Default"
{
	Properties
	{
	}
	SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 100

		// ZWrite Off
		// Blend SrcAlpha OneMinusSrcAlpha

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
				 uint value = _ColorArray[unity_InstanceID];
				 fixed4 col = fixed4(
					 ((value / 16777216)) / 255.0,
					 ((value / 65536) % 256) / 255.0,
					 ((value / 256) % 256) / 255.0,1);
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
