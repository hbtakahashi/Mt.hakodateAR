using UnityEngine;

public class ObjectRotator : MonoBehaviour
{
    // インスペクターで回転・移動させたいオブジェクトを指定
    public Transform objectToRotate;

    // インスペクターでシーン内のメインカメラを指定
    public Transform mainCamera;

    // 回転・移動の速度
    public float rotationStep = 0.1f;
    public float transformStep = 0.1f;

    // 内部フラグ
    private bool isRotatingLeft = false;
    private bool isRotatingRight = false;
    private bool isMovingFront = false;
    private bool isMovingBack = false;

    void Update()
    {
        // オブジェクトが設定されていなければ何もしない
        if (objectToRotate == null) return;

        // --- 回転処理 ---
        if (isRotatingLeft)
        {
            objectToRotate.Rotate(0f, -rotationStep, 0f, Space.World);
        }
        else if (isRotatingRight)
        {
            objectToRotate.Rotate(0f, rotationStep, 0f, Space.World);
        }

        // --- ここから修正：移動処理（カメラの向き基準 / Y軸（上下）を無視）---
        if (mainCamera != null)
        {
            // 1. カメラの前方ベクトルを取得
            Vector3 moveDirection = mainCamera.forward;

            // 2. Y軸の動きを0にする（これで水平方向のベクトルになる）
            moveDirection.y = 0;

            // 3. 正規化（Normalize）して、ベクトルの長さを1に戻す
            //    -> Yを0にした影響でベクトルの長さが変わっても、移動速度を一定に保つため
            //    -> sqrMagnitudeは負荷の軽い長さチェック。ベクトルが(0,0,0)（カメラが真上/真下を向いた時）のエラーを防ぐ
            if (moveDirection.sqrMagnitude > 0.001f)
            {
                moveDirection.Normalize();

                if (isMovingFront)
                {
                    // 水平方向の前方へ移動
                    objectToRotate.Translate(moveDirection * transformStep, Space.World);
                }
                else if (isMovingBack)
                {
                    // 水平方向の後方へ移動
                    objectToRotate.Translate(-moveDirection * transformStep, Space.World);
                }
            }
        }
    }

    // --- ボタン用関数（変更なし）---
    public void OnPressLeft() { isRotatingLeft = true; }
    public void OnReleaseLeft() { isRotatingLeft = false; }
    public void OnPressRight() { isRotatingRight = true; }
    public void OnReleaseRight() { isRotatingRight = false; }
    public void OnPressFront() { isMovingFront = true; }
    public void OnReleaseFront() { isMovingFront = false; }
    public void OnPressBack() { isMovingBack = true; }
    public void OnReleaseBack() { isMovingBack = false; }
}