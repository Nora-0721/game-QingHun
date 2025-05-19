Shader "Custom/AlphaGradient" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)   // 包含Alpha控制
        _AlphaGradient ("Alpha Gradient", Range(0, 1)) = 0.5 // 控制渐变方向
    }
    SubShader {
        Tags { 
            "Queue"="Transparent" 
            "RenderType"="Transparent" 
        }
        Blend SrcAlpha OneMinusSrcAlpha // 启用Alpha混合
        LOD 100

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float _AlphaGradient;

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;
                // 计算水平渐变（从左边到右边）
                float alpha = lerp(i.uv.x, 1 - i.uv.x, _AlphaGradient);
                col.a *= alpha; // 将渐变Alpha与材质Color的Alpha结合
                return col;
            }
            ENDCG
        }
    }
}