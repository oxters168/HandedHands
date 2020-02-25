using System;
using UnityEngine;
using UnityHelpers;

public class HandEmulator : MonoBehaviour
{
    private readonly Quaternion wristFixupRotation = new Quaternion(0.0f, 1.0f, 0.0f, 0.0f);

    public Transform handAnchor;

    public Transform meshRoot;
    public Transform[] meshBones;
    public PhysicsBone[] physicsBones;

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

            //hiddenHand.SetActive(data.IsDataValid);
            if (data.IsDataValid)
            {
                for (var i = 0; i < meshBones.Length; ++i)
                {
                    if (meshBones[i] != null && physicsBones[i] != null && physicsBones[i].bone != null && physicsBones[i].anchor != null)
                    {
                        if (i == (int)OVRSkeleton.BoneId.Hand_WristRoot)
                        {
                            physicsBones[i].bone.position = handAnchor.position;
                            physicsBones[i].bone.rotation = handAnchor.rotation;
                            physicsBones[i].bone.rotation *= wristFixupRotation;
                            meshRoot.position = physicsBones[i].bone.transform.position;
                            meshRoot.rotation = physicsBones[i].bone.transform.rotation;
                        }
                        else
                        {
                            physicsBones[i].bone.position = physicsBones[i].anchor.TransformPoint(meshBones[i].localPosition);
                            physicsBones[i].bone.rotation = physicsBones[i].anchor.TransformRotation(data.BoneRotations[i].FromFlippedXQuatf());
                            meshBones[i].rotation = physicsBones[i].bone.transform.rotation;
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