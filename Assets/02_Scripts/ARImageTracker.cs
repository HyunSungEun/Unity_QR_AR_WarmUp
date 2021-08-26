using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.ARFoundation;

public class ARImageTracker : MonoBehaviour
{

    enum ImageTrackInstanciateMode
    {
        Disable,
        SingleObjectPerImage,
        BridgeOnImage,
        MAX
    }
    [SerializeField]
    ImageTrackInstanciateMode mode;
    [SerializeField]
    GameObject bridgePrefab;
    GameObject bridgeInstance;
    [SerializeField]
    GameObject[] singleModePrefab;

    List<ARTrackedImage> trackedImages;

    private ARTrackedImageManager aRTrackedImageManager;
    private void Awake()
    {
        mode = ImageTrackInstanciateMode.Disable;
        aRTrackedImageManager = GetComponent<ARTrackedImageManager>();
        trackedImages = new List<ARTrackedImage>();
    }
    private void Start()
    {
        aRTrackedImageManager.enabled = false;
    }

    public string GetImakeTrackInstanciateModeName { get { return mode.ToString(); } }
    
    public void ChangeImageTrackInstanciateMode()
    {
        DestroyAllTrackedImages();
        aRTrackedImageManager.enabled = false;
        if (bridgeInstance != null)
        {
            Destroy(bridgeInstance);
            bridgeInstance = null;
        }
        mode = (int)mode+1 == (int)ImageTrackInstanciateMode.MAX ? mode = (ImageTrackInstanciateMode)0 : 
            mode = (ImageTrackInstanciateMode)((int)mode + 1);

        switch(mode)
        {
            case ImageTrackInstanciateMode.SingleObjectPerImage:
                aRTrackedImageManager.trackedImagePrefab = singleModePrefab[0];
                aRTrackedImageManager.enabled = true;
                break;

            case ImageTrackInstanciateMode.BridgeOnImage:
                aRTrackedImageManager.trackedImagePrefab = bridgePrefab;
                aRTrackedImageManager.enabled = true;
                break;
        }
    }
    private void OnEnable()
    {
        aRTrackedImageManager.trackedImagesChanged += OnImageChanged;
    }
    private void OnDisable()
    {
        aRTrackedImageManager.trackedImagesChanged -= OnImageChanged;
    }

    void DestroyAllTrackedImages()
    {
        if (trackedImages == null || trackedImages.Count == 0) return;
        while(trackedImages.Count>0)
        {
            Destroy(trackedImages[0].gameObject);
            trackedImages.RemoveAt(0);
        }
    }


    void OnImageChanged(ARTrackedImagesChangedEventArgs args)
    {
        foreach(ARTrackedImage image in args.added)
        {
            if (!trackedImages.Contains(image))
            { 
                trackedImages.Add(image);
                Debug.LogFormat("My Log : Added Name_{0} , goName {1} , pos{2} , refName{3}, child0Name{4}",
                    image.name,image.gameObject.name,image.transform.position.ToString(),image.referenceImage.name
                    ,image.transform.GetChild(0).name
                    );
            }
            else
            {
                Debug.LogFormat("My Log : Added BUT Alreadys Contains Name_{0} , goName {1} , pos{2} , refName{3}, child0Name{4}",
                    image.name, image.gameObject.name, image.transform.position.ToString(), image.referenceImage.name
                     , image.transform.GetChild(0).name
                    );
            }
            switch(mode)
            {
                case ImageTrackInstanciateMode.SingleObjectPerImage:
                    if (image.referenceImage.name == "QR_00")
                    {
                        image.GetComponent<MeshRenderer>().material.color = Color.white;
                        var it = image.gameObject.AddComponent<TrackableTrIndicator>();
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
                    break;
                case ImageTrackInstanciateMode.BridgeOnImage:
                    if(bridgeInstance==null)
                    {
                        bridgeInstance = image.gameObject;
                    }
                    else
                    {
                        Destroy(image.GetComponent<BridgeTrackablePrefabModifier>());
                        image.gameObject.transform.GetChild(0).gameObject.SetActive(false);
                        bridgeInstance.GetComponent<BridgeTrackablePrefabModifier>().MakeBridge(image.transform);
                    }
                    break;
            }
        }
        foreach (ARTrackedImage image in args.removed)
        {
            Debug.LogFormat("My Log : 리무브 Name_{0} , goName {1} , pos{2} , refName{3}, child0Name{4} , 포함여부{5}",
                  image.name, image.gameObject.name, image.transform.position.ToString(), image.referenceImage.name
                   , image.transform.GetChild(0).name,trackedImages.Contains(image)
                  );
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

}
