using UnityEngine;
namespace Aerosol {
    [CreateAssetMenu(fileName = "AerosolConfig", menuName = "ScriptableObjects/AerosolConfig")]
    class Config : ScriptableObject {
        public Material Skybox;
        public Shader ComputeTransmittance;
        public Shader ComputeDirectIrradiance;
        public Shader ComputeSingleScattering;
        public Shader ComputeScatteringDensity;
        public Shader ComputeIndirectIrradiance;
        public Shader ComputeMultipleScattering;
        public ModelParams Params = Const.DefaultParam();

        public void OnEnable() {
            ComputeTransmittance ??= Shader.Find("Aerosol/ComputeTransmittance");
            ComputeDirectIrradiance ??= Shader.Find("Aerosol/ComputeDirectIrradiance");
            ComputeSingleScattering ??= Shader.Find("Aerosol/ComputeSingleScattering");
            ComputeScatteringDensity ??= Shader.Find("Aerosol/ComputeScatteringDensity");
            ComputeIndirectIrradiance ??= Shader.Find("Aerosol/ComputeIndirectIrradiance");
            ComputeMultipleScattering ??= Shader.Find("Aerosol/ComputeMultipleScattering");
        }
    }
}