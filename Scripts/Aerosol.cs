using System.IO;
using UnityEngine;
namespace Aerosol {
    [ExecuteAlways]
    class Aerosol : MonoBehaviour {
        public Config Config;
        static Model model;

        void Awake() {
            Init();
        }

        void Init() {
            model = new Model(Config);
            model.Init();
            Config.Skybox.SetTexture("transmittance_texture", model.Transmittance);
            Config.Skybox.SetTexture("scattering_texture", model.Scattering);
            Config.Skybox.SetTexture("irradiance_texture", model.Irradiance);
            Config.Skybox.SetFloat("h_origin", 0);
            RenderSettings.skybox = Config.Skybox;
        }

        [ContextMenu("GenHeader")]
        void GenHeader() {
            var path = Path.Combine(Application.dataPath,
                "beantowel/Aerosol/Shaders/header.hlsl");
            var header = Model.Header(Config.Params, Const.Lambdas);
            File.WriteAllText(path, header);
        }

        public (RenderTexture, RenderTexture, RenderTexture) GetTextures() {
            return (model.Transmittance, model.Irradiance, model.Scattering);
        }
    }
}