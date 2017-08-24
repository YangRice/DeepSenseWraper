using UnityEngine;
using Meta;
using DepthSenseWarper;
using System.Runtime.InteropServices;

///<summary> An example to use camera feed via code.</summary>
///
///<seealso cref="T:UnityEngine.MonoBehaviour"/>
public class CameraFeedExample : MonoBehaviour
{

    public int sourceDevice = 1;  //  for color feed texture set value = 0, for depth set value = 1, for ir set value = 2;
    /*WARNING: the depthdata is converted to rgb space for display purposes. The values in the depth texture do not represent the actual depth value*/

    public MeshRenderer renderTarget;

    public Texture2D cameraTexture;

    private bool registered = false;

    public short depthThreshold = 150;
    private DepthSenseWarper.DepthExport.NewReceivedData depthWarper = new DepthSenseWarper.DepthExport.NewReceivedData();
    private DepthSenseWarper.ColorExport.NewReceivedData colorWarper = new DepthSenseWarper.ColorExport.NewReceivedData();

    void Start()
    {
        Warper.Initialize();
    }

    void Stop()
    {
        Warper.Finalization();
    }

    Color32 GetDepthColor(short depth)
    {
        byte d = (byte)(depth & 0x00ff);
        if (depth < 0x100)
            return new Color32(0xff, d, 0x00, 0xff);
        else if (depth < 0x200)
            return new Color32((byte)(0xff - d), 0xff, 0x00, 0xff);
        else if (depth < 0x300)
            return new Color32(0x00, 0xff, d, 0xff);
        else if (depth < 0x400)
            return new Color32(0x00, (byte)(0xff - d), 0xff, 0xff);
        else if (depth < 0x500)
            return new Color32(d, 0x00, 0xff, 0xff);
        else if (depth < 0x600)
            return new Color32(0xff, d, 0xff, 0xff);
        else
            return Color.white;
    }

    void Update()
    {
        depthThreshold = (depthThreshold < (short)0 ? (short)0 : depthThreshold);
        if (!registered)
        {

            DeviceTextureSource.Instance.registerTextureDevice(sourceDevice);
            //get the texture
            if (DeviceTextureSource.Instance.IsDeviceTextureRegistered(sourceDevice))
            {
                registered = true;
                cameraTexture = DeviceTextureSource.Instance.GetDeviceTexture(sourceDevice);

                // if a rendering target is set. Display it
                if (renderTarget != null && renderTarget.material != null)
                {
                    if (DeviceTextureSource.Instance != null && DeviceTextureSource.Instance.enabled)
                    {
                        renderTarget.material.mainTexture = cameraTexture;
                    }
                }
            }
        }
        else
        {
            colorWarper.GetData();
            depthWarper.GetData();
            DepthSenseWarper.DepthExport.NewSampleReceivedData depth = depthWarper.data;

            if (colorWarper.data.colorMap != null && depth.depthMap != null && depth.uvMap != null && depth.confidenceMap != null)
            {
                const int depthHeight = 240;
                const int depthWidth = 320;
                int i = 0;
                /*!< QVGA (320x240) */
                for (int y = depthHeight - 1; y >= 0; y--)
                    for (int x = 0; x < depthWidth; x++, i++)
                    {
                        if (depth.confidenceMap[i] < depthThreshold)
                            continue;
                        short d = depthWarper.data.depthMap[i];
                        UV uv = depthWarper.data.uvMap[i];
                        cameraTexture.SetPixel(
                            (int)(uv.u * cameraTexture.width),
                            cameraTexture.height - (int)(uv.v * cameraTexture.height),
                            GetDepthColor(d));
                    }
                cameraTexture.Apply();
            }
        }
    }

    void OnDestroy()
    {
        if (DeviceTextureSource.Instance != null)
        {
            DeviceTextureSource.Instance.unregisterTextureDevice(sourceDevice);
        }
    }

}
