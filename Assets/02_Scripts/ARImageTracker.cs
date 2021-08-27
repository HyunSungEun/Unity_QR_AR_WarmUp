using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof( ARTrackedImageManager))]
public class ARImageTracker : MonoBehaviour
{
    enum ImageTrackInstanciateMode
    {
        Disable,
        ImageTrackOn,
        MAX
    }
    [SerializeField]
    ImageTrackInstanciateMode mode;
    System.Action imageTrackInstanciateModeChangeEvent;
    public void ImageTrackInstanciateModeChangeListen(bool subscribe,System.Action callback )
    {
        if (subscribe) imageTrackInstanciateModeChangeEvent += callback;
        else imageTrackInstanciateModeChangeEvent -= callback;
    }
    [SerializeField]
    Dictionary<string, GameObject> prefabDict;
    [SerializeField]
    string[] prfabReferenceImageName;
    [SerializeField]
    GameObject[] prefabs;


    /// <summary>
    /// 트래킹 이미지에 생성된 게임오브젝트 Dictionary , Key = Reference Image Name
    /// </summary>
    [SerializeField]
    Dictionary<string, GameObject> instantiatedDict;
    

    private ARTrackedImageManager aRTrackedImageManager;
    private void Awake()
    {
        mode = ImageTrackInstanciateMode.Disable;
        aRTrackedImageManager = GetComponent<ARTrackedImageManager>();
        instantiatedDict = new Dictionary<string, GameObject>();

        prefabDict = new Dictionary<string, GameObject>();
        for(int i=0;i<prfabReferenceImageName.Length;i++)
        {
            prefabDict.Add(prfabReferenceImageName[i], prefabs[i]);
        }
    }
    private void Start()
    {
        aRTrackedImageManager.enabled = false;
    }

    public string GetImakeTrackInstanciateModeName { get { return mode.ToString(); } }
    
    /// <summary>
    /// Call By UI Mode Toggle button
    /// </summary>
    public void ChangeImageTrackInstanciateMode()
    {
        mode = (int)mode+1 == (int)ImageTrackInstanciateMode.MAX ? mode = (ImageTrackInstanciateMode)0 : 
            mode = (ImageTrackInstanciateMode)((int)mode + 1);

        switch(mode)
        {
            case ImageTrackInstanciateMode.Disable:
                InstantiatedTrackableEnable(false);
                aRTrackedImageManager.enabled = false;
                break;

            case ImageTrackInstanciateMode.ImageTrackOn:
                InstantiatedTrackableEnable(true);
                aRTrackedImageManager.enabled = true;
                break;
        }
        //Debug.Log("My Log : 체인징");
        if (imageTrackInstanciateModeChangeEvent != null)
        {
            imageTrackInstanciateModeChangeEvent();
           // Debug.Log("My Log : 체인징_이벤트 발생");
        }
    }
    /// <summary>
    /// 트랙킹 이미지에 생성된 오브젝트 enable
    /// </summary>
    /// <param name="enable"></param>
    void InstantiatedTrackableEnable(bool enable)
    {
        foreach(GameObject go in instantiatedDict.Values)
        {
            go.SetActive(enable);
        }
    }

   /// <summary>
   /// 브릿지 모델이 생성됐는지 여부 반환
   /// </summary>
   /// <param name="instanciatedGOKey">이미 생성된 브릿지</param>
   /// <returns></returns>
    bool IsBridgePrefabInstanciated(out GameObject instanciatedBridge)
    {
        if (instantiatedDict.ContainsKey("BridgeQR_05") )
        {
            instanciatedBridge = instantiatedDict["BridgeQR_05"];
            return true;
        }
        if (instantiatedDict.ContainsKey("BridgeQR_06"))
        {
            instanciatedBridge = instantiatedDict["BridgeQR_06"];
            return true;
        }
        instanciatedBridge = null;
        return false;

    }

    private void OnEnable()
    {
        aRTrackedImageManager.trackedImagesChanged += OnImageChanged;
    }
    private void OnDisable()
    {
        aRTrackedImageManager.trackedImagesChanged -= OnImageChanged;
    }



