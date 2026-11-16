using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class TrackStatus : MonoBehaviour
{
    ARTrackedImageManager manager;

    void Start()
    {
        manager = FindFirstObjectByType<ARTrackedImageManager>();
    }

    void OnGUI()
    {
        GUI.skin.label.fontSize = 40;
        string lib = manager != null && manager.referenceLibrary != null
            ? manager.referenceLibrary.count.ToString()
            : "KOSONG";
        int tracked = manager != null ? manager.trackables.count : -1;
        GUI.Label(new Rect(20, 120, 1000, 200),
            "Library: " + lib + "\nTerdeteksi: " + tracked);
    }
}