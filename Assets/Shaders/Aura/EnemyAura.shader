Shader "Unlit/PS1RiseAura"
{
    Properties
    {
        _BottomColor ("Bottom Color", Color) = (0.3,0.6,1,1)
        _TopColor ("Top Color", Color) = (0,0,0,0)
        _Speed ("Rise Speed", Float) = 1.0
        _Scale ("Noise Scale", Float) = 2.0
        _Amp ("Noise Amp", Float) = 0.4
        _Pixelate ("Pixelation (0 off)", Float) = 64
        _Opacity ("Overall Opacity", Range(0,1)) = 0.6
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100
        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float4 _BottomColor;
            float4 _TopColor;
            float _Speed;
            float _Scale;
            float _Amp;
            float _Pixelate;
            float _Opacity;

            struct appdata { float4 vertex : POSITION; float2 uv : TEXCOORD0; };
            struct v2f { float4 pos : SV_POSITION; float2 uv : TEXCOORD0; };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float hash21(float2 p) {
                p = frac(p * float2(127.1,311.7));
                p += dot(p,p+19.19);
                return frac(p.x*p.y);
            }

            float noise(float2 p) {
                float2 i = floor(p);
                float2 f = frac(p);
                float a = hash21(i+float2(0,0));
                float b = hash21(i+float2(1,0));
                float c = hash21(i+float2(0,1));
                float d = hash21(i+float2(1,1));
                float2 u = f*f*(3.0-2.0*f);
                return lerp(lerp(a,b,u.x),lerp(c,d,u.x),u.y);
            }

            float fbm(float2 p) {
                float v=0.0; float amp=0.5; float freq=1.0;
                for(int i=0;i<4;i++){
                    v += amp*noise(p*freq);
                    freq*=2.0; amp*=0.5;
                }
                return v;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;

                if (_Pixelate>1.0)
                {
                    float2 px = float2(_Pixelate,_Pixelate);
                    uv = floor(uv*px)/px;
                }

                float t = _Time.y * _Speed;
                float2 npos = uv * _Scale + float2(0, -t);
                float n = fbm(npos) * _Amp;

                float mask = smoothstep(0.0,1.0, uv.y + n);
                mask = saturate(mask);

                float3 col = lerp(_BottomColor.rgb, _TopColor.rgb, mask);

                if (col.r+col.g+col.b < 0.01) discard;

                return fixed4(col, mask*_Opacity);
            }
            ENDCG
        }
    }
    FallBack "Unlit/Transparent"
}
