using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BridgeTrackablePrefabModifier : MonoBehaviour
{
    [SerializeField]
    Transform rotationFixer;
    [SerializeField]
    Transform bridgeStartPoint;
    [SerializeField]
    Transform bridgeLastPoint;
    public float bridgeLastPointScaleModifyCoefficient;

    [SerializeField]
    Text distanceIndicatorText;
    [SerializeField]
    Canvas distanceIndicatorCanvas;

    Transform destTr;
    public void SetDest(Transform tr)=>destTr = tr;


    private void Awake()
    {
        bridgeLastPointScaleModifyCoefficient = bridgeLastPoint.localScale.y;
        SetUpTheBridge();
    }

    void SetUpTheBridge()
    {
        destTr = null;
        bridgeStartPoint.gameObject.SetActive(false);
    }

    public Transform debugDest;
    [ContextMenu("asd")]
    void debug() { MakeBridge(debugDest); }




    public void MakeBridge(Transform tr)
    {
        SetDest(tr);
        bridgeStartPoint.gameObject.SetActive(true);
    }

    private void Update()
    {
        if (!destTr) return;
        Vector3 differ = destTr.position - transform.position;
        float distance = differ.magnitude;
        rotationFixer.LookAt(destTr);
        bridgeStartPoint.localScale = new Vector3(bridgeStartPoint.localScale.x, bridgeStartPoint.localScale.y, distance);
        bridgeLastPoint.localScale = new Vector3(bridgeLastPoint.localScale.x,
            (1/ distance) *bridgeLastPointScaleModifyCoefficient,
            bridgeLastPoint.localScale.z);

        distanceIndicatorCanvas.transform.localPosition = new Vector3(0, distanceIndicatorCanvas.transform.localPosition.y
            , distance * 0.5f * (1/gameObject.transform.localScale.z));
        distanceIndicatorCanvas.transform.LookAt(Camera.main.transform);
        distanceIndicatorCanvas.transform.Rotate(new Vector3(0f, 180f, 0f));
        distanceIndicatorText.text = distance.ToString();
    }

}
