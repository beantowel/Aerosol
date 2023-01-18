
#define IN(x) const in x
#define OUT(x) out x
#define TEMPLATE(x)
#define TEMPLATE_ARGUMENT(x)
#define assert(x)

static const int TRANSMITTANCE_TEXTURE_WIDTH = 256;
static const int TRANSMITTANCE_TEXTURE_HEIGHT = 64;
static const int SCATTERING_TEXTURE_R_SIZE = 32;
static const int SCATTERING_TEXTURE_MU_SIZE = 128;
static const int SCATTERING_TEXTURE_MU_S_SIZE = 32;
static const int SCATTERING_TEXTURE_NU_SIZE = 8;
static const int IRRADIANCE_TEXTURE_WIDTH = 64;
static const int IRRADIANCE_TEXTURE_HEIGHT = 16;

static const int2 TRANSMITTANCE_TEXTURE_SIZE = int2(TRANSMITTANCE_TEXTURE_WIDTH, TRANSMITTANCE_TEXTURE_HEIGHT);
static const int3 SCATTERING_TEXTURE_SIZE = int3(
    SCATTERING_TEXTURE_NU_SIZE * SCATTERING_TEXTURE_MU_S_SIZE,
    SCATTERING_TEXTURE_MU_SIZE,
    SCATTERING_TEXTURE_R_SIZE);
static const int2 IRRADIANCE_TEXTURE_SIZE = int2(IRRADIANCE_TEXTURE_WIDTH, IRRADIANCE_TEXTURE_HEIGHT);

AtmosphereParameters _ATMOSPHERE()
{
    AtmosphereParameters a;
    a.solar_irradiance = float3(1.474,1.8504,1.91198);
    a.sun_angular_radius = 0.004675;
    a.bottom_radius = 6360;
    a.top_radius = 6420;
    a.rayleigh_density = _DensityProfile(
        _DensityProfileLayer(0,0,0,0,0),
        _DensityProfileLayer(0,1,-0.125,0,0));
    a.rayleigh_scattering = float3(0.00580233938171238,0.0135577624479202,0.0331000059763677);
    a.mie_density = _DensityProfile(
        _DensityProfileLayer(0,0,0,0,0),
        _DensityProfileLayer(0,1,-0.833333333333333,0,0));
    a.mie_scattering = float3(0.003996,0.003996,0.003996);
    a.mie_extinction = float3(0.00444,0.00444,0.00444);
    a.mie_phase_function_g = 0.8;
    a.absorption_density = _DensityProfile(
        _DensityProfileLayer(25,0,0,0.0666666666666667,-0.666666666666667),
        _DensityProfileLayer(0,0,0,-0.0666666666666667,2.66666666666667));
    a.absorption_extinction = float3(0.0006497166,0.0018809,8.501668e-05);
    a.ground_albedo = float3(0.1,0.1,0.1);
    a.mu_s_min = -0.207911690817759;
    return a;
}

static const AtmosphereParameters ATMOSPHERE = _ATMOSPHERE();
static const float3 SKY_SPECTRAL_RADIANCE_TO_LUMINANCE = float3(114975.3,71305.86,65311.04);
static const float3 SUN_SPECTRAL_RADIANCE_TO_LUMINANCE = float3(98242.78,69954.39,66475.27);
