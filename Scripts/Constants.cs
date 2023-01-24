using System;
using System.Collections.Generic;
using UnityEngine;
namespace Aerosol {
    public static class Const {
        public static class TransmittanceTextureSize {
            public const int Width = 256;
            public const int Height = 64;
        }

        public static class ScatteringTextureSize {
            public const int R = 32;
            public const int Mu = 128;
            public const int MuS = 32;
            public const int Nu = 8;

            public const int Width = Nu * MuS;
            public const int Height = Mu;
            public const int Depth = R;
        }

        public static class IrradianceTextureSize {
            public const int Width = 64;
            public const int Height = 16;
        }

        // The conversion factor between watts and lumens.
        // Ideal monochromatic source.
        public const double MaxLuminousEfficacy = 683.0;

        public static int CIEFuncDeltaLambda = 5;
        public static readonly double[] CIE2DegColorMatchingFunctions = {
            360, 0.000129900000, 0.000003917000, 0.000606100000,
            365, 0.000232100000, 0.000006965000, 0.001086000000,
            370, 0.000414900000, 0.000012390000, 0.001946000000,
            375, 0.000741600000, 0.000022020000, 0.003486000000,
            380, 0.001368000000, 0.000039000000, 0.006450001000,
            385, 0.002236000000, 0.000064000000, 0.010549990000,
            390, 0.004243000000, 0.000120000000, 0.020050010000,
            395, 0.007650000000, 0.000217000000, 0.036210000000,
            400, 0.014310000000, 0.000396000000, 0.067850010000,
            405, 0.023190000000, 0.000640000000, 0.110200000000,
            410, 0.043510000000, 0.001210000000, 0.207400000000,
            415, 0.077630000000, 0.002180000000, 0.371300000000,
            420, 0.134380000000, 0.004000000000, 0.645600000000,
            425, 0.214770000000, 0.007300000000, 1.039050100000,
            430, 0.283900000000, 0.011600000000, 1.385600000000,
            435, 0.328500000000, 0.016840000000, 1.622960000000,
            440, 0.348280000000, 0.023000000000, 1.747060000000,
            445, 0.348060000000, 0.029800000000, 1.782600000000,
            450, 0.336200000000, 0.038000000000, 1.772110000000,
            455, 0.318700000000, 0.048000000000, 1.744100000000,
            460, 0.290800000000, 0.060000000000, 1.669200000000,
            465, 0.251100000000, 0.073900000000, 1.528100000000,
            470, 0.195360000000, 0.090980000000, 1.287640000000,
            475, 0.142100000000, 0.112600000000, 1.041900000000,
            480, 0.095640000000, 0.139020000000, 0.812950100000,
            485, 0.057950010000, 0.169300000000, 0.616200000000,
            490, 0.032010000000, 0.208020000000, 0.465180000000,
            495, 0.014700000000, 0.258600000000, 0.353300000000,
            500, 0.004900000000, 0.323000000000, 0.272000000000,
            505, 0.002400000000, 0.407300000000, 0.212300000000,
            510, 0.009300000000, 0.503000000000, 0.158200000000,
            515, 0.029100000000, 0.608200000000, 0.111700000000,
            520, 0.063270000000, 0.710000000000, 0.078249990000,
            525, 0.109600000000, 0.793200000000, 0.057250010000,
            530, 0.165500000000, 0.862000000000, 0.042160000000,
            535, 0.225749900000, 0.914850100000, 0.029840000000,
            540, 0.290400000000, 0.954000000000, 0.020300000000,
            545, 0.359700000000, 0.980300000000, 0.013400000000,
            550, 0.433449900000, 0.994950100000, 0.008749999000,
            555, 0.512050100000, 1.000000000000, 0.005749999000,
            560, 0.594500000000, 0.995000000000, 0.003900000000,
            565, 0.678400000000, 0.978600000000, 0.002749999000,
            570, 0.762100000000, 0.952000000000, 0.002100000000,
            575, 0.842500000000, 0.915400000000, 0.001800000000,
            580, 0.916300000000, 0.870000000000, 0.001650001000,
            585, 0.978600000000, 0.816300000000, 0.001400000000,
            590, 1.026300000000, 0.757000000000, 0.001100000000,
            595, 1.056700000000, 0.694900000000, 0.001000000000,
            600, 1.062200000000, 0.631000000000, 0.000800000000,
            605, 1.045600000000, 0.566800000000, 0.000600000000,
            610, 1.002600000000, 0.503000000000, 0.000340000000,
            615, 0.938400000000, 0.441200000000, 0.000240000000,
            620, 0.854449900000, 0.381000000000, 0.000190000000,
            625, 0.751400000000, 0.321000000000, 0.000100000000,
            630, 0.642400000000, 0.265000000000, 0.000049999990,
            635, 0.541900000000, 0.217000000000, 0.000030000000,
            640, 0.447900000000, 0.175000000000, 0.000020000000,
            645, 0.360800000000, 0.138200000000, 0.000010000000,
            650, 0.283500000000, 0.107000000000, 0.000000000000,
            655, 0.218700000000, 0.081600000000, 0.000000000000,
            660, 0.164900000000, 0.061000000000, 0.000000000000,
            665, 0.121200000000, 0.044580000000, 0.000000000000,
            670, 0.087400000000, 0.032000000000, 0.000000000000,
            675, 0.063600000000, 0.023200000000, 0.000000000000,
            680, 0.046770000000, 0.017000000000, 0.000000000000,
            685, 0.032900000000, 0.011920000000, 0.000000000000,
            690, 0.022700000000, 0.008210000000, 0.000000000000,
            695, 0.015840000000, 0.005723000000, 0.000000000000,
            700, 0.011359160000, 0.004102000000, 0.000000000000,
            705, 0.008110916000, 0.002929000000, 0.000000000000,
            710, 0.005790346000, 0.002091000000, 0.000000000000,
            715, 0.004109457000, 0.001484000000, 0.000000000000,
            720, 0.002899327000, 0.001047000000, 0.000000000000,
            725, 0.002049190000, 0.000740000000, 0.000000000000,
            730, 0.001439971000, 0.000520000000, 0.000000000000,
            735, 0.000999949300, 0.000361100000, 0.000000000000,
            740, 0.000690078600, 0.000249200000, 0.000000000000,
            745, 0.000476021300, 0.000171900000, 0.000000000000,
            750, 0.000332301100, 0.000120000000, 0.000000000000,
            755, 0.000234826100, 0.000084800000, 0.000000000000,
            760, 0.000166150500, 0.000060000000, 0.000000000000,
            765, 0.000117413000, 0.000042400000, 0.000000000000,
            770, 0.000083075270, 0.000030000000, 0.000000000000,
            775, 0.000058706520, 0.000021200000, 0.000000000000,
            780, 0.000041509940, 0.000014990000, 0.000000000000,
            785, 0.000029353260, 0.000010600000, 0.000000000000,
            790, 0.000020673830, 0.000007465700, 0.000000000000,
            795, 0.000014559770, 0.000005257800, 0.000000000000,
            800, 0.000010253980, 0.000003702900, 0.000000000000,
            805, 0.000007221456, 0.000002607800, 0.000000000000,
            810, 0.000005085868, 0.000001836600, 0.000000000000,
            815, 0.000003581652, 0.000001293400, 0.000000000000,
            820, 0.000002522525, 0.000000910930, 0.000000000000,
            825, 0.000001776509, 0.000000641530, 0.000000000000,
            830, 0.000001251141, 0.000000451810, 0.000000000000,
        };

