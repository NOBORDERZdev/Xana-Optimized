Shader "Custom/Sky360" {
    Properties {
        _MainTex ("Sky texture", 2D) = "white" {}
        _ScrollX ("Texture scroll speed X", Float) = 1.0
        //_ScrollY ("Texture scroll speed Y", Float) = 0.0
        _ColorTint("Color", Color) = (1, 1, 1, 1)
    }

    SubShader {
        Tags { "Queue"="Geometry+800" "RenderType"="Opaque" }
        LOD 100

        CGINCLUDE
        #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
        #include "UnityCG.cginc"

        sampler2D _MainTex;

        float4 _MainTex_ST;
        float _ScrollX;
        float _ScrollY;
        float4 _ColorTint;

        struct v2f {
            float4 pos : SV_POSITION;
            float2 uv : TEXCOORD0;
            fixed4 color : TEXCOORD1;
        };

        v2f vert (appdata_full v) {
            v2f o;
            o.pos = UnityObjectToClipPos(v.vertex);
            o.uv = TRANSFORM_TEX(float2(v.texcoord.x, 1.0 - v.texcoord.y), _MainTex) + frac(float2(_ScrollX, _ScrollY) * _Time);
            o.color = _ColorTint;
            return o;
        }
        ENDCG

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma fragmentoption ARB_precision_hint_fastest

            fixed4 frag (v2f i) : SV_Target {
                fixed4 tex = tex2D(_MainTex, i.uv);
                return tex * i.color;
            }
            ENDCG
        }
    }
}