using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrackaablePrefabSpecIndicator : MonoBehaviour
{
    public Text wp;
    public Text s;
    public Text r;
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
        wp.text = string.Format("({0:0.00#},{0:0.00#},{0:0.00#})",  entity.position.x, entity.position.y, entity.position.z);
        s.text = string.Format("({0:0.00#},{0:0.00#},{0:0.00#})", entity.localScale .x, entity.localScale.y, entity.localScale.z);
        r.text = string.Format("({0:0.00#},{0:0.00#},{0:0.00#})", entity.rotation.eulerAngles .x, entity.rotation.eulerAngles.y, entity.rotation.eulerAngles.z);
    }
}
