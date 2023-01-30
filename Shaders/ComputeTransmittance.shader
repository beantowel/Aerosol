Shader "Aerosol/ComputeTransmittance"
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
                output.texcoords = v.uv * TRANSMITTANCE_TEXTURE_SIZE;
                return output;
            }

            float3 frag(VS_OUTPUT input) : SV_Target
            {
                return ComputeTransmittanceToTopAtmosphereBoundaryTexture(
                    ATMOSPHERE, input.texcoords);
            }
            ENDHLSL
        }
    }
}
