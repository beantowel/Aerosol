Shader "Aerosol/ComputeDirectIrradiance"
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
                float3 delta_irradiance : SV_Target0;
                float3 irradiance : SV_Target1;
            };

            VS_OUTPUT vertex(VS_INPUT v)
            {
                VS_OUTPUT output;
                output.pos = UnityObjectToClipPos(v.vertex);
                output.texcoords = v.uv * IRRADIANCE_TEXTURE_SIZE;
                return output;
            }

            PS_OUTPUT frag(VS_OUTPUT input)
            {
                PS_OUTPUT output;
                output.delta_irradiance = ComputeDirectIrradianceTexture(
                    ATMOSPHERE, transmittance_texture, input.texcoords);
                output.irradiance = 0;
                return output;
            }
            ENDHLSL
        }
    }
}
