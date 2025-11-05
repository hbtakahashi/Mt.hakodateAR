using UnityEngine;
using UnityEngine.EventSystems;

public class TouchController : MonoBehaviour
{
    public Camera mainCamera;

    // ★★★ 変更点1：タグを削除し、レイヤーマスク（LayerMask）を追加 ★★★
    [Tooltip("タッチに反応させたいオブジェクトが含まれるレイヤー")]
    public LayerMask targetLayer;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;
            HandleTouch(Input.mousePosition);
        }
        else if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)) return;
            HandleTouch(Input.GetTouch(0).position);
        }
    }

    private void HandleTouch(Vector2 screenPosition)
    {
        Ray ray = mainCamera.ScreenPointToRay(screenPosition);
        RaycastHit hit;

        // ★★★ 変更点2：Raycastを LayerMask でフィルタリングする ★★★
        // if (Physics.Raycast(ray, out hit, 100.0f))
        // ↓
        if (Physics.Raycast(ray, out hit, 10000.0f, targetLayer))
        {
            // レイヤーマスクで既にフィルタリング済みなので、タグチェックは不要
            // if (hit.collider.CompareTag(targetTag))
            // {

            // 当たったオブジェクトに「ActivatePanel」メッセージを送る
            hit.collider.SendMessage("ActivatePanel", SendMessageOptions.DontRequireReceiver);

            // }
        }
    }
}