using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    [SerializeField]
    ARImageTracker aRImageTracker;
    [SerializeField]
    Button imageTrackInstanceModeToggleBtn;
    public void ImageTrackInstanceModeToggle() => aRImageTracker.ChangeImageTrackInstanciateMode();

    private void Update()
    {
        imageTrackInstanceModeToggleBtn.transform.GetChild(0).GetComponent<Text>().text = aRImageTracker.GetImakeTrackInstanciateModeName;
    }
}
