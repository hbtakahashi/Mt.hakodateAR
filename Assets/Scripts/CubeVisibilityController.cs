using UnityEngine;
using UnityEngine.UI; // Textを使うために必要

/// <summary>
/// ターゲットとなるGameObjectの表示・非表示を制御するクラス
/// </summary>
public class CubeVisibilityController : MonoBehaviour
{
    [Header("制御対象オブジェクト")]
    [Tooltip("表示/非表示を切り替えるCubeオブジェクト（表示対象）")]
    public GameObject targetCube;

    [Tooltip("非表示にするOutlineCubeオブジェクト（非表示対象）")]
    public GameObject outlineCube;

    [Tooltip("ボタン群をまとめている親オブジェクト（このオブジェクトも非表示にする）")]
    public GameObject buttonsContainer;

    // ★★★ 1. カメラのTransformを Inspector から設定する ★★★
    [Header("AR")]
    [Tooltip("ARカメラのTransform（targetCubeの親だったオブジェクト）")]
    public Transform arCameraTransform;

    [Header("UI")]
    [Tooltip("ボタンが押されたか確認するためのテキスト")]
    public Text debugText;

    // ★★★ 2. Cubeのカメラからの「相対的な初期位置」を記憶する変数 ★★★
    private Vector3 _targetCubeInitialLocalPosition;
    private Quaternion _targetCubeInitialLocalRotation;

    void Start()
    {
        if (targetCube != null)
        {
            // 起動時は非表示にしておく
            targetCube.SetActive(false);

            // ★★★ 3. 起動時の「相対位置」を記憶しておく ★★★
            // (これをやらないと、リセット時に元の位置に戻せない)
            _targetCubeInitialLocalPosition = targetCube.transform.localPosition;
            _targetCubeInitialLocalRotation = targetCube.transform.localRotation;
        }

        if (outlineCube != null)
        {
            outlineCube.SetActive(true);
        }

        if (buttonsContainer != null)
        {
            buttonsContainer.SetActive(true);
        }

        if (debugText != null)
        {
            debugText.text = "待機中...";
        }

        // arCameraTransformが未設定なら警告を出す
        if (arCameraTransform == null)
        {
            Debug.LogError("arCameraTransform が設定されていません！ AR Camera をドラッグ＆ドロップしてください。");
        }
    }

    /// <summary>
    /// Cubeを表示し、Outlineとボタン群を非表示にするメソッド
    /// </summary>
    public void ShowTarget_HideOutlineAndButtons()
    {
        // 1. targetCube を「表示」する
        if (targetCube != null)
        {
            // ★★★ 4. 親子関係を解除し、ワールドに固定する ★★★
            // これでカメラ（親）が動いても、Cubeは動かなくなる
            targetCube.transform.SetParent(null);

            targetCube.SetActive(true);
        }
        else
        {
            Debug.LogWarning("targetCubeが設定されていません。");
        }

        // 2. outlineCube を「非表示」にする (変更なし)
        if (outlineCube != null)
        {
            outlineCube.SetActive(false);
        }
        else
        {
            Debug.LogWarning("outlineCubeが設定されていません。");
        }

        // 3. ボタン群のコンテナを「非表示」にする (変更なし)
        if (buttonsContainer != null)
        {
            buttonsContainer.SetActive(false);
        }
        else
        {
            Debug.LogWarning("buttonsContainerが設定されていません。");
        }

        // 4. デバッグテキストの更新 (変更なし)
        if (debugText != null)
        {
            debugText.text = "Cubeを表示し、Outlineとボタンを非表示にしました。";
        }
    }

    /// <summary>
    /// Cubeを非表示にし、Outlineとボタン群を「再表示」する（戻る）
    /// </summary>
    public void ResetView()
    {
        // 1. targetCube を「非表示」に戻す
        if (targetCube != null)
        {
            targetCube.SetActive(false);

            // ★★★ 5. 親子関係を元に戻し、ARカメラの子に戻す ★★★
            targetCube.transform.SetParent(arCameraTransform);

            // ★★★ 6. 記憶しておいた「相対位置」に戻す ★★★
            // (これをしないと、次に表示したときに変な位置に出る)
            targetCube.transform.localPosition = _targetCubeInitialLocalPosition;
            targetCube.transform.localRotation = _targetCubeInitialLocalRotation;
        }

        // 2. outlineCube を「表示」に戻す (変更なし)
        if (outlineCube != null)
        {
            outlineCube.SetActive(true);
        }

        // 3. ボタン群のコンテナを「表示」に戻す (変更なし)
        if (buttonsContainer != null)
        {
            buttonsContainer.SetActive(true);
        }

        // 4. デバッグテキストの更新 (変更なし)
        if (debugText != null)
        {
            debugText.text = "待機中...";
        }
    }
}