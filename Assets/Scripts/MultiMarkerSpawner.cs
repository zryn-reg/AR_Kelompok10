using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARTrackedImageManager))]
public class MultiMarkerSpawner : MonoBehaviour
{
    [System.Serializable]
    public class MarkerPrefab
    {
        public string imageName;
        public GameObject prefab;
    }

    public List<MarkerPrefab> markers = new List<MarkerPrefab>();

    ARTrackedImageManager manager;
    readonly Dictionary<string, GameObject> spawned = new Dictionary<string, GameObject>();
    readonly Dictionary<string, string> info = new Dictionary<string, string>();

    void Awake() { manager = GetComponent<ARTrackedImageManager>(); }
    void OnEnable() { manager.trackablesChanged.AddListener(OnChanged); }
    void OnDisable() { manager.trackablesChanged.RemoveListener(OnChanged); }

    void OnChanged(ARTrackablesChangedEventArgs<ARTrackedImage> args)
    {
        foreach (var img in args.added) UpdateImage(img);
        foreach (var img in args.updated) UpdateImage(img);
    }

    void UpdateImage(ARTrackedImage img)
    {
        string n = img.referenceImage.name;
        var entry = markers.Find(m => m.imageName == n);
        string status = img.trackingState.ToString();

        if (entry == null) status += " | NAMA TIDAK COCOK";
        else if (entry.prefab == null) status += " | PREFAB KOSONG";
        else
        {
            if (!spawned.TryGetValue(n, out var go) || go == null)
            {
                go = Instantiate(entry.prefab, img.transform);
                go.transform.localPosition = Vector3.zero;
                go.transform.localRotation = Quaternion.identity;
                spawned[n] = go;
            }
            go.SetActive(img.trackingState != TrackingState.None);
            status += " | OK";
        }
        info[n] = status;
    }

    void OnGUI()
    {
        GUI.skin.label.fontSize = 36;
        var sb = new StringBuilder("Marker:\n");
        foreach (var kv in info) sb.AppendLine(kv.Key + " -> " + kv.Value);
        GUI.Label(new Rect(20, 200, 1000, 600), sb.ToString());
    }
}