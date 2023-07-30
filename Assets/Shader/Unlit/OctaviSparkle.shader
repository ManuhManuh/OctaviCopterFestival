Shader "Octavi/OctaviSparkle"
{
    Properties
    {
        [Header(Sparkles)]
        _SparkleDepth ("Sparkle Depth", Range(0, 5)) = 1
        _NoiseScale("nois Scale", Range(0, 5)) = 1
        _AnimSpeed ("Animation Speed", Range( 0, 5)) = 1

         _MainTex ("MainTex", 2D) = "white" {}

        
    }
   
	SubShader
	{
		Tags { "RenderType"="Opaque"}
		LOD 100

		Pass
		{
			//Tags { "LightMode" = "ForwardBase" }
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			#include "Simplex3D.cginc"
			#include "SparklesCG.cginc"

			struct MeshData
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL;     
			};

			struct Interpolators
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float3 wPos : TEXCOORD1;
				float3 pos : TEXCOORD3;
				float3 normal : NORMAL;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _Color, _SpecColor;
			float _SpecPow, _GlitterPow;

			Interpolators vert (MeshData v)
			{
				Interpolators o;
				
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.wPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				o.pos = v.vertex;
				o.normal = mul(unity_ObjectToWorld, float4(v.normal,0)).xyz;
				o.vertex = UnityObjectToClipPos(v.vertex);  
				return o; 
			}
			
			fixed4 frag (Interpolators i) : SV_Target
			{
				//Sparkles
				float3 viewDir = normalize(UnityWorldSpaceViewDir(i.wPos));
				float sparkles = Sparkles(viewDir,i.wPos);

				return sparkles;
			}
			ENDCG
		}
	}
}
