using UnityEngine;
using UnityEngine.Rendering;

namespace Aerosol {
    static class Util {
        static readonly CommandBuffer buffer = new CommandBuffer();
        static readonly Matrix4x4 view = Matrix4x4.identity;
        static readonly Matrix4x4 ortho = Matrix4x4.Ortho(-1, 1, -1, 1, -1, 1);
        static Mesh fullTri;

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

        public static RenderTextureDescriptor Tex2Desc(int width, int height) {
            var desc = new RenderTextureDescriptor(
                width, height, RenderTextureFormat.ARGBHalf, 0);
            desc.sRGB = false;
            return desc;
        }

        public static RenderTextureDescriptor Tex3Desc() {
            var desc = new RenderTextureDescriptor(
                Const.ScatteringTextureSize.Width,
                Const.ScatteringTextureSize.Height,
                RenderTextureFormat.ARGBHalf, 0);
            desc.dimension = TextureDimension.Tex3D;
            desc.volumeDepth = Const.ScatteringTextureSize.Depth;
            desc.sRGB = false;
            return desc;
        }

        public static Mesh FullSceneTri(float scale = 1) {
            if (fullTri != null) {
                return fullTri;
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
            fullTri = new Mesh();
            fullTri.vertices = vertices;
            fullTri.uv = uv;
            fullTri.triangles = triangles;
            return fullTri;
        }
    }
}