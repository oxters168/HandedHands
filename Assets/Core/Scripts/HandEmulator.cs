using System;
using UnityEngine;
using UnityHelpers;

public class HandEmulator : MonoBehaviour
{
    private readonly Quaternion wristFixupRotation = new Quaternion(0.0f, 1.0f, 0.0f, 0.0f);

    //public float jointDistanceMultiplier = 10;

    public Transform handAnchor;

    public Transform trackedRoot;
    public Transform[] trackedBones;
    public PhysicsTransform[] physicsBones;
    public Transform[] meshBones;
    private Quaternion[] cachedRotations;

    //public GameObject hiddenHand;

    [SerializeField]
    private OVRSkeleton.IOVRSkeletonDataProvider _dataProvider;

    void Awake()
    {
        _dataProvider = GetComponent<OVRSkeleton.IOVRSkeletonDataProvider>();
        CacheRotations();
    }
    void Update()
    {
        if (_dataProvider != null)
        {
            var data = _dataProvider.GetSkeletonPoseData();

            trackedRoot.position = handAnchor.position;
            trackedRoot.rotation = handAnchor.rotation;

            //hiddenHand.SetActive(data.IsDataValid);
            if (data.IsDataValid && data.IsDataHighConfidence)
            {
                for (var i = 0; i < trackedBones.Length; ++i)
                {
                    var trackedBone = trackedBones[i];
                    var physicsBone = physicsBones[i];
                    var meshBone = meshBones[i];

                    if (trackedBone != null)
                    {
                        trackedBone.localRotation = data.BoneRotations[i].FromFlippedXQuatf();
                        if (i == (int)OVRSkeleton.BoneId.Hand_WristRoot)
                            trackedBone.localRotation *= wristFixupRotation;

                        if (physicsBone != null)
                        {
                            if (i == (int)OVRSkeleton.BoneId.Hand_WristRoot)
                            {
                                //physicsBone.AffectedBody.MovePosition(trackedBone.position);
                                //physicsBone.AffectedBody.MoveRotation(trackedBone.rotation);
                                physicsBone.position = trackedBone.position;
                                physicsBone.rotation = trackedBone.rotation;
                            }
                            else
                            {
                                //Use physics position if traced position too far off
                                //var physicsPosition = physicsBone.anchor.TransformPoint(trackedBone.localPosition);
                                //if ((trackedBone.position - physicsBone.anchor.position).sqrMagnitude > (physicsPosition - physicsBone.anchor.position).sqrMagnitude)
                                //    physicsBone.position = physicsPosition;
                                //else
                                //    physicsBone.position = trackedBone.position;
                                //physicsBone.rotation = trackedBone.rotation;

                                //var joint = physicsBone.GetComponent<ConfigurableJoint>();
                                var joint = physicsBone.joint;
                                //joint.SetTargetRotationLocal(trackedBone.localRotation, cachedRotations[i]); //This function is heavy, it has two cross functions in it
                                joint.SetTargetRotation(trackedBone.localRotation, cachedRotations[i]);
                                //joint.targetRotation = Quaternion.identity * (cachedRotations[i] * Quaternion.Inverse(trackedBone.localRotation));
                                //joint.targetPosition = trackedBone.position;

                                //physicsBone.localPosition = trackedBone.localPosition;
                                //physicsBone.localRotation = trackedBone.localRotation;

                                physicsBone.position = trackedBone.position;
                                //physicsBone.rotation = trackedBone.rotation;
                            }

                            if (meshBone != null)
                            {
                                meshBone.position = physicsBone.transform.position;
                                meshBone.rotation = physicsBone.transform.rotation;
                            }
                        }
                    }
                }
            }
        }
    }

    private void CacheRotations()
    {
        cachedRotations = new Quaternion[trackedBones.Length];
        for (int i = 0; i < trackedBones.Length; i++)
            if (trackedBones[i] != null)
                cachedRotations[i] = trackedBones[i].localRotation;
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