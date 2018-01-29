using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public Button playStop;
    public Text segName;
    //public Button edit;


    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        
        playStop.onClick.AddListener(() => { ConstructionEdit.instance.EnablePhysics(!ConstructionEdit.PhysEnabled); });
    }

    public void PlayStatusChange()
    {
        playStop.GetComponentInChildren<Text>().text = ConstructionEdit.PhysEnabled == true ? "Pause" : "Play";
    }

    public void SelectedSegmentChange(string name)
    {
        segName.text = name;
    }
}
