using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityHelpers;

[RequireComponent(typeof(PhysicsTransform))]
public class HandEmulator : MonoBehaviour
{
    private readonly Quaternion wristFixupRotation = new Quaternion(0.0f, 1.0f, 0.0f, 0.0f);

    public Transform handAnchor;

    public Transform[] bones;
    public GameObject hiddenHand;
    private PhysicsTransform[] bonePhysics;

    [SerializeField]
    private OVRSkeleton.IOVRSkeletonDataProvider _dataProvider;
    private PhysicsTransform rootTransform;

    void Awake()
    {
        _dataProvider = GetComponent<OVRSkeleton.IOVRSkeletonDataProvider>();
        rootTransform = GetComponent<PhysicsTransform>();
        //InitBones();
    }

    void Update()
    {
        rootTransform.position = handAnchor.position;
        rootTransform.rotation = handAnchor.rotation;

        if (_dataProvider != null)
        {
            var data = _dataProvider.GetSkeletonPoseData();

            hiddenHand.SetActive(data.IsDataValid);
            if (data.IsDataValid)
            {
                for (var i = 0; i < bones.Length; ++i)
                {
                    if (bones[i] != null)
                    {
                        //bonePhysics[i].position = bonePhysics[i].transform.position;
                        //bonePhysics[i].rotation = bonePhysics[i].transform.parent.rotation * data.BoneRotations[i].FromFlippedXQuatf();
                        bones[i].localRotation = data.BoneRotations[i].FromFlippedXQuatf();
                        if (i == (int)OVRSkeleton.BoneId.Hand_WristRoot)
                        {
                            bones[i].localRotation *= wristFixupRotation;
                            //bonePhysics[i].rotation *= wristFixupRotation;
                        }
                    }
                }
            }
        }
    }

    private void InitBones()
    {
        bonePhysics = new PhysicsTransform[bones.Length];
        for (int i = 0; i < bones.Length; i++)
        {
            var physicsTransform = bones[i].GetComponent<PhysicsTransform>();
            bonePhysics[i] = physicsTransform;

            physicsTransform.striveForPosition = false;
            physicsTransform.striveForOrientation = true;
            physicsTransform.counteractGravity = true;
        }
    }
}
