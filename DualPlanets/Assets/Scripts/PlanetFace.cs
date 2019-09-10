using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlanetFace
{

    Mesh mesh;
    int resolution;
    Vector3 normal;
    Vector3 tangent;
    Vector3 bitangent;

    public PlanetFace(Mesh _mesh, int _resolution, Vector3 _localUp)
    {
        mesh = _mesh;
        resolution = _resolution;
        normal = _localUp;

        tangent = new Vector3(normal.y, normal.z, normal.x);
        bitangent = Vector3.Cross(normal, tangent);

    }

    public void ConstructMesh()
    {
        int resMin1 = resolution - 1;

        Vector3[] vertices = new Vector3[resolution * resolution];
        int[] tris = new int[resMin1 * resMin1 * 6];
        Vector3[] norms = new Vector3[resolution * resolution];

        int vCount = 0;

        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                Vector2 percent = new Vector2(x, y) / resMin1;
                Vector3 pointOnCube = normal + (percent.x - 0.5f) * 2.0f * tangent + (percent.y - 0.5f) * 2.0f * bitangent;
                Vector3 pointOnSphere = pointOnCube.normalized;
                vertices[vCount] = pointOnSphere;
                norms[vCount] = pointOnSphere;

                if (!(y == resolution - 1 || x == resolution - 1))
                {
                    int triCount = (vCount - y) * 6;
                    tris[triCount] = vCount;
                    tris[triCount + 1] = vCount + resolution + 1;
                    tris[triCount + 2] = vCount + resolution;

                    tris[triCount + 3] = vCount;
                    tris[triCount + 4] = vCount + 1;
                    tris[triCount + 5] = vCount + resolution + 1;
                }

                vCount++;
            }
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = tris;
        mesh.normals = norms;
    }
}
