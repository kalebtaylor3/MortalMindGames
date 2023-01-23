Shader "Custom/VertexColorSmoothRoundedEdge" {
    Properties{
        _Color1("Color1", Color) = (0,0,0,1)
        _Color2("Color2", Color) = (1,0,0,1)
        _BlendValue("Blend Value", Range(0,1)) = 0.5
        _EdgeRadius("Edge Radius", Range(0,1)) = 0.1
    }

        SubShader{
            Tags {"Queue" = "Transparent" "RenderType" = "Transparent"}
            LOD 100

            Pass {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"

                struct appdata {
                    float4 vertex : POSITION;
                    float4 color : COLOR;
                };

                struct v2f {
                    float2 uv : TEXCOORD0;
                    float4 color : COLOR;
                    float4 vertex : SV_POSITION;
                };

                sampler2D _MainTex;
                float4 _Color1;
                float4 _Color2;
                float _BlendValue;
                float _EdgeRadius;

                v2f vert(appdata v) {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.color = lerp(v.color * _Color1, v.color * _Color2, smoothstep(0, _EdgeRadius, _BlendValue));
                    o.uv = v.vertex.xy * 0.5 + 0.5;
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target {
                    fixed4 col = tex2D(_MainTex, i.uv) * i.color;
                    if (col.a < 0.1)
                        discard;
                    return col;
                }
                ENDCG
            }
    }
        FallBack "Diffuse"
}