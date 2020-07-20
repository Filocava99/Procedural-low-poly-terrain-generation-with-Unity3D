using System.Collections.Generic;
using UnityEngine;

public class Terrain : MonoBehaviour
{
    public GameObject prefab;

    FastNoise fastNoise = new FastNoise(532452345);

    public void Start()
    {
        int size = 16;
        for (int x = 0; x < size; x++)
        {
            for (int z = 0; z < size; z++)
            {
                Mesh mesh = GetTerrain(new Vector2Int(x, z));
                GameObject terrain = Instantiate(prefab, transform);
                terrain.name = "Terrain(" + x + "," + z + ")";
                terrain.GetComponent<MeshFilter>().mesh = mesh;
                terrain.GetComponent<MeshCollider>().sharedMesh = mesh;
                var terrainTransform = terrain.GetComponent<Transform>();
                Vector3 oldPos = terrainTransform.position;
                terrainTransform.position = new Vector3(x * 16, oldPos.y, z * 16);
            }
        }
    }

    public Mesh GetTerrain(Vector2Int position)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();
        int z, x;
        int size = 16;
        float scale = 0.09f;
        for (z = 0; z < size; z++)
        {
            for (x = 0; x < size; x++)
            {
                int heightV1 = (int) SumOcatave(16, x + (position.x * size), z + 1 + (position.y * size), 0.5f, scale,
                    0, 255);
                int heightV2 = (int) SumOcatave(16, x + (position.x * size), z + (position.y * size), 0.5f, scale, 0,
                    255);
                int heightV3 = (int) SumOcatave(16, x + 1 + (position.x * size), z + (position.y * size), 0.5f, scale,
                    0, 255);
                int heightV4 = (int) SumOcatave(16, x + 1 + (position.x * size), z + 1 + (position.y * size), 0.5f,
                    scale, 0, 255);
                vertices.Add(new Vector3(x, heightV1, z + 1));
                vertices.Add(new Vector3(x, heightV2, z));
                vertices.Add(new Vector3(x + 1, heightV3, z));
                vertices.Add(new Vector3(x + 1, heightV4, z + 1));
                uvs.Add(new Vector2(0.25f, 0.875f));
                uvs.Add(new Vector2(0.25f, 0.75f));
                uvs.Add(new Vector2(0.375f, 0.75f));
                uvs.Add(new Vector2(0.375f, 0.875f));
                int verticesIndex = vertices.Count - 4;
                triangles.Add(verticesIndex + 2);
                triangles.Add(verticesIndex + 1);
                triangles.Add(verticesIndex);
                triangles.Add(verticesIndex + 3);
                triangles.Add(verticesIndex + 2);
                triangles.Add(verticesIndex);
            }
        }
        var mesh = new Mesh {vertices = vertices.ToArray(), triangles = triangles.ToArray(), uv = uvs.ToArray()};
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        return mesh;
    }

    private double SumOcatave(int numIteration, int x, int y, float persistance, float scale, int low, int high)
    {
        float maxAmp = 0;
        float amp = 1;
        float freq = scale;
        float noise = 0;

        for (int i = 0; i < numIteration; ++i)
        {
            noise += fastNoise.GetSimplex(x * freq, y * freq) * amp;
            maxAmp += amp;
            amp *= persistance;
            freq *= 2;
        }

        noise /= maxAmp;

        noise = noise * (high - low) / 2f + (high + low) / 2f;

        return noise;
    }
}