Shader "Unlit/PS1_Base"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _AmbientIntensity ("Ambient Intensity", Float) = .25
        _SnapRange1 ("Range 1", Float) = 50.0
        _SnapRange2 ("Range 2", Float) = 60.0
        _TimeSpeed ("Time Speed", Float) = .5
        _BackLight ("Back Light", Float) = .5
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

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float  w  : TEXCOORD1;
                float  light : TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Snap, _AmbientIntensity, _TimeSpeed, _SnapRange1, _SnapRange2, _BackLight;

            v2f vert (appdata v)
            {
                v2f o;
                float4 clipPos = UnityObjectToClipPos(v.vertex);
                float snap = lerp(_SnapRange1, _SnapRange2, sin(_Time.y * _TimeSpeed) * 0.5 + 0.5);
                clipPos.xy = floor(clipPos.xy * snap) / snap;
                o.vertex = clipPos;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex) / clipPos.w;
                o.w  = 1.0 / clipPos.w;
                float3 normal = UnityObjectToWorldNormal(v.normal);
                float3 lightDir = normalize(float3(1, 1, 0));
                float diffuse = max(dot(normal, lightDir), 0) + _BackLight * min(dot(normal, lightDir), 0);
                diffuse = floor(diffuse * 20) / 20;
                o.light = diffuse;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv / i.w;
                fixed4 col = tex2D(_MainTex, uv);
                float3 ambient = _AmbientIntensity * col.xyz;
                float3 albedo = col.xyz * i.light + ambient;
                return fixed4(albedo, 1);
            }
            ENDCG
        }
    }
}
