using System.IO;
using UnityEditor;
using UnityEngine;
namespace Aerosol {
    [ExecuteAlways]
    class Aerosol : MonoBehaviour {
        public Assets Assets;
        static Model model;

        void Awake() {
            Init();
        }

        void Init() {
            model = new Model(Const.DefaultParam(), Assets);
            model.Init();
            Assets.Skybox.SetTexture("transmittance_texture", model.Transmittance);
            Assets.Skybox.SetTexture("scattering_texture", model.Scattering);
            Assets.Skybox.SetTexture("irradiance_texture", model.Irradiance);
        }

        [ContextMenu("GenHeader")]
        void GenHeader() {
            var path = Path.Combine(Application.dataPath,
                "beantowel/Aerosol/Shaders/header.hlsl");
            var header = Model.Header(Const.DefaultParam(), Const.Lambdas);
            File.WriteAllText(path, header);
        }

        void OnValidate() {
            if (model == null) {
                Init();
            } else {
                model.Init();
            }
        }
    }
}