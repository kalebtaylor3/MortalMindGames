Shader "Custom/PageEmission"
{
    Properties{
       _TintColor("Tint Color", Color) = (0.500000,0.500000,0.500000,0.500000)
       _MainTex("Particle Texture", 2D) = "white" { }
       _InvFade("Soft Particles Factor", Range(0.010000,3.000000)) = 1.000000
       _EmissionMap("Emission Map", 2D) = "white" {}
       _EmissionColor("Emission Color", Color) = (1, 1, 1, 1)
       _Emission("Emission", Range(0, 5)) = 1
    }
        SubShader{
            Tags { "QUEUE" = "Transparent" "IGNOREPROJECTOR" = "true" "RenderType" = "Transparent" "PreviewType" = "Plane" }
            ZWrite Off
            Cull Off
            Blend SrcAlpha OneMinusSrcAlpha
            ColorMask RGB
           // Add new texture sampler for emission map
           sampler2D _EmissionMap;
       // Add new color property for emission color
       fixed4 _EmissionColor;
       // Add new float property for emission intensity
       float _Emission;

       // Add new input structure for emission map
       struct Input {
           float2 uv_MainTex;
           float2 uv_EmissionMap;
       };

       // Modify the existing vertex program to also sample the emission map
       // and pass it to the fragment program
       void vert(inout appdata_full v, out Input o) {
           UNITY_INITIALIZE_OUTPUT(Input,o);
           o.uv_MainTex = v.texcoord.xy;
           o.uv_EmissionMap = v.texcoord.xy;
           // existing vertex program
           mul_sat v.color, v.color, _TintColor;
           v.vertex.xyz = v.vertex.xyz + _MainTex.Sample(sampler_MainTex, o.uv_MainTex).rgb * _InvFade;
       }
       // Modify the existing fragment program to also sample the emission map
       // and add it to the final color output
       fixed4 frag(Input i) : COLOR{
           fixed4 col = tex2D(_MainTex, i.uv_MainTex);
           fixed4 emission = tex2D(_EmissionMap, i.uv_EmissionMap);
           col.rgb += _EmissionColor.rgb * _Emission * emission.r;
           return col;
       }
           ENDCG
       }
           FallBack "Particles/Alpha Blended"
}