Shader "Custom/VertexColorEmissiveWithAlpha" {
    Properties{
        _Color("Color", Color) = (1,1,1,1)
        _Emission("Emission", Color) = (0,0,0,1)
        _EmissionMultiplier("Emission Multiplier", Range(0, 10)) = 1
        _Alpha("Alpha", Range(0,1)) = 1
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
                float4 _Color;
                float4 _Emission;
                float _EmissionMultiplier;
                float _Alpha;

                v2f vert(appdata v) {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.color = v.color * _Color;
                    if (v.color.r > 0) {
                        o.color.rgb += _Emission.rgb * _EmissionMultiplier;
                    }
                    o.color.a *= _Alpha;
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