        public static readonly Vector3[] XYZToSRGB = {
            new Vector3(+3.2406f, -1.5372f, -0.4986f),
            new Vector3(-0.9689f, +1.8758f, +0.0415f),
            new Vector3(+0.0557f, -0.2040f, +1.0570f)
        };

        public const double LambdaR = 680.0;
        public const double LambdaG = 550.0;
        public const double LambdaB = 440.0;
        public static readonly Vector3 Lambdas = new Vector3(
            (float)LambdaR, (float)LambdaG, (float)LambdaB);

        public const int LambdaMin = 360;
        public const int LambdaMax = 830;
        public const int DeltaLambda = 10;

        public const double SunAngularRadius = 0.00935 / 2.0;
        public const double SunSolidAngle = Math.PI * SunAngularRadius * SunAngularRadius;
        public const double LengthUnitInMeters = 1000;
        // Values from "Reference Solar Spectral Irradiance: ASTM G-173", ETR column
        // (see http://rredc.nrel.gov/solar/spectra/am1.5/ASTMG173/ASTMG173.html),
        // summed and averaged in each bin (e.g. the value for 360nm is the average
        // of the ASTM G-173 values for all wavelengths between 360 and 370nm).
        // Values in W.m^-2.
        public static readonly double[] SolarIrradiance = {
            1.11776, 1.14259, 1.01249, 1.14716, 1.72765, 1.73054, 1.6887, 1.61253,
            1.91198, 2.03474, 2.02042, 2.02212, 1.93377, 1.95809, 1.91686, 1.8298,
            1.8685, 1.8931, 1.85149, 1.8504, 1.8341, 1.8345, 1.8147, 1.78158, 1.7533,
            1.6965, 1.68194, 1.64654, 1.6048, 1.52143, 1.55622, 1.5113, 1.474, 1.4482,
            1.41018, 1.36775, 1.34188, 1.31429, 1.28303, 1.26758, 1.2367, 1.2082,
            1.18737, 1.14683, 1.12362, 1.1058, 1.07124, 1.04992
        };
        // Values from http://www.iup.uni-bremen.de/gruppen/molspec/databases/
        // referencespectra/o3spectra2011/index.html for 233K, summed and averaged in
        // each bin (e.g. the value for 360nm is the average of the original values
        // for all wavelengths between 360 and 370nm). Values in m^2.
        public static readonly double[] OzoneCrossSection = {
            1.18e-27, 2.182e-28, 2.818e-28, 6.636e-28, 1.527e-27, 2.763e-27, 5.52e-27,
            8.451e-27, 1.582e-26, 2.316e-26, 3.669e-26, 4.924e-26, 7.752e-26, 9.016e-26,
            1.48e-25, 1.602e-25, 2.139e-25, 2.755e-25, 3.091e-25, 3.5e-25, 4.266e-25,
            4.672e-25, 4.398e-25, 4.701e-25, 5.019e-25, 4.305e-25, 3.74e-25, 3.215e-25,
            2.662e-25, 2.238e-25, 1.852e-25, 1.473e-25, 1.209e-25, 9.423e-26, 7.455e-26,
            6.566e-26, 5.105e-26, 4.15e-26, 4.228e-26, 3.237e-26, 2.451e-26, 2.801e-26,
            2.534e-26, 1.624e-26, 1.465e-26, 2.078e-26, 1.383e-26, 7.105e-27
        };
        // From https://en.wikipedia.org/wiki/Dobson_unit, in molecules.m^-2.
        public const double DobsonUnit = 2.687e20;
        // Maximum number density of ozone molecules, in m^-3 (computed so at to get
        // 300 Dobson units of ozone - for this we divide 300 DU by the integral of
        // the ozone density profile defined below, which is equal to 15km).
        public const double MaxOzoneNumberDensity = 300.0 * DobsonUnit / 15000.0;
        // Wavelength independent solar irradiance "spectrum" (not physically
        // realistic, but was used in the original implementation).
        public const double ConstantSolarIrradiance = 1.5;
        public const double BottomRadius = 6360000.0;
        public const double TopRadius = 6420000.0;
        public const double Rayleigh = 1.24062e-6;
        public const double RayleighScaleHeight = 8000.0;
        public const double MieScaleHeight = 1200.0;
        public const double MieAngstromAlpha = 0.0;
        public const double MieAngstromBeta = 5.328e-3;
        public const double MieSingleScatteringAlbedo = 0.9;
        public const double MiePhaseFunctionG = 0.8;
        public const double GroundAlbedo = 0.1;
        public static readonly Func<bool, double> MaxSunZenithAngle =
            (bool half_precision) => (half_precision ? 102.0 : 120.0) / 180.0 * Math.PI;
        public static readonly DensityProfileLayer RayleighLayer = new DensityProfileLayer {
            ExpTerm = 1.0,
            ExpScale = -1.0 / RayleighScaleHeight,
        };
        public static readonly DensityProfileLayer MieLayer = new DensityProfileLayer {
            ExpTerm = 1.0,
            ExpScale = -1.0 / MieScaleHeight,
        };
        public static readonly List<DensityProfileLayer> OzoneDensity = new List<DensityProfileLayer>{
            new DensityProfileLayer{
                Width = 25000.0,
                LinearTerm = 1.0 / 15000.0,
                ConstantTerm = -2.0 / 3.0,
            },
            new DensityProfileLayer{
                LinearTerm = -1.0 / 15000.0,
                ConstantTerm = 8.0 / 3.0,
            }
        };

