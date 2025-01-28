Shader "Custom/DoodleEffectWithAlpha"
{
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _NoiseScale ("Noise Scale", Float) = 0.1
        _NoiseSnap ("Noise Snap", Float) = 0.2
    }
    SubShader {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha // Enable alpha blending for transparency

        Pass {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc" // Standard functions

            // Properties
            sampler2D _MainTex; // Texture sampler
            float _NoiseScale;
            float _NoiseSnap;

            // Random function
            float rand(float2 co) {
                return frac(sin(dot(co.xy, float2(12.9898, 78.233))) * 43758.5453);
            }

            // Snapping function
            float snap(float x, float snapValue) {
                return snapValue * round(x / snapValue);
            }

            struct appdata_t {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0; // Include texture coordinates
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0; // Pass texture coordinates to the fragment shader
            };

            // Vertex shader
            v2f vert(appdata_t v) {
                v2f o;
                float time = snap(_Time.y, _NoiseSnap);
                float2 noise = float2(
                    rand(v.vertex.xy + time),
                    rand(v.vertex.xy + time * 2.0)
                ) * _NoiseScale;
                v.vertex.xy += noise; // Apply wobble effect
                o.pos = UnityObjectToClipPos(v.vertex); // Transform to clip space
                o.uv = v.uv; // Pass texture coordinates
                return o;
            }

            // Fragment shader
            fixed4 frag(v2f i) : SV_Target {
                fixed4 col = tex2D(_MainTex, i.uv); // Sample the texture
                col.a *= col.rgba.a; // Use the texture's alpha for transparency
                return col; // Output the texture color with correct alpha
            }
            ENDHLSL
        }
    }
}