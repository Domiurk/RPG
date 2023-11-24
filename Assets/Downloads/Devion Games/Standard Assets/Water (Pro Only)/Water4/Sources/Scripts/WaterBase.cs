using UnityEngine;

namespace UnityStandardAssets.Water
{
    public enum WaterQuality
    {
        High = 2,
        Medium = 1,
        Low = 0,
    }

    [ExecuteInEditMode]
    public class WaterBase : MonoBehaviour
    {
        public Material sharedMaterial;
        public WaterQuality waterQuality = WaterQuality.High;
        public bool edgeBlend = true;


        public void UpdateShader()
        {
            if (waterQuality > WaterQuality.Medium)
            {
                sharedMaterial.shader.maximumLOD = 501;
            }
            else if (waterQuality > WaterQuality.Low)
            {
                sharedMaterial.shader.maximumLOD = 301;
            }
            else
            {
                sharedMaterial.shader.maximumLOD = 201;
            }

            if (!SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.Depth))
            {
                edgeBlend = false;
            }

            if (edgeBlend)
            {
                Shader.EnableKeyword("WATER_EDGEBLEND_ON");
                Shader.DisableKeyword("WATER_EDGEBLEND_OFF");

                if (Camera.main)
                {
                    Camera.main.depthTextureMode |= DepthTextureMode.Depth;
                }
            }
            else
            {
                Shader.EnableKeyword("WATER_EDGEBLEND_OFF");
                Shader.DisableKeyword("WATER_EDGEBLEND_ON");
            }
        }


        public void WaterTileBeingRendered(Transform tr, Camera currentCam)
        {
            if (currentCam && edgeBlend)
            {
                currentCam.depthTextureMode |= DepthTextureMode.Depth;
            }
        }


        public void Update()
        {
            if (sharedMaterial)
            {
                UpdateShader();
            }
        }
    }
}