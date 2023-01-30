using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Aerosol {
    public struct DensityProfileLayer {
        public double Width;
        public double ExpTerm;
        public double ExpScale;
        public double LinearTerm;
        public double ConstantTerm;
    }

    [System.Serializable]
    public struct ModelParams {
        public List<double> Wavelengths;
        public List<double> SolarIrradiance;
        public double SunAngularRadius;
        public double BottomRadius;
        public double TopRadius;
        public List<DensityProfileLayer> RayleighDensity;
        public List<double> RayleighScattering;
        public List<DensityProfileLayer> MieDensity;
        public List<double> MieScattering;
        public List<double> MieExtinction;
        public double MiePhaseFunctionG;
        public List<DensityProfileLayer> AbsorptionDensity;
        public List<double> AbsorptionExtinction;
        public List<double> GroundAlbedo;
        public double MaxSunZenithAngle;
        public double LengthUnitInMeters;
    }

    class Model {
        public RenderTexture Transmittance;
        public RenderTexture Scattering;
        public RenderTexture Irradiance;
        public Config Conf;

        RenderTextureDescriptor TrnDesc, SctDesc, IrrDesc;
        Material ComputeTransmittance, ComputeDirectIrradiance,
            ComputeSingleScattering, ComputeScatteringDensity,
            ComputeIndirectIrradiance, ComputeMultipleScattering;

        public Model(
            Config conf
        ) {
            Conf = conf;
            TrnDesc = Util.Tex2Desc(Const.TransmittanceTextureSize.Width, Const.TransmittanceTextureSize.Height);
            SctDesc = Util.Tex3Desc();
            IrrDesc = Util.Tex2Desc(Const.IrradianceTextureSize.Width, Const.IrradianceTextureSize.Height);
            Transmittance = new RenderTexture(TrnDesc);
            Scattering = new RenderTexture(SctDesc);
            Irradiance = new RenderTexture(IrrDesc);
        }

        ~Model() {
        }

        public void Init(uint numScatteringOrders = 4) {
            if (ComputeTransmittance == null) {
                // ??= does not work here because of bad bad UnityEngine.Object
                ComputeTransmittance = new Material(Conf.ComputeTransmittance);
                ComputeDirectIrradiance = new Material(Conf.ComputeDirectIrradiance);
                ComputeSingleScattering = new Material(Conf.ComputeSingleScattering);
                ComputeScatteringDensity = new Material(Conf.ComputeScatteringDensity);
                ComputeIndirectIrradiance = new Material(Conf.ComputeIndirectIrradiance);
                ComputeMultipleScattering = new Material(Conf.ComputeMultipleScattering);
            }
            // The pre-computations require temporary textures, in particular to store the
            // contribution of one scattering order, which is needed to compute the next
            // order of scattering (the final precomputed textures store the sum of all
            // the scattering orders). We allocate them here, and destroy them at the end
            // of this method.
            RenderTexture deltaIrradiance = RenderTexture.GetTemporary(IrrDesc);
            RenderTexture deltaRayleighScattering = RenderTexture.GetTemporary(SctDesc);
            RenderTexture deltaMieScattering = RenderTexture.GetTemporary(SctDesc);
            RenderTexture deltaScatteringDensity = RenderTexture.GetTemporary(SctDesc);
            // delta_multiple_scattering_texture is only needed to compute scattering
            // order 3 or more, while delta_rayleigh_scattering_texture and
            // delta_mie_scattering_texture are only needed to compute double scattering.
            // Therefore, to save memory, we can store delta_rayleigh_scattering_texture
            // and delta_multiple_scattering_texture in the same GPU texture.
            RenderTexture deltaMultipleScattering = deltaRayleighScattering;

            PreCompute(deltaIrradiance, deltaRayleighScattering,
                deltaMieScattering, deltaScatteringDensity, deltaMultipleScattering,
                Matrix4x4.identity, numScatteringOrders);

            RenderTexture.ReleaseTemporary(deltaIrradiance);
            RenderTexture.ReleaseTemporary(deltaRayleighScattering);
            RenderTexture.ReleaseTemporary(deltaMieScattering);
            RenderTexture.ReleaseTemporary(deltaScatteringDensity);
        }

        void PreCompute(
            RenderTexture deltaIrradiance,
            RenderTexture deltaRayleighScattering,
            RenderTexture deltaMieScattering,
            RenderTexture deltaScatteringDensity,
            RenderTexture deltaMultipleScattering,
            Matrix4x4 luminanceFromRadiance,
            uint numScatteringOrders
        ) {
            Debug.Log("pre-compute model");
            // Compute the transmittance, and store it in transmittance_texture_.
            Util.DrawRect(ComputeTransmittance, Transmittance);

            // Compute the direct irradiance, store it in delta_irradiance_texture.
            // (we don't want the direct irradiance in irradiance_texture_,
            // but only the irradiance from the sky).
            ComputeDirectIrradiance.SetTexture("transmittance_texture", Transmittance);
            Util.DrawRect(ComputeDirectIrradiance, deltaIrradiance, Irradiance);

            // Compute the rayleigh and mie single scattering, store them in
            // delta_rayleigh_scattering_texture and delta_mie_scattering_texture, and
            // either store them or accumulate them in scattering_texture_ and
            // optional_single_mie_scattering_texture_.
            ComputeSingleScattering.SetMatrix("luminance_from_radiance", luminanceFromRadiance);
            ComputeSingleScattering.SetTexture("transmittance_texture", Transmittance);
            Util.DrawCube(ComputeSingleScattering,
                deltaRayleighScattering, deltaMieScattering, Scattering);
            // Compute the 2nd, 3rd and 4th order of scattering, in sequence.
            for (uint order = 2; order <= numScatteringOrders; order++) {
                // Compute the scattering density, and store it in
                // delta_scattering_density_texture.
                ComputeScatteringDensity.SetTexture("transmittance_texture", Transmittance);
                ComputeScatteringDensity.SetTexture("single_rayleigh_scattering_texture", deltaRayleighScattering);
                ComputeScatteringDensity.SetTexture("single_mie_scattering_texture", deltaMieScattering);
                ComputeScatteringDensity.SetTexture("multiple_scattering_texture", deltaMultipleScattering);
                ComputeScatteringDensity.SetTexture("irradiance_texture", deltaIrradiance);
                ComputeScatteringDensity.SetInteger("scattering_order", (int)order);
                Util.DrawCube(ComputeScatteringDensity, deltaScatteringDensity);

                // Compute the indirect irradiance, store it in delta_irradiance_texture and
                // accumulate it in irradiance_texture_.
                ComputeIndirectIrradiance.SetMatrix("luminance_from_radiance", luminanceFromRadiance);
                ComputeIndirectIrradiance.SetTexture("single_rayleigh_scattering_texture", deltaRayleighScattering);
                ComputeIndirectIrradiance.SetTexture("single_mie_scattering_texture", deltaMieScattering);
                ComputeIndirectIrradiance.SetTexture("multiple_scattering_texture", deltaMultipleScattering);
                ComputeIndirectIrradiance.SetInteger("scattering_order", (int)order - 1);
                deltaIrradiance.DiscardContents();
                Util.DrawRect(ComputeIndirectIrradiance, deltaIrradiance, Irradiance);

                // Compute the multiple scattering, store it in
                // delta_multiple_scattering_texture, and accumulate it in
                // scattering_texture_.
                ComputeMultipleScattering.SetMatrix("luminance_from_radiance", luminanceFromRadiance);
                ComputeMultipleScattering.SetTexture("transmittance_texture", Transmittance);
                ComputeMultipleScattering.SetTexture("scattering_density_texture", deltaScatteringDensity);
                deltaMultipleScattering.DiscardContents();
                Util.DrawCube(ComputeMultipleScattering, deltaMultipleScattering, Scattering);
            }
        }

        static Vector3 CIEColorMatchingFunctionTableValue(double wavelength) {
            Func<int, int, double> color = (int row, int column) =>
                Const.CIE2DegColorMatchingFunctions[4 * row + column];

            if (wavelength < Const.LambdaMin || wavelength > Const.LambdaMax) {
                return Vector3.zero;
            }
            double u = (wavelength - Const.LambdaMin) / Const.CIEFuncDeltaLambda;
            int row = (int)Math.Floor(u);
            Assert.IsTrue(row >= 0 && row + 1 < 95);
            Assert.IsTrue(Const.CIE2DegColorMatchingFunctions[4 * row] <= wavelength &&
                   Const.CIE2DegColorMatchingFunctions[4 * (row + 1)] >= wavelength);
            u -= row;
            var x = color(row, 1) * (1.0 - u) + color(row + 1, 1) * u;
            var y = color(row, 2) * (1.0 - u) + color(row + 1, 2) * u;
            var z = color(row, 3) * (1.0 - u) + color(row + 1, 3) * u;
            return new Vector3((float)x, (float)y, (float)z);
        }

        static double Interpolate(
            in List<double> wavelengths,
            in List<double> wavelengthFunction,
            double wavelength) {
            Assert.IsTrue(wavelengthFunction.Count == wavelengths.Count);
            if (wavelength < wavelengths[0]) {
                return wavelengthFunction[0];
            }
            for (int i = 0; i < wavelengths.Count - 1; i++) {
                if (wavelength < wavelengths[i + 1]) {
                    double u = (wavelength - wavelengths[i]) / (wavelengths[i + 1] - wavelengths[i]);
                    return wavelengthFunction[i] * (1.0 - u) + wavelengthFunction[i + 1] * u;
                }
            }
            return wavelengthFunction[wavelengthFunction.Count - 1];
        }

        // The returned constants are in lumen.nm / watt.
        static Vector3 ComputeSpectralRadianceToLuminanceFactors(
            List<double> wavelengths,
            List<double> solarIrradiance,
            double lambdaPower) {
            Vector3 kRGB = Vector3.zero;
            double solarR = Interpolate(wavelengths, solarIrradiance, Const.LambdaR);
            double solarG = Interpolate(wavelengths, solarIrradiance, Const.LambdaG);
            double solarB = Interpolate(wavelengths, solarIrradiance, Const.LambdaB);
            int dLambda = 1;
            for (int lambda = Const.LambdaMin; lambda < Const.LambdaMax; lambda += dLambda) {
                var xyzBar = CIEColorMatchingFunctionTableValue(lambda);
                var rBar = Vector3.Dot(Const.XYZToSRGB[0], xyzBar);
                var gBar = Vector3.Dot(Const.XYZToSRGB[1], xyzBar);
                var bBar = Vector3.Dot(Const.XYZToSRGB[2], xyzBar);
                double irradiance = Interpolate(wavelengths, solarIrradiance, lambda);
                kRGB.x += (float)(rBar * irradiance / solarR * Math.Pow(lambda / Const.LambdaR, lambdaPower));
                kRGB.y += (float)(gBar * irradiance / solarG * Math.Pow(lambda / Const.LambdaG, lambdaPower));
                kRGB.z += (float)(bBar * irradiance / solarB * Math.Pow(lambda / Const.LambdaB, lambdaPower));
            }
            kRGB *= (float)Const.MaxLuminousEfficacy * dLambda;
            return kRGB;
        }

        public static string Header(ModelParams para, Vector3 lambdas) {
            Func<List<double>, double, string> toString = (List<double> spectrum, double scale) => {
                double r = Model.Interpolate(para.Wavelengths, spectrum, lambdas.x) * scale;
                double g = Model.Interpolate(para.Wavelengths, spectrum, lambdas.y) * scale;
                double b = Model.Interpolate(para.Wavelengths, spectrum, lambdas.z) * scale;
                return $"float3({r:g},{g:g},{b:g})";
            };
            Func<DensityProfileLayer, string> densityLayer = (DensityProfileLayer layer) => {
                return $@"_DensityProfileLayer({layer.Width / para.LengthUnitInMeters},{layer.ExpTerm},{layer.ExpScale * para.LengthUnitInMeters},{layer.LinearTerm * para.LengthUnitInMeters},{layer.ConstantTerm})";
            };
            Func<List<DensityProfileLayer>, string> densityProfile = (List<DensityProfileLayer> layers) => {
                const int layerCount = 2;
                while (layers.Count < layerCount) {
                    layers.Insert(0, new DensityProfileLayer());
                }

                var nl = Environment.NewLine;
                string result = $"_DensityProfile({nl}        ";
                for (int i = 0; i < layerCount; i++) {
                    result += densityLayer(layers[i]);
                    result += i < layerCount - 1 ? $",{nl}        " : ")";
                }
                return result;
            };

            var skyRGB = Model.ComputeSpectralRadianceToLuminanceFactors(para.Wavelengths, para.SolarIrradiance, -3);
            var sunRGB = Model.ComputeSpectralRadianceToLuminanceFactors(para.Wavelengths, para.SolarIrradiance, 0);
            // $"float3({r},{g},{b})"

            string header = $@"
#define IN(x) const in x
#define OUT(x) out x
#define TEMPLATE(x)
#define TEMPLATE_ARGUMENT(x)
#define assert(x)

static const int TRANSMITTANCE_TEXTURE_WIDTH = {Const.TransmittanceTextureSize.Width};
static const int TRANSMITTANCE_TEXTURE_HEIGHT = {Const.TransmittanceTextureSize.Height};
static const int SCATTERING_TEXTURE_R_SIZE = {Const.ScatteringTextureSize.R};
static const int SCATTERING_TEXTURE_MU_SIZE = {Const.ScatteringTextureSize.Mu};
static const int SCATTERING_TEXTURE_MU_S_SIZE = {Const.ScatteringTextureSize.MuS};
static const int SCATTERING_TEXTURE_NU_SIZE = {Const.ScatteringTextureSize.Nu};
static const int IRRADIANCE_TEXTURE_WIDTH = {Const.IrradianceTextureSize.Width};
static const int IRRADIANCE_TEXTURE_HEIGHT = {Const.IrradianceTextureSize.Height};

static const int2 TRANSMITTANCE_TEXTURE_SIZE = int2(TRANSMITTANCE_TEXTURE_WIDTH, TRANSMITTANCE_TEXTURE_HEIGHT);
static const int3 SCATTERING_TEXTURE_SIZE = int3(
    SCATTERING_TEXTURE_NU_SIZE * SCATTERING_TEXTURE_MU_S_SIZE,
    SCATTERING_TEXTURE_MU_SIZE,
    SCATTERING_TEXTURE_R_SIZE);
static const int2 IRRADIANCE_TEXTURE_SIZE = int2(IRRADIANCE_TEXTURE_WIDTH, IRRADIANCE_TEXTURE_HEIGHT);

AtmosphereParameters _ATMOSPHERE()
{{
    AtmosphereParameters a;
    a.solar_irradiance = {toString(para.SolarIrradiance, 1.0)};
    a.sun_angular_radius = {para.SunAngularRadius};
    a.bottom_radius = {para.BottomRadius / para.LengthUnitInMeters};
    a.top_radius = {para.TopRadius / para.LengthUnitInMeters};
    a.rayleigh_density = {densityProfile(para.RayleighDensity)};
    a.rayleigh_scattering = {toString(para.RayleighScattering, para.LengthUnitInMeters)};
    a.mie_density = {densityProfile(para.MieDensity)};
    a.mie_scattering = {toString(para.MieScattering, para.LengthUnitInMeters)};
    a.mie_extinction = {toString(para.MieExtinction, para.LengthUnitInMeters)};
    a.mie_phase_function_g = {para.MiePhaseFunctionG};
    a.absorption_density = {densityProfile(para.AbsorptionDensity)};
    a.absorption_extinction = {toString(para.AbsorptionExtinction, para.LengthUnitInMeters)};
    a.ground_albedo = {toString(para.GroundAlbedo, 1.0)};
    a.mu_s_min = {Math.Cos(para.MaxSunZenithAngle)};
    return a;
}}

static const AtmosphereParameters ATMOSPHERE = _ATMOSPHERE();
static const float3 SKY_SPECTRAL_RADIANCE_TO_LUMINANCE = float3({skyRGB.x},{skyRGB.y},{skyRGB.z});
static const float3 SUN_SPECTRAL_RADIANCE_TO_LUMINANCE = float3({sunRGB.x},{sunRGB.y},{sunRGB.z});
";
            return header;
        }
    }
}