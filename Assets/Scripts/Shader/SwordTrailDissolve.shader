Shader "Custom/SwordTrailDissolveStable"
{
    Properties
    {
        _MainTex ("主纹理", 2D) = "white" {}
        _NoiseTex ("噪声纹理", 2D) = "white" {}
        _BaseColor ("基础颜色", Color) = (1,1,1,1)
        _DissolveAmount ("溶解程度", Range(0,1)) = 0
        _EdgeColor ("边缘颜色", Color) = (1,0.5,0,1)
        _EdgeWidth ("边缘宽度", Range(0,0.2)) = 0.1
        _NoiseScale ("噪声缩放", Float) = 1
        _TimeScale ("时间缩放", Float) = 1
    }
    
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off
        
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
                float2 uvNoise : TEXCOORD1;
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD2;
            };
            
            sampler2D _MainTex;
            sampler2D _NoiseTex;
            float4 _MainTex_ST;
            float4 _NoiseTex_ST;
            float4 _BaseColor;
            float _DissolveAmount;
            float4 _EdgeColor;
            float _EdgeWidth;
            float _NoiseScale;
            float _TimeScale;
            
            // 稳定的噪声采样函数
            float stableNoise(sampler2D tex, float2 uv, float time)
            {
                // 使用多重采样减少锯齿
                float2 dx = ddx(uv) * 2.0;
                float2 dy = ddy(uv) * 2.0;
                
                float4 noise = 0;
                noise += tex2Dgrad(tex, uv, dx, dy);
                noise += tex2Dgrad(tex, uv + float2(0.25, 0.25), dx, dy);
                noise += tex2Dgrad(tex, uv + float2(0.5, 0.5), dx, dy);
                noise += tex2Dgrad(tex, uv + float2(0.75, 0.75), dx, dy);
                
                return noise.r * 0.25;
            }
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                
                // 稳定的噪声UV计算
                o.uvNoise = v.uv * _NoiseScale;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * _BaseColor;
                
                // 稳定的噪声采样
                float2 noiseUV = i.uvNoise;
                float time = _Time.y * _TimeScale;
                
                // 使用世界坐标来避免UV拉伸导致的抖动
                noiseUV += i.worldPos.xz * 0.1;
                noiseUV += time * 0.5;
                
                float noise = stableNoise(_NoiseTex, noiseUV, time);
                
                // 更平滑的溶解计算
                float dissolve = noise - _DissolveAmount;
                
                // 使用更精确的clip阈值
                clip(dissolve + 0.001);
                
                // 平滑的边缘过渡
                float edge = smoothstep(0, _EdgeWidth, dissolve);
                float edgeStrength = 1.0 - edge;
                
                // 避免过于尖锐的边缘
                edgeStrength = smoothstep(0, 1, edgeStrength);
                
                // 边缘颜色混合
                col.rgb = lerp(col.rgb, _EdgeColor.rgb, edgeStrength * _EdgeColor.a);
                
                // 透明度控制
                col.a *= saturate(dissolve / _EdgeWidth);
                col.a = smoothstep(0, 1, col.a); // 额外的平滑处理
                
                return col;
            }
            ENDCG
        }
    }
}