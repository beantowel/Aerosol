using UnityEngine;
using UnityEngine.Rendering;

namespace Aerosol {
    static class Util {
        static readonly CommandBuffer buffer = new CommandBuffer();
        static readonly Matrix4x4 view = Matrix4x4.identity;
        static readonly Matrix4x4 ortho = Matrix4x4.Ortho(-1, 1, -1, 1, -1, 1);
        static Mesh fullMesh;

        static void InitBuffer() {
            buffer.Clear();
            buffer.SetViewProjectionMatrices(view, ortho);
        }


        public static void DrawRect(Material mat, params RenderTargetIdentifier[] rts) {
            var mesh = FullSceneTri();
            InitBuffer();
            buffer.SetRenderTarget(rts, rts[0]);
            buffer.DrawMesh(mesh, Matrix4x4.identity, mat);
            Graphics.ExecuteCommandBuffer(buffer);
        }

        public static void DrawCube(Material mat, params RenderTargetIdentifier[] rts) {
            var mesh = FullSceneTri();
            for (int i = 0; i < Const.ScatteringTextureSize.Depth; i++) {
                InitBuffer();
                buffer.SetRenderTarget(rts, rts[0], 0, CubemapFace.Unknown, i);
                mat.SetInteger("layer", i);
                buffer.DrawMesh(mesh, Matrix4x4.identity, mat);
                Graphics.ExecuteCommandBuffer(buffer);
            }
        }

        public static RenderTexture NewTexture2D(int width, int height, bool isTemp = false) {
            var desc = new RenderTextureDescriptor(
                width, height, RenderTextureFormat.ARGBFloat, 0);
            return isTemp ?
                RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.ARGBFloat) :
                new RenderTexture(width, height, 0, RenderTextureFormat.ARGBFloat);
        }

        public static RenderTexture NewTexture3D(bool halfPrecision, bool isTemp = false) {
            var desc = new RenderTextureDescriptor(
                Const.ScatteringTextureSize.Width,
                Const.ScatteringTextureSize.Height,
                halfPrecision ? RenderTextureFormat.ARGBHalf : RenderTextureFormat.ARGBFloat, 0);
            desc.dimension = TextureDimension.Tex3D;
            desc.volumeDepth = Const.ScatteringTextureSize.Depth;
            return isTemp ? RenderTexture.GetTemporary(desc) : new RenderTexture(desc);
        }

        public static Mesh FullSceneTri(float scale = 1) {
            if (fullMesh != null) {
                return fullMesh;
            }
            Vector3[] vertices = new Vector3[] {
                new Vector3(-1, -1, 0) * scale,
                new Vector3(3, -1, 0) * scale,
                new Vector3(-1, 3, 0) * scale,
            };
            Vector2[] uv = new Vector2[] {
                new Vector2(0, 0),
                new Vector2(2, 0),
                new Vector2(0, 2),
            };
            int[] triangles = new int[]{
                0, 1, 2
            };
            fullMesh = new Mesh();
            fullMesh.vertices = vertices;
            fullMesh.uv = uv;
            fullMesh.triangles = triangles;
            return fullMesh;
        }
    }
}