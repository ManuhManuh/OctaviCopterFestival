Shader "Unlit/ScaleRainbowShader"
{
    Properties // ownDefined inputData   
    {
        
        _Color ("Color", Color) = (1,1,1,1)
        _Scale ("UV Scale", Float) = 1
    }
    SubShader 
    {
        Tags { "RenderType"="Opaque" }
          

        Pass 
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            

            #include "UnityCG.cginc"

            
           float4 _Color;
           float _Scale;

            // automatically filled out by unity
            struct MeshData  // perVertex meshData    
            {
                float4 vertex : POSITION; // vertexPosition   
                float3 normals : NORMAL; // normalDirection of a vertex     
                float4 uv0 : TEXCOORD0; // uv0 coordinates -> diffuse/normal map textures  
                
            };

            struct Interpolaters // v2f    
            { 
                
                float4 vertex : SV_POSITION; // clipSpacePosition of the vertex 
                float3 normal : TEXCOORD0;      

                float2 uv : TEXCOORD1; 
            };   

                                  

            Interpolaters vert (MeshData v)                                                          
            {
                Interpolaters o;
                o.vertex = UnityObjectToClipPos(v.vertex); // localSpace to clipSpace 
                o.normal = UnityObjectToWorldNormal(v.normals); // show normals of the object -> visualize normalDirections           

                o.uv = v.uv0 * _Scale; 
                return o;  
            }

            

            // actual fragmentShader 
            float4 frag (Interpolaters i) : SV_Target
            {          
                return float4(i.uv,0,1);
            }
            ENDCG 
        }
    }
}
