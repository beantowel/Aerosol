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
    }
}