    void OnImageChanged(ARTrackedImagesChangedEventArgs args)
    {
        foreach(ARTrackedImage image in args.added)
        {
            image.destroyOnRemoval = true;
            switch(image.referenceImage.name)
            {
                //현재는 이미지 이름에 종속적으로 생성 프리팹 구분
                case "QR_00":
                case "QR_09":
                    InstanciatePrefab(image);
                    break;
                case "BridgeQR_05":
                case "BridgeQR_06":
                    GameObject alreadyBridge;
                    if(IsBridgePrefabInstanciated(out alreadyBridge))
                    {
                        GameObject bridgeDest = new GameObject("BridgeDest");
                        bridgeDest.transform.SetParent(image.transform);
                        instantiatedDict.Add(image.referenceImage.name, bridgeDest);

                        alreadyBridge.GetComponent<BridgeTrackablePrefabModifier>().MakeBridge(bridgeDest.transform);
                    }
                    else
                    {
                        InstanciatePrefab(image);
                    }
                    break;
            }
        }

        foreach (ARTrackedImage image in args.removed)
        {
            Debug.LogFormat("My Log : 리무브 레퍼런스 네임{0}", image.referenceImage.name);
            switch (image.referenceImage.name)
            {
                //현재는 이미지 이름에 종속적으로 생성 프리팹 구분
                case "QR_00":
                case "QR_09":
                    instantiatedDict.Remove(image.referenceImage.name);
                    break;
                case "BridgeQR_05":
                    Destroy(instantiatedDict["BridgeQR_06"].transform.parent.gameObject);
                    instantiatedDict.Remove(image.referenceImage.name);
                    instantiatedDict.Remove("BridgeQR_06");
                    break;
                case "BridgeQR_06":
                    Destroy(instantiatedDict["BridgeQR_05"].transform.parent.gameObject);
                    instantiatedDict.Remove(image.referenceImage.name);
                    instantiatedDict.Remove("BridgeQR_05");
                    break;
            }
            /*
            Debug.LogFormat("My Log : 리무브 Name_{0} , goName {1} , pos{2} , refName{3}, child0Name{4} , 포함여부{5}",
                  image.name, image.gameObject.name, image.transform.position.ToString(), image.referenceImage.name
                   , image.transform.GetChild(0).name, instantiatedDict.ContainsKey(image.referenceImage.name)
                  );
            */
        }


        /*
        Debug.Log(args.ToString());
        foreach(ARTrackedImage image in args.added)
        {
            if(image.referenceImage.name == "QR_00")
            {
                image.GetComponent<MeshRenderer>().material.color = Color.white;
                var it= image.gameObject.AddComponent<TrackableTrIndicator>();
                it.HangUp("QR_00");

            }
            if (image.referenceImage.name == "QR_09")
            {
                GameObject temp = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                image.GetComponent<MeshFilter>().mesh = temp.GetComponent<MeshFilter>().mesh;
                GameObject.Destroy(temp);

                image.GetComponent<MeshRenderer>().material.color = Color.cyan;

                var it = image.gameObject.AddComponent<TrackableTrIndicator>();
                it.HangUp("QR_09");
            }

            Debug.Log("My Log : "+image.name+"/"+image.gameObject.name+"/"+image.trackableId+"/"+image.transform.position.ToString()+"/"+
                image.transform.parent.gameObject.name+"/" + image.transform.parent.gameObject.transform.position.ToString()+
                "/"+image.nativePtr.ToString()+"."+image.trackingState+"/"+image.destroyOnRemoval
                );
            
        }
        */
    }
    /// <summary>
    /// 레퍼런스이미지 이름에 맞는 프리팹 인스턴스 후 instantiatedDict에 추가
    /// </summary>
    /// <param name="image">aRTrackedImageManager.trackedImagesChanged 이벤트의 added에서 참조됨</param>
    /// <returns></returns>
    GameObject InstanciatePrefab(ARTrackedImage image)
    {
        string refImageName = image.referenceImage.name;
        if (instantiatedDict.ContainsKey(refImageName)) return null;
        GameObject prefab = prefabDict[refImageName];
        GameObject GO = Instantiate<GameObject>(prefab, image.transform);
        instantiatedDict.Add(refImageName, GO);
        return GO;

    }

}
