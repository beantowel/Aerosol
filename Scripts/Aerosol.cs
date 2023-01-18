using System.IO;
using UnityEditor;
using UnityEngine;
namespace Aerosol {
    [ExecuteAlways]
    class Aerosol : MonoBehaviour {
        public Assets Assets;

        Model model;

        void Awake() {
            Init();
        }

        void Init() {
            model = new Model(Const.DefaultParam(), Assets);
            model.Init();
        }

        [MenuItem("Window/Aerosol/GenHLSLHeader")]
        static void GenHLSLHeader() {
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