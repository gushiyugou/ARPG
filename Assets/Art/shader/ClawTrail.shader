// 示例性代码框架，突出关键步骤
Shader "Custom/Particles/ClawTrail"
{
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _TintColor ("Tint Color", Color) = (1,1,1,1)
        _Emission ("Emission", Float) = 2.0
        _Cutoff ("Alpha Cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };
            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _TintColor;
            half _Emission;
            half _Cutoff;

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                fixed4 col = tex2D(_MainTex, i.uv); // 采样纹理
                col *= _TintColor * i.color; // 叠加顶点色和色调
                col.rgb *= _Emission; // 自发光强度
                clip(col.a - _Cutoff); // Alpha裁剪，实现锐利边缘
                return col;
            }
            ENDCG
        }
    }
}