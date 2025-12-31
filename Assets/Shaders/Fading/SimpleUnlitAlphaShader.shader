Shader "Unlit/SimpleUnlitAlphaShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _Alpha ("Alpha", Range(0,1)) = 1
        _AmbientIntensity ("AI", Float) = .25
    }
    SubShader
    {
        Tags {     "Queue"="Transparent"
            "RenderType"="Transparent" }
        
        Blend SrcAlpha OneMinusSrcAlpha
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 normal : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _Color;
            float _Alpha, _AmbientIntensity;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.normal = UnityObjectToWorldNormal(v.normal);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 diffuse = saturate(dot(i.normal, float3(1,0,0)));
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;
                float3 ambient = _AmbientIntensity * col;
                float3 albedo = col.rgb * diffuse + ambient;
                return float4(albedo, _Alpha);
            }
            ENDCG
        }
    }
}
