Shader "Unlit/ObstacleShader"
{
    Properties
    {
        _BaseColor("Base Color", Color) = (1,1,1,1)

        [NoScaleOffset]
        _NoiseTex("Noise Texture", 2D) = "black" {}

        _MinDisp("Minimum Displacement", Range(0, 1)) = 0
        _MaxDisp("Maximum Displacement", Range(0, 1)) = 0

        _Excitement("Excitement", Range(0, 1)) = 0

    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"


            struct vertdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAl;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            float4 _BaseColor;
            sampler2D _NoiseTex;
            float _MinDisp;
            float _MaxDisp;
            float _Excitement;


            v2f vert (vertdata v)
            {
                v2f o;
                float4 newPos = v.vertex; 


                // sample the noise texture (several times?)
                //float4 noise = tex2Dlod(_NoiseTex, float4(frac(newPos.xy * _Time.y/2), 0, 0)); 

                //float3 awayFromCenter = normalize((newPos - float4(0, 0, 0, 0)).xxxx);

                //newPos.x += awayFromCenter * noise.x * lerp(_MinDisp, _MaxDisp, _Excitement);

                o.vertex = UnityObjectToClipPos(newPos);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float4 col = _BaseColor;
                return col;
            }
            ENDCG
        }
    }
}
