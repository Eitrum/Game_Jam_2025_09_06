Shader "Toolkit/Toxel"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Scale("Scale", Range(0.02, 4)) = 1
		_GridScale("GridScale", Range(2, 16)) = 4
		_Blend("Blend", Range(0, 10)) = 3
		_Padding("Padding", Range(0, 0.5)) = 0.05
		_ColorMode("ColorMode", Range(0, 1)) = 0
		_Mode("Mode", Range(0, 1)) = 0
	}
	SubShader{
		Tags { "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
		#pragma surface surf Standard vertex:vert
		#pragma target 5.0

		sampler2D _MainTex;
		float _Scale;
		
		struct Input
		{
			float3 pos;
			float3 norm;
    		float4 vertColor;
            float2 uv_MainTex;
		};

		inline float4 TriplanarSamplingCF(sampler2D textureall, float3 worldPos, float3 worldNormal, float2 center, float2 size)
		{
			float3 projNormal = ( pow( abs( worldNormal ), 50 ) );
			projNormal /= projNormal.x + projNormal.y + projNormal.z;
			float3 nsign = sign( worldNormal );
			float negProjNormalY = max( 0, projNormal.y * -nsign.y );
			projNormal.y = max( 0, projNormal.y * nsign.y );
			half4 xNorm; half4 yNorm; half4 yNormN; half4 zNorm;
			float2 halfSize = size * 0.5;
			center -= halfSize;
			xNorm =  tex2D(textureall, center + abs(fmod(_Scale * worldPos.zy * float2( nsign.x, 1.0 ) * size, 1)) * size);
			yNorm =  tex2D(textureall, center + abs(fmod(_Scale * worldPos.xz * float2( nsign.y, 1.0 ) * size, 1)) * size);
			yNormN = tex2D(textureall, center + abs(fmod(_Scale * worldPos.xz * float2( nsign.y, 1.0 ) * size, 1)) * size);
			zNorm =  tex2D(textureall, center + abs(fmod(_Scale * worldPos.xy * float2(-nsign.z, 1.0 ) * size, 1)) * size);
			return xNorm * projNormal.x + yNorm * projNormal.y + zNorm * projNormal.z + yNormN * negProjNormalY;
		}

		void vert(inout appdata_full v, out Input o)
		{
		    UNITY_INITIALIZE_OUTPUT(Input, o);
			o.pos = mul( unity_ObjectToWorld, v.vertex ).xyz;
			o.norm = UnityObjectToWorldNormal( v.normal );
	        o.vertColor = v.color;
		}

		float _GridScale;
		float _Blend;
		float _Mode;
		float _ColorMode;
		float _Padding;

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			float4 col = float4(0,0,0,1);
			float3 vc = IN.vertColor.rgb;

			vc = pow(vc, _Blend);
			float totalc = vc.r + vc.g + vc.b;
			vc.r /= totalc;
			vc.g /= totalc;
			vc.b /= totalc;

			float2 inputuv = IN.uv_MainTex;
			float val0x = inputuv.x % 0.99999;
			float val0y = inputuv.y % 0.99999;
			int texid0 = floor(val0x * 256.1);
			int texid1 = floor(val0y * 256.1);
			int texid2 = floor(inputuv.x * 1.00001);
			//int texid3 = floor(inputuv.y);

			// DEBUG
			int index = 0;
			if(vc.g > max(vc.b, vc.r))
				index = 1;
			else if(vc.b > max(vc.r, vc.g))
				index = 2;
			
			float3 col2 = float3(0,0,0);
			if(index == 0)
			col2 = float3(1, 0, 0);
			if(index == 1)
			col2 = float3(0, 1, 0);
			if(index == 2)
			col2 = float3(0, 0, 1);
			vc = lerp(vc, col2, _ColorMode);
			
			float2 size = float2(1 / _GridScale, 1 / _GridScale);
			float pad = _Padding * _GridScale;
			float2 initialOffset = size * 0.5 + float2(pad, pad);
			float2 uv0 = initialOffset + float2(fmod(texid0, _GridScale), floor(texid0 / _GridScale)) * size;
			float2 uv1 = initialOffset + float2(fmod(texid1, _GridScale), floor(texid1 / _GridScale)) * size;
			float2 uv2 = initialOffset + float2(fmod(texid2, _GridScale), floor(texid2 / _GridScale)) * size;
			//float2 uv3 = initialOffset + float2(texid3 % _GridScale, floor(texid3 / _GridScale)) * size;

			size -= float2(pad*2, pad*2);

			float3 c0 = TriplanarSamplingCF(_MainTex, IN.pos, IN.norm, uv0, size);
			float3 c1 = TriplanarSamplingCF(_MainTex, IN.pos, IN.norm, uv1, size);
			float3 c2 = TriplanarSamplingCF(_MainTex, IN.pos, IN.norm, uv2, size);
			
			col.rgb = 
				c0 * vc.r +
				c1 * vc.g +
				c2 * vc.b;

				col.rgb = lerp(col.rgb, vc, _Mode);
		    o.Albedo = col;
		}
		ENDCG
	}
	FallBack"Diffuse"
}
