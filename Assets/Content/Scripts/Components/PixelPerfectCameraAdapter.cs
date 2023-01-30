using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

/// <summary>
///   Adapt a pixel perfect camera to the dynamic resolution
/// </summary>
public class PixelPerfectCameraAdapter : MonoBehaviour
{
    private PixelPerfectCamera pixelPerfectCamera;
    private float pixelRatio;

    private void Awake()
    {
        pixelPerfectCamera = GetComponent<PixelPerfectCamera>();
        pixelRatio = (pixelPerfectCamera.refResolutionX + pixelPerfectCamera.refResolutionY) / pixelPerfectCamera.assetsPPU;
    }

    private void Update()
    {
        pixelPerfectCamera.refResolutionX = Screen.width;
        pixelPerfectCamera.refResolutionY = Screen.height;
        pixelPerfectCamera.assetsPPU = (int)((pixelPerfectCamera.refResolutionX + pixelPerfectCamera.refResolutionY) / pixelRatio);
    }
}