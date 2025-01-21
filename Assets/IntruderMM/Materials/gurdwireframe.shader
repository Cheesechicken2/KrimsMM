Shader "gurdwireframe"
{
    Properties
    {
        _WireThickness ("Wire Thickness", RANGE(0, 800)) = 100
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma geometry geom
            #pragma fragment frag

            #include "UnityCG.cginc"

            float _WireThickness;

            struct appdata
            {
                float4 vertex : POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2g
            {
                float4 projectionSpaceVertex : SV_POSITION;
                float4 worldSpacePosition : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO_EYE_INDEX
            };

            struct g2f
            {
                float4 projectionSpaceVertex : SV_POSITION;
                float4 worldSpacePosition : TEXCOORD0;
                float4 dist : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            v2g vert (appdata v)
            {
                v2g o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_OUTPUT_STEREO_EYE_INDEX(o);

                o.projectionSpaceVertex = UnityObjectToClipPos(v.vertex);
                o.worldSpacePosition = mul(unity_ObjectToWorld, v.vertex);
                return o;
            }

            [maxvertexcount(3)]
            void geom(triangle v2g i[3], inout TriangleStream<g2f> triangleStream)
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i[0]);

                float2 p0 = i[0].projectionSpaceVertex.xy / i[0].projectionSpaceVertex.w;
                float2 p1 = i[1].projectionSpaceVertex.xy / i[1].projectionSpaceVertex.w;
                float2 p2 = i[2].projectionSpaceVertex.xy / i[2].projectionSpaceVertex.w;

                float2 edge0 = p2 - p1;
                float2 edge1 = p2 - p0;
                float2 edge2 = p1 - p0;

                float area = abs(edge1.x * edge2.y - edge1.y * edge2.x);
                float wireThickness = 800 - _WireThickness;

                g2f o;
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.worldSpacePosition = i[0].worldSpacePosition;
                o.projectionSpaceVertex = i[0].projectionSpaceVertex;
                o.dist.xyz = float3( (area / length(edge0)), 0.0, 0.0) * o.projectionSpaceVertex.w * wireThickness;
                o.dist.w = 1.0 / o.projectionSpaceVertex.w;
                triangleStream.Append(o);

                o.worldSpacePosition = i[1].worldSpacePosition;
                o.projectionSpaceVertex = i[1].projectionSpaceVertex;
                o.dist.xyz = float3(0.0, (area / length(edge1)), 0.0) * o.projectionSpaceVertex.w * wireThickness;
                o.dist.w = 1.0 / o.projectionSpaceVertex.w;
                triangleStream.Append(o);

                o.worldSpacePosition = i[2].worldSpacePosition;
                o.projectionSpaceVertex = i[2].projectionSpaceVertex;
                o.dist.xyz = float3(0.0, 0.0, (area / length(edge2))) * o.projectionSpaceVertex.w * wireThickness;
                o.dist.w = 1.0 / o.projectionSpaceVertex.w;
                triangleStream.Append(o);
            }

            fixed4 frag (g2f i) : SV_Target
            {
                float minDistanceToEdge = min(i.dist[0], min(i.dist[1], i.dist[2])) * i.dist[3];

                if(minDistanceToEdge > 0.9)
                {
                    return fixed4(0,0,0,0);
                }

                float t = exp2(-2 * minDistanceToEdge * minDistanceToEdge);

                const fixed4 colors[11] = {
                    fixed4(0.1, 0.2, 0.6, 0.05),  // Solid Blue (Low Alpha)
                    fixed4(0.1, 0.2, 0.6, 0.05),  // Solid Blue (Low Alpha)
                    fixed4(0.1, 0.2, 0.6, 0.05),  // Solid Blue (Low Alpha)
                    fixed4(0.1, 0.2, 0.6, 0.05),  // Solid Blue (Low Alpha)
                    fixed4(0.1, 0.2, 0.6, 0.05),  // Solid Blue (Low Alpha)
                    fixed4(0.1, 0.2, 0.6, 0.05),  // Solid Blue (Low Alpha)
                    fixed4(0.1, 0.2, 0.6, 0.05),  // Solid Blue (Low Alpha)
                    fixed4(0.1, 0.2, 0.6, 0.05),  // Solid Blue (Low Alpha)
                    fixed4(0.1, 0.2, 0.6, 0.05),  // Solid Blue (Low Alpha)
                    fixed4(0.1, 0.2, 0.6, 0.05),  // Solid Blue (Low Alpha)
                    fixed4(0.1, 0.2, 0.6, 0.05)   // Solid Blue (Low Alpha)
                };

                float cameraToVertexDistance = length(_WorldSpaceCameraPos - i.worldSpacePosition);
                int index = clamp(floor(cameraToVertexDistance), 0, 10);
                fixed4 wireColor = colors[index];

                fixed4 solidColor = fixed4(0.1, 0.2, 0.6, 0.05);
                fixed4 finalColor = lerp(solidColor, wireColor, t);
                finalColor.a = t;

                return finalColor;
            }
            ENDCG
        }
    }
}
