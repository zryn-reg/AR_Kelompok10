using TMPro;
using UnityEngine;

public class ARInfoPanel : MonoBehaviour
{
    public MultiMarkerSpawner spawner;
    public TMP_Text txtTitle;
    public TMP_Text txtDescription;

    [Header("Teks saat belum ada marker")]
    public string idleTitle = "Arahkan kamera ke marker";
    [TextArea] public string idleDescription = "Scan salah satu marker untuk memunculkan hewan.";

    void Update()
    {
        var info = spawner != null ? spawner.CurrentInfo : null;
        string title = info != null ? info.title : idleTitle;
        string desc = info != null ? info.description : idleDescription;

        if (txtTitle.text != title) txtTitle.text = title;
        if (txtDescription.text != desc) txtDescription.text = desc;
    }
}