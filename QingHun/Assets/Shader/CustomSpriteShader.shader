Shader "Custom/LineClip"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ClipRight ("Clip Right", Range(0,1)) = 0.51
        _ClipLeft ("Clip Left", Range(0,1)) = 0.49
    }
    SubShader
    {
        Tags { "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            sampler2D _MainTex;
            float _ClipRight;
            float _ClipLeft;

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                // 根据UV坐标裁剪左右部分
                clip(i.uv.x - _ClipLeft);
                clip(_ClipRight - i.uv.x);
                return col;
            }
            ENDCG
        }
    }
}