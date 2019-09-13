using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetShapeGenerator
{
    PlanetShape shapeSettings;
    NoiseFilter[] noiseFilters;

    public PlanetShapeGenerator(PlanetShape settings)
    {
        shapeSettings = settings;
        noiseFilters = new NoiseFilter[settings.noiseLayers.Length];
        for(int i = 0; i < noiseFilters.Length; ++i)
        {
            noiseFilters[i] = new NoiseFilter(settings.noiseLayers[i].noiseSettings);
        }
    }

    public Vector3 CalculatePointOnPlanet(Vector3 pointOnSphere)
    {
        float firstLayerValue = 0;
        float elevation = 0;

        if(noiseFilters.Length > 0)
        {
            firstLayerValue = noiseFilters[0].Evaluate(pointOnSphere);
            if (shapeSettings.noiseLayers[0].enabled)
            {
                elevation = firstLayerValue;
            }
        }


        for (int i = 1; i < noiseFilters.Length; ++i)
        {
            if (shapeSettings.noiseLayers[i].enabled)
            {
                float mask = (shapeSettings.noiseLayers[i].useFirstLayerAsMask) ? firstLayerValue : 1.0f;
                elevation += noiseFilters[i].Evaluate(pointOnSphere) * mask;
            }
        }
        return pointOnSphere * shapeSettings.radius * (1 + elevation);
    }
}
