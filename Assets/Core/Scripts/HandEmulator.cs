using System;
using UnityEngine;
using UnityHelpers;

public class HandEmulator : MonoBehaviour
{
    private readonly Quaternion wristFixupRotation = new Quaternion(0.0f, 1.0f, 0.0f, 0.0f);

    public Transform handAnchor;

    public Transform trackedRoot;
    public Transform[] trackedBones;
    public PhysicsTransform[] physicsBones;
    public Transform[] meshBones;

    //public GameObject hiddenHand;

    [SerializeField]
    private OVRSkeleton.IOVRSkeletonDataProvider _dataProvider;

    void Awake()
    {
        _dataProvider = GetComponent<OVRSkeleton.IOVRSkeletonDataProvider>();
        //InitBones();
    }

    void Update()
    {
        if (_dataProvider != null)
        {
            var data = _dataProvider.GetSkeletonPoseData();

            trackedRoot.position = handAnchor.position;
            trackedRoot.rotation = handAnchor.rotation;

            //hiddenHand.SetActive(data.IsDataValid);
            if (data.IsDataValid)
            {
                for (var i = 0; i < trackedBones.Length; ++i)
                {
                    if (trackedBones[i] != null)
                    {
                        trackedBones[i].localRotation = data.BoneRotations[i].FromFlippedXQuatf();
                        if (i == (int)OVRSkeleton.BoneId.Hand_WristRoot)
                            trackedBones[i].localRotation *= wristFixupRotation;

                        if (physicsBones[i] != null)
                        {
                            physicsBones[i].position = trackedBones[i].position;
                            physicsBones[i].rotation = trackedBones[i].rotation;

                            if (meshBones[i] != null)
                            {
                                meshBones[i].position = physicsBones[i].transform.position;
                                meshBones[i].rotation = physicsBones[i].transform.rotation;
                            }
                        }
                    }
                }
            }
        }
    }
}

[Serializable]
public class PhysicsBone
{
    [SerializeField]
    public Transform anchor;
    [SerializeField]
    public PhysicsTransform bone;
}