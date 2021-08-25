using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrackaablePrefabSpecIndicator : MonoBehaviour
{
    public Text wp;
    public Text s;
    public Transform entity;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(Camera.main.transform);
        transform.Rotate(new Vector3(0f, 180f, 0f));
        wp.text = entity.position.ToString();
        s.text = entity.localScale.ToString();
    }
}
