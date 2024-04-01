Shader "Custom/TransparentRealisticWaterShader" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _WaveSpeed ("Wave Speed", Float) = 1
        _WaveHeight ("Wave Height", Float) = 0.1
        _WaveFrequency ("Wave Frequency", Float) = 1
        _Color ("Color", Color) = (0, 0, 1, 0.5)
        _ReflectiveColor ("Reflective Color", Color) = (1, 1, 1, 0.5)
        _ReflectiveStrength ("Reflective Strength", Range(0, 1)) = 0.5
        _Cube ("Reflection Cubemap", Cube) = "" {}
    }
    SubShader {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        LOD 200
        
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        ColorMask RGB

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f {
                float2 texcoord : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 worldNormal : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
            };

            sampler2D _MainTex;
            float _WaveSpeed;
            float _WaveHeight;
            float _WaveFrequency;
            fixed4 _Color;
            fixed4 _ReflectiveColor;
            float _ReflectiveStrength;
            samplerCUBE _Cube;

            // Improved Perlin noise function
            float ImprovedPerlinNoise(float2 uv) {
                float2 p = floor(uv);
                float2 f = frac(uv);
                f = f * f * (3.0 - 2.0 * f);
                float n = p.x + p.y * 57.0;
                return lerp(lerp(frac(sin(n) * 43758.5453), frac(sin((n + 1.0) * 43758.5453)), f.x),
                            lerp(frac(sin((n + 57.0) * 43758.5453)), frac(sin((n + 58.0) * 43758.5453)), f.x), f.y);
            }

            v2f vert(appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldNormal = mul((float3x3)unity_WorldToObject, v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

                // Calculate wave offset based on time
                float waveOffset = _WaveSpeed * _Time.y;

                // Improved wave calculation using Perlin noise
                float waveHeight = _WaveHeight * (ImprovedPerlinNoise(v.texcoord * _WaveFrequency + float2(waveOffset, 0)) * 2.0 - 1.0);
                o.vertex.y += waveHeight;

                o.texcoord = v.texcoord;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target {
                // Sample the texture
                fixed4 c = tex2D (_MainTex, i.texcoord);
                // Calculate reflection
                fixed4 reflectiveColor = texCUBE(_Cube, reflect(-normalize(i.worldPos), i.worldNormal));
                // Final color with reflection
                fixed4 finalColor = c * _Color + reflectiveColor * _ReflectiveColor * _ReflectiveStrength;
                // Increase transparency
                finalColor.a *= _Color.a * 0.3; // Adjust this value as needed for desired transparency
                return finalColor;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
