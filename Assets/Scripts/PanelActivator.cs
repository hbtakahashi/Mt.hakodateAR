using UnityEngine;

// Cube側にアタッチするスクリプト
public class PanelActivator : MonoBehaviour
{
    [Tooltip("このオブジェクトがタッチされた時に表示するパネル")]
    public GameObject panelToShow;

    /// <summary>
    /// TouchControllerから呼び出されるメソッド
    /// </summary>
    public void ActivatePanel()
    {
        if (panelToShow != null)
        {
            panelToShow.SetActive(true);
        }
        else
        {
            Debug.LogWarning(this.name + " に panelToShow が設定されていません。");
        }
    }
}