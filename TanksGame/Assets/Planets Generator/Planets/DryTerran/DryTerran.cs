
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class DryTerran : MonoBehaviour, IPlanet {
    [FormerlySerializedAs("Land")]
    [Header("References")]
    [SerializeField] private GameObject landMesh; 
    [SerializeField] private GradientTextureGenerate gradientLand;

    private Material land;

    [Header("Gradient Settings")]
    [SerializeField] private string gradientTextureKey = "_GradientTex";
    
    [Header("Colors")]
    [SerializeField] private Color[] planetColors = new[] {
        new Color(1f, 0.537f, 0.2f),    // #ff8933
        new Color(0.902f, 0.271f, 0.224f), // #e64539
        new Color(0.678f, 0.184f, 0.271f), // #ad2f45
        new Color(0.322f, 0.2f, 0.247f),   // #52333f
        new Color(0.239f, 0.161f, 0.212f)  // #3d2936
    };
    [SerializeField] private float[] times = new float[] { 0, 0.2f, 0.4f, 0.6f, 0.8f, 1.0f };
    
    private readonly GradientColorKey[] colorKey = new GradientColorKey[5];
    private readonly GradientAlphaKey[] alphaKey = new GradientAlphaKey[5];
    
    private void Start()
    {
        land = landMesh.GetComponent<Image>().material;
        SetGragientColor();
    }
    
    public void SetPixel(float amount)
    {
        land.SetFloat(ShaderProperties.Key_Pixels, amount);
    }

    public void SetLight(Vector2 pos)
    {
        land.SetVector(ShaderProperties.Key_Light_origin, pos);
    }

    public void SetSeed(float seed)
    {
        var convertedSeed = seed % 1000f / 100f;
        land.SetFloat(ShaderProperties.Key_Seed, convertedSeed);
    }

    public void SetRotate(float r)
    {
        land.SetFloat(ShaderProperties.Key_Rotation, r);
    }

    public void UpdateTime(float time)
    {
        land.SetFloat(ShaderProperties.Key_time, time  );
    }

    public void SetCustomTime(float time)
    {
        var dt = 10f + time * 60f;
        land.SetFloat(ShaderProperties.Key_time, dt);
    }

    public void SetInitialColors()
    {
        SetGragientColor();
    }

    private void SetGragientColor()
    {
        for (var i = 0; i < colorKey.Length; i++)
        {
            colorKey[i].color = planetColors[i];
            colorKey[i].time = times[i];
            alphaKey[i].alpha = 1.0f;
            alphaKey[i].time = times[i];
        }
        gradientLand?.SetColors(colorKey, alphaKey);
    }
    public Color[] GetColors()
    {
        return null;
    }
    
    public void SetColors(Color[] planetColors)
    {
        
    }
}
