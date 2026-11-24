using System.Collections.Generic;
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
        public string title;
        [TextArea] public string description;
    }

    public List<MarkerPrefab> markers = new List<MarkerPrefab>();
    public float hideDelay = 0.5f;

    public Transform CurrentTarget { get; private set; }
    public MarkerPrefab CurrentInfo { get; private set; }

    ARTrackedImageManager manager;
    readonly Dictionary<string, GameObject> spawned = new Dictionary<string, GameObject>();
    readonly Dictionary<string, float> lastSeen = new Dictionary<string, float>();

    void Awake() { manager = GetComponent<ARTrackedImageManager>(); }
    void OnEnable() { manager.trackablesChanged.AddListener(OnChanged); }
    void OnDisable() { manager.trackablesChanged.RemoveListener(OnChanged); }

    void OnChanged(ARTrackablesChangedEventArgs<ARTrackedImage> args)
    {
        foreach (var img in args.added) Handle(img);
        foreach (var img in args.updated) Handle(img);
    }

    void Handle(ARTrackedImage img)
    {
        string n = img.referenceImage.name;
        var entry = markers.Find(m => m.imageName == n);
        if (entry == null || entry.prefab == null) return;

        if (!spawned.TryGetValue(n, out var go) || go == null)
        {
            go = Instantiate(entry.prefab, img.transform);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            go.SetActive(false);
            spawned[n] = go;
        }

        if (img.trackingState == TrackingState.Tracking)
            lastSeen[n] = Time.time;
    }

    void Update()
    {
        float newest = -1f;
        Transform best = null;
        MarkerPrefab bestInfo = null;

        foreach (var kv in spawned)
        {
            if (kv.Value == null) continue;

            bool visible = lastSeen.TryGetValue(kv.Key, out var t) && Time.time - t < hideDelay;
            kv.Value.SetActive(visible);

            if (visible && t > newest)
            {
                newest = t;
                best = kv.Value.transform;
                bestInfo = markers.Find(m => m.imageName == kv.Key);
            }
        }

        CurrentTarget = best;
        CurrentInfo = bestInfo;
    }
}