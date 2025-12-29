Shader "Unlit/PS1Fire"
{
    Properties
    {
        _MainTex ("Main Tex (not required)", 2D) = "white" {}
        _BottomColor ("Bottom Color", Color) = (1,0.6,0,1)
        _MiddleColor ("Middle Color", Color) = (1,0.2,0,1)
        _TopColor ("Top Color", Color) = (1,1,0.2,1)
        _MaskTex ("Mask Texture (grayscale)", 2D) = "white" {}
        _MaskColorA ("Mask Color A", Color) = (0.2,0.05,0.0,1)
        _MaskColorB ("Mask Color B", Color) = (1,0.1,0.0,1)
        _MaskLerp ("Mask Lerp (0 = flame, 1 = maskColors)", Range(0,1)) = 0.0

        _Speed ("Speed", Float) = 1.0
        _Scale ("Noise Scale", Float) = 3.0
        _Amp ("Noise Amp", Float) = 0.45
        _Stretch ("Vertical Stretch", Float) = 1.8
        _Threshold ("Flame Threshold", Range(0,1)) = 0.5
        _Levels ("Color Levels (quantize)", Range(1,12)) = 5
        _Pixelate ("Pixelation (0 disable)", Float) = 64.0
        _Opacity ("Opacity", Range(0,1)) = 1.0

        _MaskTiling ("Mask Tiling", Vector) = (1,1,0,0)
        _MaskOffset ("Mask Offset", Vector) = (0,0,0,0)
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            sampler2D _MaskTex;
            float4 _BottomColor;
            float4 _MiddleColor;
            float4 _TopColor;
            float4 _MaskColorA;
            float4 _MaskColorB;
            float _MaskLerp;

            float _Speed;
            float _Scale;
            float _Amp;
            float _Stretch;
            float _Threshold;
            float _Levels;
            float _Pixelate;
            float _Opacity;
            float4 _MaskTiling;
            float4 _MaskOffset;

            struct appdata { float4 vertex : POSITION; float2 uv : TEXCOORD0; };
            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv  : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv  = v.uv;
                return o;
            }

            float hash21(float2 p) {
                p = frac(p * float2(127.1, 311.7));
                p += dot(p, p + 34.345);
                return frac(p.x * p.y);
            }

            float noise(in float2 p) {
                float2 i = floor(p);
                float2 f = frac(p);
                float a = hash21(i + float2(0,0));
                float b = hash21(i + float2(1,0));
                float c = hash21(i + float2(0,1));
                float d = hash21(i + float2(1,1));
                float2 u = f * f * (3.0 - 2.0 * f);
                return lerp(lerp(a,b,u.x), lerp(c,d,u.x), u.y);
            }

            float fbm(float2 p) {
                float v = 0.0;
                float amp = 0.5;
                float freq = 1.0;
                for (int i = 0; i < 5; ++i) {
                    v += amp * noise(p * freq);
                    freq *= 2.0;
                    amp *= 0.5;
                }
                return v;
            }

            fixed4 palette(float t, float2 uv, float n)
            {
                t = saturate(t + n * 0.6);
                float3 lower = lerp(_BottomColor.rgb, _MiddleColor.rgb, saturate(t * 2.0));
                float3 upper = lerp(_MiddleColor.rgb, _TopColor.rgb, saturate(t * 2.0 - 1.0));
                float3 col = lerp(lower, upper, step(0.5, t));
                return fixed4(col, 1.0);
            }

            fixed4 frag (v2f i) : SV_Target
{
    float2 uv = i.uv;

    if (_Pixelate > 1.0) {
        float2 px = float2(_Pixelate, _Pixelate);
        uv = floor(uv * px) / px;
    }

    float ttime = _Time.y * _Speed;
    float2 npos = float2(uv.x * _Scale + ttime * 0.8, uv.y * _Scale - ttime * 1.3);
    float n = fbm(npos) * _Amp + 0.15 * noise(npos * 3.0);

    float vertical = 1.0 - uv.y;
    float flame = smoothstep(_Threshold - 0.25, _Threshold + 0.25, vertical * _Stretch + n);

    float levels = max(1.0, _Levels);
    float q = floor(flame * levels) / (levels - 1.0);
    q = saturate(q);

    fixed4 flameCol = palette(vertical, uv, n);
    flameCol.rgb *= q;

    float2 maskUV = uv * _MaskTiling.xy + _MaskOffset.xy;
    float maskS = tex2D(_MaskTex, maskUV).r;
    fixed4 maskCol = lerp(_MaskColorA, _MaskColorB, maskS);

    fixed4 finalCol = lerp(flameCol, maskCol, _MaskLerp);

    if (finalCol.r + finalCol.g + finalCol.b < 0.01) discard;

    finalCol.a = _Opacity;
    return finalCol;
}
            ENDCG
        }
    }
    FallBack "Unlit/Transparent"
}
