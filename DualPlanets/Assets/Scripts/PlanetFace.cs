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
        normal = _localUp.normalized;

        tangent = new Vector3(normal.y, normal.z, normal.x).normalized;
        bitangent = Vector3.Cross(normal, tangent).normalized;

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

                Vector3 pointOnSphere;

                if (percent.x == 0.5 && percent.y == 0.5)
                {
                    pointOnSphere = normal;
                }
                else if (percent.y == 0.5f)
                {
                    pointOnSphere = Quaternion.AngleAxis((percent.x - 0.5f) * 90.0f, tangent) * normal;
                }
                else if (percent.x == 0.5f)
                {
                    pointOnSphere = Quaternion.AngleAxis((percent.y - 0.5f) * 90.0f, bitangent) * normal;
                }
                else
                {

                    Vector3 rotOnTan = Quaternion.AngleAxis((percent.x - 0.5f) * 90.0f, tangent) * normal;
                    Vector3 planeANorm = Vector3.Cross(rotOnTan, tangent).normalized;
                    Vector3 rotOnBitan = Quaternion.AngleAxis((percent.y - 0.5f) * 90.0f, bitangent) * normal;
                    Vector3 planeBNorm = Vector3.Cross(rotOnBitan, bitangent).normalized;

                    bool swapped = false;
                    if (planeANorm.x == 0)
                    {
                        if (planeBNorm.x == 0)
                        {
                            Debug.LogError("No x is non-zero.");
                            return;
                        }
                        else
                        {
                            Vector3 temp = planeBNorm;
                            planeBNorm = planeANorm;
                            planeANorm = temp;
                            swapped = true;
                        }
                    }
                    float hDenom = planeBNorm.y * planeANorm.x - planeBNorm.x * planeANorm.y;
                    if (hDenom == 0)
                    {
                        Debug.LogError("H denominator borked.");
                        return;
                    }

                    float hValue = (planeANorm.z * planeBNorm.x - planeANorm.x * planeBNorm.z) / hDenom;
                    float gValue = ((-planeANorm.y) * hValue - planeANorm.z) / planeANorm.x;

                    float kDenom = gValue * gValue + hValue * hValue + 1;
                    if (kDenom == 0)
                    {
                        Debug.LogError("Denominator of k has encountered a problem.");
                        return;
                    }
                    float kValue = Mathf.Sqrt(1.0f / kDenom);

                    Vector3 test1 = new Vector3(gValue * kValue, hValue * kValue, kValue);
                    if (Vector3.Dot(test1, normal) > 0)
                    {
                        pointOnSphere = test1;
                    }
                    else
                    {
                        pointOnSphere = new Vector3(-gValue * kValue, -hValue * kValue, -kValue);
                    }

                    pointOnSphere = pointOnSphere.normalized;
                }

                // Vector3 pointOnCube = normal + (percent.x - 0.5f) * 2.0f * tangent + (percent.y - 0.5f) * 2.0f * bitangent;

                // Vector3 pointOnSphere = pointOnCube.normalized;
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
