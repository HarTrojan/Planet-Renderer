using UnityEngine;
using UnityEngine.UI;
using System;

public class PlanetRenderer : MonoBehaviour
{
    [SerializeField] private RawImage rawImage;
    [SerializeField] private ComputeShader computeShader;
    [SerializeField] private int width = 1024;
    [SerializeField] private int height = 1024;
    [SerializeField] private int focalLength = 768;
    [SerializeField] private int octaves = 20;
    [SerializeField] private float startFrequency = 9.06f;
    [SerializeField] private float startAmplitude = 1.0f;
    [SerializeField] private float warpStrength = 0.4f;
    [SerializeField] private float normalStrength = 0.01f;
    [SerializeField] private bool absoluteTerrain = false;

    private RenderTexture texture;
    private int kernel;
    private Vector3 planetPos = new Vector3(0, 0, 0);
    private float planetRadius = 512;
    private Vector3 cameraPos = new Vector3(0, 0, -1024);
    private float sunAngle = -MathF.PI/8;
    private Vector3 sunDir;


    public void SetCameraPosition(Vector3 newPos)
    {
        cameraPos = newPos;

        // Camera direction calculation
        Vector3 cameraDir = (planetPos - cameraPos).normalized;

        // Camera position and direction is sent to the shader
        computeShader.SetFloats("CameraPos",
            cameraPos.x,
            cameraPos.y,
            cameraPos.z
        );

        computeShader.SetFloats("CameraDir",
            cameraDir.x,
            cameraDir.y,
            cameraDir.z
        );
    }


    void Start()
    {
        // Shader kernel setup
        kernel = computeShader.FindKernel("CSMain");

        // Creation of output texture
        texture = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32);
        texture.enableRandomWrite = true;
        texture.Create();

        rawImage.texture = texture;

        // Shader output setup
        computeShader.SetTexture(kernel, "Result", texture);
        computeShader.SetInts("Size", width, height);
        computeShader.SetInts("FocalLength", focalLength);

        // Sun direction calculation
        sunDir = new Vector3(MathF.Cos(sunAngle), 0.0f, MathF.Sin(sunAngle));

        // Parameters sent to shader
        computeShader.SetFloats("PlanetPos", planetPos.x, planetPos.y, planetPos.z);
        computeShader.SetFloats("PlanetRadius", planetRadius);
        computeShader.SetFloats("CameraPos", cameraPos.x, cameraPos.y, cameraPos.z);
        computeShader.SetFloats("SunDir", sunDir.x, sunDir.y, sunDir.z);

        computeShader.SetInt("Octaves", octaves);
        computeShader.SetFloat("StartFrequency", startFrequency);
        computeShader.SetFloat("StartAmplitude", startAmplitude);
        computeShader.SetFloat("WarpStrength", warpStrength);
        computeShader.SetFloat("NormalStrength", normalStrength);
        computeShader.SetBool("AbsoluteTerrain", absoluteTerrain);
    }


    void Update()
    {   
        // Parameters sent to shader each frame
        computeShader.SetInt("Octaves", octaves);
        computeShader.SetInts("FocalLength", focalLength);
        computeShader.SetFloat("StartFrequency", startFrequency);
        computeShader.SetFloat("StartAmplitude", startAmplitude);
        computeShader.SetFloat("WarpStrength", warpStrength);
        computeShader.SetFloat("NormalStrength", normalStrength);
        computeShader.SetBool("AbsoluteTerrain", absoluteTerrain);

        // Thread number calculation
        int groupsX = Mathf.CeilToInt(width / 8f);
        int groupsY = Mathf.CeilToInt(height / 8f);

        // Shader dispatch
        computeShader.Dispatch(kernel, groupsX, groupsY, 1);
    }


    void OnDestroy()
    {
        if (texture != null)
            texture.Release();
    }
}