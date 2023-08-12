Shader "Unlit/RainbowHeight"
{     
    Properties  
    {
        [NoScaleOffset] _MainTex ("Texture", 2D) = "white" {}
        _Height ("Height", Range(0,1)) = 1 // so we don't have to remap it. (0,1) is easier for the math    
        _BoarderSize("BoarderSize", Range(0,0.5)) = 1
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
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float _Height;
            float _BoarderSize;

            Interpolators vert (MeshData v)
            {
                Interpolators o;
                o.vertex = UnityObjectToClipPos(v.vertex);                    
                o.uv = v.uv;                   
                return o;
            }

            float InverseLerp(float a, float b, float v)
            {
                return (v-a)/(b-a);
            }


            float4 frag (Interpolators i) : SV_Target
            {
                // rounded corner clipping   
                float2 coords = i.uv;
                coords.x *= 8;
                float2 pointOnLineSeg = float2 ( clamp(coords.x, 0.5, 7.5), 0.5);
                float sdf = distance(coords, pointOnLineSeg)  * 2 - 1;
                clip(-sdf);

                // Boarder
                float boarderSdf = sdf + _BoarderSize;
                float pd = fwidth(boarderSdf); // screen space partial derivate  
                float boarderMask = saturate(boarderSdf / pd);

                // Create mask which changes how health changes -> across what does it change
                float heightbarMask = _Height > i.uv.x; 
                clip(heightbarMask - 0.5); // transparency without sorting issues what you normally get with transparency (still writing to the zBuffer)              
                float3 heightbarColor = tex2D(_MainTex, float2(_Height, i.uv.y));
      
                // Flash if hit out of range
                if(_Height< 0.05)
                {
                    float flash = cos(_Time.y * 4) * 0.4 + 1;
                    heightbarColor *= flash;
                }

                if(_Height > 0.95)
                {
                    float flash = cos(_Time.y * 4) * 0.4 + 1;
                    heightbarColor *= flash;
                }
 
                return float4(heightbarColor * heightbarMask * boarderMask, 1);
            }
            ENDCG
        }
    }
}
