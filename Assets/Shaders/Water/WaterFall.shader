Shader "Unlit/WaterFall"
{
    Properties
    {
        _Color1 ("Color 1", Color) = (1,1,1,1)
        _Color2 ("Color 2", Color) = (1,1,1,1)
        _Color3 ("Color 3", Color) = (1,1,1,1)
        _Color4 ("Color 4", Color) = (1,1,1,1)
        _NoiseScale ("Noise Scale", Float) = 1
        _TimeScale ("Time Scale", Float) = 1
        _Threshold ("Threshold", Float) = 0.1
        _Alpha ("Alpha", Range(0, 1)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #define RANDOM1 12.9898
            #define RANDOM2 78.233
            #define RANDOM3 43758.54531213
            
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            float _NoiseScale, _TimeScale, _Threshold, _Alpha;
            float4 _Color1, _Color2, _Color3, _Color4;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float random(float2 st)
            {
                float randomVal = frac(sin(dot(st.xy, float2(RANDOM1, RANDOM2))) * RANDOM3);
                return  randomVal;
            }
            
            float simple_Noise(float2 st)
            {
                float2 i = floor(st);
                float2 f = frac(st);

                float a = random(i);
                float b = random(i + float2(1.0, 0.0));
                float c = random(i + float2(0.0, 1.0));
                float d = random(i + float2(1.0, 1.0));

                float2 u = f * f * (3.0 - 2.0 * f);
                return lerp(lerp(a, b, u.x), lerp(c, d, u.x), u.y);
            }

            float4 frag (v2f i) : SV_Target
            {
                float2 displacementWaterfall = simple_Noise(i.uv * _NoiseScale + (_Time.y/_TimeScale));
                float noise = simple_Noise(i.uv * _NoiseScale + (_Time.y/_TimeScale)).x;
                noise = round(noise * 5.0) / 5.0;
                float4 final = lerp(lerp(_Color1, _Color2, i.uv.y), lerp(_Color3, _Color4, i.uv.y), noise);
                final = lerp(float4(1,1,1,1), final, step(_Threshold, i.uv.y + simple_Noise(i.uv * _NoiseScale)));
                return float4(final.xyz, _Alpha);
            }
            ENDCG
        }
    }
}
