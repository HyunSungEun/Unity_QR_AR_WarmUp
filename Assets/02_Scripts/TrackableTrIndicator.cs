using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrackableTrIndicator : MonoBehaviour
{
    Text indicatorText;
    string QRName;
    public void HangUp(string QRName)
    {
        this.QRName = QRName;
        indicatorText = GameObject.Find(QRName + "Text").GetComponent<Text>();

    }
    private void Update()
    {
        if (indicatorText == null) return;
        indicatorText.text =QRName+"."+ gameObject.name+"/"+ transform.position.ToString();
    }
    private void OnDestroy()
    {
        indicatorText.text = "ÆÄ±«µÊ";
    }
}