        public static ModelParams DefaultParam() {
            int nLambda = (LambdaMax - LambdaMin) / DeltaLambda + 1;
            var wavelengths = new List<double>(nLambda); // [LambdaMin, LambdaMax], step=DeltaLambda
            var solarIrradiance = new List<double>(nLambda);
            var rayleighScattering = new List<double>(nLambda);
            var mieScattering = new List<double>(nLambda);
            var mieExtinction = new List<double>(nLambda);
            var absorptionExtinction = new List<double>(nLambda);
            var groundAlbedo = new List<double>(nLambda);

            for (int l = LambdaMin; l <= LambdaMax; l += DeltaLambda) {
                int index = (l - LambdaMin) / DeltaLambda;
                double lambda = (double)l * 1e-3; // micro-meters, nm->um
                double mie = MieAngstromBeta / MieScaleHeight * Math.Pow(lambda, -MieAngstromAlpha);
                wavelengths.Add(l);
                solarIrradiance.Add(SolarIrradiance[index]);
                rayleighScattering.Add(Rayleigh * Math.Pow(lambda, -4));
                mieScattering.Add(mie * MieSingleScatteringAlbedo);
                mieExtinction.Add(mie);
                absorptionExtinction.Add(MaxOzoneNumberDensity * OzoneCrossSection[index]);
                groundAlbedo.Add(GroundAlbedo);
            }

            return new ModelParams {
                Wavelengths = wavelengths,
                SolarIrradiance = solarIrradiance,
                SunAngularRadius = SunAngularRadius,
                BottomRadius = BottomRadius,
                TopRadius = TopRadius,
                RayleighDensity = new List<DensityProfileLayer> { RayleighLayer },
                RayleighScattering = rayleighScattering,
                MieDensity = new List<DensityProfileLayer> { MieLayer },
                MieScattering = mieScattering,
                MieExtinction = mieExtinction,
                MiePhaseFunctionG = MiePhaseFunctionG,
                AbsorptionDensity = OzoneDensity,
                AbsorptionExtinction = absorptionExtinction,
                GroundAlbedo = groundAlbedo,
                MaxSunZenithAngle = MaxSunZenithAngle(true),
                LengthUnitInMeters = LengthUnitInMeters,
            };
        }
    }
}