using UnityEngine;
using UnityEngine.UI; // Textを使うために必要

/// <summary>
/// 3Dオブジェクトをタッチしてドラッグ＆ドロップし、
/// ピンチで拡大・縮小、ツイストで回転も行うスクリプト。
/// AR Cameraにアタッチすることを推奨。
/// </summary>
public class DragAndDropController : MonoBehaviour
{
    [Header("Core Settings")]
    [Tooltip("Rayを飛ばすメインカメラ（AR Camera）")]
    public Camera mainCamera;

    [Tooltip("ドラッグ可能なオブジェクトに付けるタグ名")]
    public string draggableTag = "Draggable";

    [Header("Sensitivity")]
    [Tooltip("ドラッグの移動感度。1.0が標準。")]
    public float moveSensitivity = 1.0f;

    [Tooltip("ピンチ（拡大・縮小）の感度")]
    public float scaleSensitivity = 0.01f;

    [Tooltip("ツイスト（回転）の感度")]
    public float rotateSensitivity = 0.5f;

    [Header("Debug")]
    [Tooltip("現在の状態を表示するUIテキスト")]
    public Text debugText;

    // --- プライベート（内部管理用）変数 ---
    private Transform _draggedObject = null;
    private float _distanceToCamera;
    private Vector3 _lastWorldPosition;
    private Transform _originalParent = null;

    /// <summary>
    /// 起動時にテキストを初期化
    /// </summary>
    void Start()
    {
        if (debugText != null)
        {
            debugText.text = "Idle"; // 待機状態
        }
    }

