#include "functions.hlsl"

sampler2D transmittance_texture;
sampler3D scattering_texture;
sampler3D single_mie_scattering_texture;
sampler2D irradiance_texture;

RadianceSpectrum GetSolarRadiance()
{
    return ATMOSPHERE.solar_irradiance / (PI * ATMOSPHERE.sun_angular_radius * ATMOSPHERE.sun_angular_radius);
}
RadianceSpectrum GetSkyRadiance(
    Position camera, Direction view_ray, Length shadow_length,
    Direction sun_direction, out DimensionlessSpectrum transmittance)
{
    return GetSkyRadiance(ATMOSPHERE, transmittance_texture,
        scattering_texture, single_mie_scattering_texture,
        camera, view_ray, shadow_length, sun_direction, transmittance);
}
RadianceSpectrum GetSkyRadianceToPoint(
    Position camera, Position position, Length shadow_length,
    Direction sun_direction, out DimensionlessSpectrum transmittance)
{
    return GetSkyRadianceToPoint(ATMOSPHERE, transmittance_texture,
        scattering_texture, single_mie_scattering_texture,
        camera, position, shadow_length, sun_direction, transmittance);
}
IrradianceSpectrum GetSunAndSkyIrradiance(
    Position p, Direction normal, Direction sun_direction,
    out IrradianceSpectrum sky_irradiance)
{
    return GetSunAndSkyIrradiance(ATMOSPHERE, transmittance_texture,
        irradiance_texture, p, normal, sun_direction, sky_irradiance);
}

Luminance3 GetSolarLuminance()
{
    return ATMOSPHERE.solar_irradiance / (PI * ATMOSPHERE.sun_angular_radius * ATMOSPHERE.sun_angular_radius) * SUN_SPECTRAL_RADIANCE_TO_LUMINANCE;
}

Luminance3 GetSkyLuminance(
    Position camera, Direction view_ray, Length shadow_length,
    Direction sun_direction, out DimensionlessSpectrum transmittance)
{
    return GetSkyRadiance(ATMOSPHERE, transmittance_texture,
               scattering_texture, single_mie_scattering_texture,
               camera, view_ray, shadow_length, sun_direction, transmittance)
        * SKY_SPECTRAL_RADIANCE_TO_LUMINANCE;
}

Luminance3 GetSkyLuminanceToPoint(
    Position camera, Position position, Length shadow_length,
    Direction sun_direction, out DimensionlessSpectrum transmittance)
{
    return GetSkyRadianceToPoint(ATMOSPHERE, transmittance_texture,
               scattering_texture, single_mie_scattering_texture,
               camera, position, shadow_length, sun_direction, transmittance)
        * SKY_SPECTRAL_RADIANCE_TO_LUMINANCE;
}

Illuminance3 GetSunAndSkyIlluminance(
    Position p, Direction normal, Direction sun_direction,
    out IrradianceSpectrum sky_irradiance)
{
    IrradianceSpectrum sun_irradiance = GetSunAndSkyIrradiance(
        ATMOSPHERE, transmittance_texture, irradiance_texture, p, normal,
        sun_direction, sky_irradiance);
    sky_irradiance *= SKY_SPECTRAL_RADIANCE_TO_LUMINANCE;
    return sun_irradiance * SUN_SPECTRAL_RADIANCE_TO_LUMINANCE;
}
