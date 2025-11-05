using System.Text;
using UnityEngine;
using UnityEngine.UI; 
using Google.XR.ARCoreExtensions; 

public class GeospatialDebugger : MonoBehaviour
{
    /* public ARGeospatialManager GeospatialManager;
        public Text DebugText; 
    private StringBuilder sb = new StringBuilder();
    void Update()
    {
        if (GeospatialManager == null || DebugText == null)
        {
            return;
        }
      
        sb.Clear();       
        sb.AppendLine($"State: {GeospatialManager.GeospatialState}");   
        if (GeospatialManager.GeospatialState == GeospatialState.Localized)
        {
           
            sb.AppendLine("--- VPS ACTIVE (Likely) ---");
            sb.AppendLine($"Horizontal Acc: {GeospatialManager.HorizontalAccuracy:F2} m");
            sb.AppendLine($"Orientation Yaw Acc: {GeospatialManager.OrientationYawAccuracy:F2} ��");
        }
        else if (GeospatialManager.GeospatialState == GeospatialState.Localizing)
        {
            sb.AppendLine("--- LOCALIZING... ---");
            sb.AppendLine("(Trying to find location...)");
        }
        else
        {      
            sb.AppendLine("--- NOT LOCALIZED ---");
            sb.AppendLine("(GPS/VPS Failed. Using compass fallback.)");
        }
        DebugText.text = sb.ToString();
    }
    */
}