using System.Collections.Generic;
using UnityEngine;

public class ARObjectController : MonoBehaviour
{
    [Header("Sumber target")]
    public MultiMarkerSpawner spawner;

    [Header("Rotation")]
    public float rotateStep = 15f;

    [Header("Scale (relatif terhadap scale awal objek)")]
    public float scaleFactor = 1.15f;
    public float minMultiplier = 0.4f;
    public float maxMultiplier = 3f;

    [Header("Feedback")]
    public AudioSource audioSource;

    readonly Dictionary<Transform, Vector3> baseScale = new Dictionary<Transform, Vector3>();

    Transform GetTarget()
    {
        if (spawner == null || spawner.CurrentTarget == null)
        {
            Debug.LogWarning("Target AR belum ditemukan. Scan marker terlebih dahulu.");
            return null;
        }
        return spawner.CurrentTarget;
    }

    public void RotateLeft()  { Rotate(-rotateStep); }
    public void RotateRight() { Rotate(rotateStep); }
    public void ScaleUp()     { Scale(scaleFactor); }
    public void ScaleDown()   { Scale(1f / scaleFactor); }

    void Rotate(float degrees)
    {
        var t = GetTarget();
        if (t == null) return;
        t.Rotate(0f, degrees, 0f, Space.Self);
        PlayFeedback();
    }

    void Scale(float factor)
    {
        var t = GetTarget();
        if (t == null) return;

        if (!baseScale.TryGetValue(t, out var start))
        {
            start = t.localScale;
            baseScale[t] = start;
        }

        float current = t.localScale.x / Mathf.Max(start.x, 0.0001f);
        float desired = Mathf.Clamp(current * factor, minMultiplier, maxMultiplier);
        t.localScale = start * desired;
        PlayFeedback();
    }

    void PlayFeedback()
    {
        if (audioSource == null) return;
        audioSource.Stop();
        audioSource.Play();
    }
}