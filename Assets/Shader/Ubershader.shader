// ImD Ubershader by Wolfgang Reichardt
// contact@dotmos.org
//
// v 0.4
//
// Just some features, no optimizations yet

Shader "Custom/ImD Ubershader" {
Properties {
	_Color ("Main Color (RGB) Alpha (A)", Color) = (1,1,1,1)
	_AlphaCutoff ("Alpha Cutoff", Range(0,1)) = 0.5
	_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
	_Shininess ("Specular Shininess", Range (0.03, 1)) = 0.078125
	_EmissiveColor ("Emissive Color", Color) = (1,0,0,0)
	_MainTex ("Base (RGB)", 2D) = "white" {}
	_BumpMap ("Normalmap", 2D) = "bump" {}
	_BumpDetailMap ("Detail Normalmap", 2D) = "bump" {}
	_DataMap ("Specular (R) Gloss (G) Emissive (B)", 2D) = "black" {}
}
SubShader { 
	Tags { "RenderType"="Opaque" }
	LOD 400
	Cull Off
	
CGPROGRAM
#pragma surface surf BlinnPhong
#pragma target 3.0


sampler2D _MainTex;
sampler2D _BumpMap;
sampler2D _BumpDetailMap;
sampler2D _DataMap;
fixed4 _Color;
fixed4 _EmissiveColor;
half _AlphaCutoff;
half _Shininess;
fixed _DetailTiling;

struct Input {
	float2 uv_MainTex;
	float2 uv_BumpMap;
	float2 uv_BumpDetailMap;
};

void surf (Input IN, inout SurfaceOutput o) {
	fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
	clip(tex.a-(1-_Color.a));
	fixed4 data = tex2D(_DataMap, IN.uv_MainTex);
	//tex.rgb = clamp(tex.rgb - data.z, 0,1);
	o.Albedo = tex.rgb * _Color.rgb;
	o.Gloss = data.r;
	o.Alpha = tex.a * _Color.a;
	o.Specular = data.g * _Shininess;
	
	//DETAIL NORMALMAPPING
	half3 normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
	half3 detailNormal = UnpackNormal(tex2D(_BumpDetailMap, IN.uv_BumpDetailMap));
	/*
	//Detail oriented detail-normalmap blending. See here http://blog.selfshadow.com/publications/blending-in-detail/
	float3 t = normal*float3( 2,  2, 2) + float3(-1, -1,  0);
	float3 u = detailNormal*float3(-2, -2, 2) + float3( 1,  1, -1);
	//float3 r = t*dot(t, u)/t.z - u;
	float3 r = normalize(t*dot(t, u) - u*t.z);
	o.Normal = r*0.5 + 0.5;
	*/
	//UDN detail-normalmap blending. See here http://blog.selfshadow.com/publications/blending-in-detail/
	o.Normal = normalize(float3(normal.xy + detailNormal.xy, normal.z));
	//--------------------------
	
	o.Emission = data.z * _EmissiveColor.rgb * 2;
	
	//Phat Dissolve effect :D
	//o.Emission += pow(1-(tex.a - (1-_Color.a)),50)*_EmissiveColor.rgb * 2;
}
ENDCG
}

FallBack "Specular"
}

