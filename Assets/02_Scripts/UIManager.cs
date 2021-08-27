using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
public class UIManager : MonoBehaviour
{
    [SerializeField]
    ARImageTracker aRImageTracker;
    [SerializeField]
    ARPlaneManager arPlaneManager;
    
    [SerializeField]
    Button imageTrackInstanceModeToggleBtn;
    [SerializeField]
    Text arPlaneManagerIndicator;
    private void OnEnable()
    {
        aRImageTracker.ImageTrackInstanciateModeChangeListen(true,ImageTrackModeIndicate);
        imageTrackInstanceModeToggleBtn.transform.GetChild(0).GetComponent<Text>().text = aRImageTracker.GetImakeTrackInstanciateModeName;
    }
    private void OnDisable()
    {
        aRImageTracker.ImageTrackInstanciateModeChangeListen(false, ImageTrackModeIndicate);
    }
    /// <summary>
    /// button click method
    /// </summary>
    public void ImageTrackInstanceModeToggle() => aRImageTracker.ChangeImageTrackInstanciateMode();

    public void ARPlaneManagerToggle()
    {
        if(arPlaneManager.enabled)
        {
            foreach(var tr in  arPlaneManager.trackables)
            {
                tr.gameObject.SetActive(false);
            }
            arPlaneManager.enabled = false;
            arPlaneManagerIndicator.text = "Plane Detect OFF";
        }
        else
        {
            foreach (var tr in arPlaneManager.trackables)
            {
                tr.gameObject.SetActive(true);
            }
            
            arPlaneManager.enabled = true;
            arPlaneManagerIndicator.text = "Plane Detect ON";
        }
    }

    void ImageTrackModeIndicate()
    {
        imageTrackInstanceModeToggleBtn.transform.GetChild(0).GetComponent<Text>().text = aRImageTracker.GetImakeTrackInstanciateModeName;
    }

  
}
