

#ifndef METABALL_INCLUDE
#define METABALL_INCLUDE
#define MAX_PARTICLES 256

float4 _ParticlesPos[MAX_PARTICLES];
float _ParticlesSize[MAX_PARTICLES];
float _NumParticles;

float GetDistanceSphere(float3 from, float3 center, float radius)
{
    return length(from - center) - radius;
}

float GetDistanceMetaball(float3 p)
{
    float sumDensity = 0;
    float sumRi = 0;
    float minDistance = 100000;
    for (int i = 0; i < _NumParticles; ++i)
    {
        float4 center = _ParticlesPos[i];
        float radius = 0.3 * _ParticlesSize[i];
        float r = length(center - p);
        if (r <= radius)
        {
            sumDensity += 2 * (r * r * r) / (radius * radius * radius) - 3 * (r * r) / (radius * radius) + 1;
        }
        minDistance = min(minDistance, r - radius);
        sumRi += radius;
    }

    return max(minDistance, (0.2 - sumDensity) / (3 / 2.0 * sumRi));
}

void SphereTraceMetaballs_float(float3 WorldPosition, out float Alpha)
{
#if defined(SHADERGRAPH_PREVIEW)
    Alpha = 0;
#else
    float maxDistance = 100;
    float threshold = 0.00001;
    float t = 0;
    int numSteps = 0;

    float outAlpha = 0;

    float3 viewPosition = GetCurrentViewPosition();
    half3 viewDir = SafeNormalize(WorldPosition - viewPosition);
    while (t < maxDistance)
    {
        float minDistance = 1000000;
        float3 from = viewPosition + t * viewDir;
        float d = GetDistanceMetaball(from);
        if (d < minDistance)
        {
            minDistance = d;
        }

        if (minDistance <= threshold * t)
        {
            outAlpha = 1;
            break;
        }

        t += minDistance;
        ++numSteps;
    }
    
    Alpha = outAlpha;
#endif
}
#endif