    /// <summary>
    /// 毎フレームの入力処理
    /// </summary>
    void Update()
    {
        // --- 0. 入力状態の初期化 ---
        Vector2 screenPosition = Vector2.zero;
        bool grabBegan = false;
        bool isGrabbing = false;
        bool grabEnded = false;

        // --- A. モバイル操作 ---
        if (Input.touchCount > 0)
        {
            // 2本指のピンチ＆ツイスト操作
            if (Input.touchCount == 2)
            {
                if (_draggedObject != null)
                {
                    HandlePinch(); // スケール変更を実行
                    HandleTwist(); // 回転処理を実行
                }
            }
            // 1本指のドラッグ操作
            else if (Input.touchCount == 1)
            {
                Touch touch = Input.GetTouch(0);
                screenPosition = touch.position;
                if (touch.phase == TouchPhase.Began) grabBegan = true;
                else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary) isGrabbing = (_draggedObject != null);
                else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) grabEnded = true;
            }
        }
        // --- B. PC操作（デバッグ用）---
        else
        {
            screenPosition = Input.mousePosition;
            if (Input.GetMouseButtonDown(0)) grabBegan = true;
            else if (Input.GetMouseButton(0)) isGrabbing = (_draggedObject != null);
            else if (Input.GetMouseButtonUp(0)) grabEnded = true;
        }

        // --- C. 処理の実行 ---
        if (grabBegan)
        {
            HandleGrab(screenPosition);
        }
        else if (isGrabbing)
        {
            HandleDrag(screenPosition);
        }
        else if (grabEnded)
        {
            HandleDrop();
        }
    }

    /// <summary>
    /// ① 掴む処理：指定座標のオブジェクトを探す
    /// </summary>
    private void HandleGrab(Vector2 screenPosition)
    {
        Ray ray = mainCamera.ScreenPointToRay(screenPosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.CompareTag(draggableTag))
            {
                _draggedObject = hit.transform;
                _distanceToCamera = Vector3.Distance(hit.point, mainCamera.transform.position);
                _lastWorldPosition = mainCamera.ScreenToWorldPoint(new Vector3(
                    screenPosition.x, screenPosition.y, _distanceToCamera));

                _originalParent = _draggedObject.parent;
                _draggedObject.SetParent(null);

                if (debugText != null)
                {
                    // ★テキスト更新：掴んだ瞬間の位置を表示
                    debugText.text = $"Grabbed: {_draggedObject.name}\nPos: {_draggedObject.position.ToString("F2")}";
                }
            }
            else
            {
                if (debugText != null)
                {
                    debugText.text = "Touched (Hit: " + hit.collider.name + ")";
                }
            }
        }
        else
        {
            if (debugText != null)
            {
                debugText.text = "Touched (Miss)";
            }
        }
    }

    /// <summary>
    /// ② ドラッグ処理：掴んだオブジェクトを指に追従させる（Y軸ロック）
    /// </summary>
    private void HandleDrag(Vector2 screenPosition)
    {
        if (_draggedObject == null) return;

        Vector3 currentWorldPosition = mainCamera.ScreenToWorldPoint(new Vector3(
            screenPosition.x, screenPosition.y, _distanceToCamera));
        Vector3 worldDelta = currentWorldPosition - _lastWorldPosition;

        worldDelta.y = 0; // Y軸（垂直方向）の移動を強制的にゼロにする

        _draggedObject.position += worldDelta * moveSensitivity;
        _lastWorldPosition = currentWorldPosition;

        // ★★★ テキスト更新：ドラッグ中の位置を毎フレーム表示 ★★★
        if (debugText != null)
        {
            // .ToString("F2") は小数点以下2桁まで表示するという意味
            debugText.text = $"Grabbing: {_draggedObject.name}\nPos: {_draggedObject.position.ToString("F2")}";
        }
    }

    /// <summary>
    /// ③ 離す処理：掴んでいるオブジェクトを解放する
    /// </summary>
    private void HandleDrop()
    {
        if (_draggedObject != null)
        {
            _draggedObject.SetParent(_originalParent);
            _originalParent = null;

            if (debugText != null)
            {
                debugText.text = "Dropped (Idle)";
            }
            _draggedObject = null;
        }
        else
        {
            if (debugText != null)
            {
                debugText.text = "Idle";
            }
        }
    }

    /// <summary>
    /// ④ 2本指ピンチ処理：オブジェクトを拡大・縮小する
    /// </summary>
    private void HandlePinch()
    {
        if (_draggedObject == null) return;

        Touch touchZero = Input.GetTouch(0);
        Touch touchOne = Input.GetTouch(1);

        Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
        Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

        float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
        float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

        float difference = currentMagnitude - prevMagnitude;
        float scaleChange = difference * scaleSensitivity;

        _draggedObject.localScale += new Vector3(scaleChange, scaleChange, scaleChange);

        if (_draggedObject.localScale.x <= 0.01f)
        {
            _draggedObject.localScale = new Vector3(0.01f, 0.01f, 0.01f);
        }

        // ★★★ テキスト更新：ピンチ中の位置を表示 ★★★
        if (debugText != null)
        {
            debugText.text = $"Pinching: {_draggedObject.name}\nPos: {_draggedObject.position.ToString("F2")}";
        }
    }

    /// <summary>
    /// ⑤ 2本指ツイスト処理：オブジェクトを回転させる
    /// </summary>
    private void HandleTwist()
    {
        if (_draggedObject == null) return;

        Touch touchZero = Input.GetTouch(0);
        Touch touchOne = Input.GetTouch(1);

        Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
        Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

        Vector2 prevDirection = touchOnePrevPos - touchZeroPrevPos;
        Vector2 currentDirection = touchOne.position - touchZero.position;

        float angleDelta = (Mathf.Atan2(currentDirection.y, currentDirection.x) - Mathf.Atan2(prevDirection.y, prevDirection.x)) * Mathf.Rad2Deg;
        float rotationAmount = angleDelta * rotateSensitivity;

        _draggedObject.Rotate(Vector3.up, -rotationAmount, Space.World);

        // ★★★ テキスト更新：回転中の位置を表示 ★★★
        // （ピンチとツイストは同時に起こるので、Pinch側のテキスト更新が優先されるかもしれないが、念のため）
        if (debugText != null)
        {
            debugText.text = $"Rotating: {_draggedObject.name}\nPos: {_draggedObject.position.ToString("F2")}";
        }
    }
}