using UnityEngine;
using UnityEngine.SceneManagement; // これが必須

public class MenuController : MonoBehaviour
{
    public void LoadVPSScene()
    {
        // "VPSSceneName" は君が付けたシーンファイルの名前に置き換える
        SceneManager.LoadScene("VPS");
    }

    public void LoadGPSScene()
    {
        // "GPSSceneName" は君が付けたシーンファイルの名前に置き換える
        SceneManager.LoadScene("GPS");
    }
    public void LoadButtonScene()
    {
        // "GPSSceneName" は君が付けたシーンファイルの名前に置き換える
        SceneManager.LoadScene("buttonPushAR");
    }
     public void LoadSampleScene()
    {
        // "GPSSceneName" は君が付けたシーンファイルの名前に置き換える
        SceneManager.LoadScene("Sample");
    }

     public void LoadFingerScene()
    {
        // "GPSSceneName" は君が付けたシーンファイルの名前に置き換える
        SceneManager.LoadScene("Finger");
    }

    public void LoadFingerHozionScene()
    {
        // "GPSSceneName" は君が付けたシーンファイルの名前に置き換える
        SceneManager.LoadScene("FingerHolizon");
    }


}