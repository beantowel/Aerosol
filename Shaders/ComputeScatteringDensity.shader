Shader "Aerosol/ComputeScatteringDensity"
{
    Properties
    {
    }

    SubShader
    {
        Pass
        {
            Cull Off ZWrite Off ZTest Always
            HLSLPROGRAM
            #pragma vertex vertex
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "functions.hlsl"

            sampler2D transmittance_texture;
            sampler3D single_rayleigh_scattering_texture;
            sampler3D single_mie_scattering_texture;
            sampler3D multiple_scattering_texture;
            sampler2D irradiance_texture;
            int scattering_order;
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

            VS_OUTPUT vertex(VS_INPUT v)
            {
                VS_OUTPUT output;
                output.pos = UnityObjectToClipPos(v.vertex);
                output.texcoords = v.uv * SCATTERING_TEXTURE_SIZE.xy;
                return output;
            }

            float3 frag(VS_OUTPUT input) : SV_Target
            {
                return ComputeScatteringDensityTexture(
                    ATMOSPHERE, transmittance_texture, single_rayleigh_scattering_texture,
                    single_mie_scattering_texture, multiple_scattering_texture,
                    irradiance_texture, float3(input.texcoords, layer + 0.5),
                    scattering_order);
            }
            ENDHLSL
        }
    }
}
