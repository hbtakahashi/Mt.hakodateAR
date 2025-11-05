using UnityEngine;
using System.Collections.Generic; // Listを使うために必要

// ----------------------------------------------------------------------
// インスペクタで設定するための構造体
// ----------------------------------------------------------------------
[System.Serializable]
public struct TargetLocation
{
    public string name; // 管理用の名前
    public double latitude;
    public double longitude;
    
    // ★変更点(1): WGS84高度(楕円体高)を指定する項目を追加
    public double altitude; 
    
    public GameObject cubePrefab; 
}

// ----------------------------------------------------------------------
// メインのスクリプト（AR Cameraにアタッチ）
// ----------------------------------------------------------------------
[RequireComponent(typeof(Camera))]
public class PlaceCubesWithGPSCompass : MonoBehaviour
{
    [Header("ターゲット（Cube）のGPS座標リスト")]
    public List<TargetLocation> targetLocations = new List<TargetLocation>();

    [Header("必要なコンポーネント")]
    public LocationUpdater locationUpdater; 

    private List<GameObject> placedCubes = new List<GameObject>();
    private Camera arCamera;

    void Start()
    {
        // (Startメソッドの中身は変更ありません)
        Input.compass.enabled = true;

        if (locationUpdater == null) { /* (エラー処理) */ return; }
        arCamera = GetComponent<Camera>();
        if (targetLocations.Count == 0) { /* (警告処理) */ return; }

        foreach (TargetLocation target in targetLocations)
        {
            if (target.cubePrefab == null) { /* (エラー処理) */ continue; }
            GameObject newCube = Instantiate(target.cubePrefab); 
            newCube.name = $"Cube_{target.name}"; 
            placedCubes.Add(newCube);
        }
    }

    void Update()
    {
        if (locationUpdater.Status != LocationServiceStatus.Running || placedCubes.Count == 0)
        {
            return; 
        }

        // --- 共通情報を取得 ---
        LocationInfo myLocation = locationUpdater.Location;
        float myHeading = Input.compass.trueHeading;
        Vector3 cameraPosition = arCamera.transform.position;
        Quaternion cameraRotationY = Quaternion.Euler(0, arCamera.transform.eulerAngles.y, 0);
        
        // ★変更点(2): プレイヤーの現在のWGS84高度を取得
        double myAltitude = myLocation.altitude;

        // --- すべてのCubeの位置を個別に更新するループ ---
        for (int i = 0; i < placedCubes.Count; i++)
        {
            GameObject currentCube = placedCubes[i];
            TargetLocation currentTarget = targetLocations[i];

            // 4. 2点間の距離と方位を計算 (この関数は2D計算なので変更不要)
            double distance;
            double bearing;
            CalculateDistanceAndBearing(
                myLocation.latitude, myLocation.longitude,
                currentTarget.latitude, currentTarget.longitude, 
                out distance, out bearing
            );

            // 5. カメラから見たターゲットの角度（Y軸回転）
            float angleToTarget = (float)(bearing - myHeading);

            // 6. カメラ（プレイヤー）の位置を基準にCubeの位置を決定する
            Quaternion rotationFromCamera = Quaternion.Euler(0, angleToTarget, 0);
            Vector3 targetPosition = cameraPosition + (cameraRotationY * rotationFromCamera * Vector3.forward * (float)distance);
            
            // ★変更点(3): 高度差を計算し、Y座標に反映
            // ターゲットの高度 と 自分の高度 の差を計算
            double altitudeDifference = currentTarget.altitude - myAltitude;
            
            // カメラのY座標を基準に、高度差を加算
            targetPosition.y = cameraPosition.y + (float)altitudeDifference; 

            currentCube.transform.position = targetPosition;
            currentCube.transform.LookAt(cameraPosition);
        }
    }


    /// <summary>
    /// 2つの緯度経度から距離(m)と方位(度)を計算する (Haversine公式)
    /// ※このメソッドは変更不要です
    /// </summary>
    private void CalculateDistanceAndBearing(double lat1, double lon1, double lat2, double lon2, out double distance, out double bearing)
    {
        // (中身は前回のコードと全く同じ)
        const double R = 6371000;
        double rLat1 = lat1 * Mathf.Deg2Rad;
        double rLon1 = lon1 * Mathf.Deg2Rad;
        double rLat2 = lat2 * Mathf.Deg2Rad;
        double rLon2 = lon2 * Mathf.Deg2Rad;
        double dLat = rLat2 - rLat1;
        double dLon = rLon2 - rLon1;
        double a = Mathf.Sin((float)(dLat / 2)) * Mathf.Sin((float)(dLat / 2)) +
                   Mathf.Cos((float)rLat1) * Mathf.Cos((float)rLat2) *
                   Mathf.Sin((float)(dLon / 2)) * Mathf.Sin((float)(dLon / 2));
        double c = 2 * Mathf.Atan2(Mathf.Sqrt((float)a), Mathf.Sqrt((float)(1 - a)));
        distance = R * c;
        double y = Mathf.Sin((float)dLon) * Mathf.Cos((float)rLat2);
        double x = Mathf.Cos((float)rLat1) * Mathf.Sin((float)rLat2) -
                   Mathf.Sin((float)rLat1) * Mathf.Cos((float)rLat2) * Mathf.Cos((float)dLon);
        bearing = Mathf.Atan2((float)y, (float)x) * Mathf.Rad2Deg;
        bearing = (bearing + 360) % 360;
    }
}