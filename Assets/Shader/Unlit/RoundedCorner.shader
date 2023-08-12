Shader "Unlit/RoundedCorner"
{
    Properties
    {
        [NoScaleOffset] _MainTex ("Texture", 2D) = "white" {}
        

    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        

        Pass
        {
            ZWrite Off // otherwise transparency collission mess   
            Blend SrcAlpha OneMinusSrcAlpha // src * srcAplpha + dst * (1-srcAlpha) -> alphaBlending
            

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
           
            #include "UnityCG.cginc"

            struct MeshData
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Interpolators
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
        
            Interpolators vert (MeshData v)
            {
                Interpolators o;
                o.vertex = UnityObjectToClipPos(v.vertex);                    
                o.uv = v.uv;                   
                return o;
            }
                     
            float4 frag (Interpolators i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
               
                // Rounded Corners
                float2 coords = i.uv;
                coords.x *= 8;
                float2 pointOnLineSeg = float2 ( clamp(coords.x, 0.5, 7.5), 0.5);
                float sdf = distance(coords, pointOnLineSeg)  * 2 - 1;
                clip(-sdf);

                return col;        
   
            }
            ENDCG
        }
    }
}
