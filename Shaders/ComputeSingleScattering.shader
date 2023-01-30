Shader "Aerosol/ComputeSingleScattering"
{
    Properties
    {
    }

    SubShader
    {
        Pass {
            Cull Off ZWrite Off ZTest Always
            HLSLPROGRAM
            #pragma vertex vertex
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "functions.hlsl"

            sampler2D transmittance_texture;
            float4x4 luminance_from_radiance;
            int layer;

            struct VS_INPUT
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct VS_OUTPUT
            {
                float4 pos : SV_POSITION;
                float2 texcoords : TEXCOORD0;
            };

            struct PS_OUTPUT
            {
                float3 delta_rayleigh : SV_Target0;
                float3 delta_mie : SV_Target1;
                float4 scattering : SV_Target2;
            };

            VS_OUTPUT vertex(VS_INPUT v)
            {
                VS_OUTPUT output;
                output.pos = UnityObjectToClipPos(v.vertex);
                output.texcoords = v.uv * SCATTERING_TEXTURE_SIZE.xy;
                return output;
            }

            PS_OUTPUT frag(VS_OUTPUT input)
            {
                PS_OUTPUT output;
                ComputeSingleScatteringTexture(
                    ATMOSPHERE, transmittance_texture, float3(input.texcoords, layer + 0.5),
                    output.delta_rayleigh, output.delta_mie);
                float3x3 lfr = (float3x3)luminance_from_radiance;
                output.scattering = float4(
                    mul(lfr, output.delta_rayleigh),
                    mul(lfr, output.delta_mie).r);
                return output;
            }
            ENDHLSL
        }
    }
}
