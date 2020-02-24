/************************************************************************************
Copyright : Copyright (c) Facebook Technologies, LLC and its affiliates. All rights reserved.

Licensed under the Oculus Utilities SDK License Version 1.31 (the "License"); you may not use
the Utilities SDK except in compliance with the License, which is provided at the time of installation
or download, or which otherwise accompanies this software in either electronic or hard copy form.

You may obtain a copy of the License at
https://developer.oculus.com/licenses/utilities-1.31

Unless required by applicable law or agreed to in writing, the Utilities SDK distributed
under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF
ANY KIND, either express or implied. See the License for the specific language governing
permissions and limitations under the License.
************************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-80)]
public class OVRPhysicsSkeleton : MonoBehaviour
{
	[SerializeField]
	private OVRPlugin.SkeletonType _skeletonType = OVRPlugin.SkeletonType.None;
	[SerializeField]
	private OVRSkeleton.IOVRSkeletonDataProvider _dataProvider;

	[SerializeField]
	private bool _updateRootPose = false;
	[SerializeField]
	private bool _updateRootScale = false;
	[SerializeField]
	private bool _enablePhysicsCapsules = false;

	private GameObject _bonesGO;
	private GameObject _bindPosesGO;
	private GameObject _capsulesGO;

	protected List<OVRBone> _bones;
	private List<OVRBone> _bindPoses;
	private List<OVRBoneCapsule> _capsules;

	private readonly Quaternion wristFixupRotation = new Quaternion(0.0f, 1.0f, 0.0f, 0.0f);
	public bool IsInitialized { get; private set; }
	public bool IsDataValid { get; private set; }
	public bool IsDataHighConfidence { get; private set; }
	public IList<OVRBone> Bones { get; protected set; }
	public IList<OVRBone> BindPoses { get; private set; }
	public IList<OVRBoneCapsule> Capsules { get; private set; }
	public OVRPlugin.SkeletonType GetSkeletonType() { return _skeletonType; }

	#region From OVRCustomSkeleton.cs
	[SerializeField]
	private List<Transform> _customBones = new List<Transform>(new Transform[(int)OVRPlugin.BoneId.Max]);

	#if UNITY_EDITOR
	private static readonly string[] _fbxBoneNames =
	{
		"wrist",
		"forearm_stub",
		"thumb0",
		"thumb1",
		"thumb2",
		"thumb3",
		"index1",
		"index2",
		"index3",
		"middle1",
		"middle2",
		"middle3",
		"ring1",
		"ring2",
		"ring3",
		"pinky0",
		"pinky1",
		"pinky2",
		"pinky3"
	};

	private static readonly string[] _fbxFingerNames =
	{
		"thumb",
		"index",
		"middle",
		"ring",
		"pinky"
	};
	private static readonly string[] _handPrefix = { "l_", "r_" };
	#endif

	public List<Transform> CustomBones { get { return _customBones; } }

	#if UNITY_EDITOR
	public void TryAutoMapBonesByName()
	{
		OVRPlugin.BoneId start = GetCurrentStartBoneId();
		OVRPlugin.BoneId end = GetCurrentEndBoneId();
		OVRPlugin.SkeletonType skeletonType = GetSkeletonType();
		if (start != OVRPlugin.BoneId.Invalid && end != OVRPlugin.BoneId.Invalid)
		{
			for (int bi = (int)start; bi < (int)end; ++bi)
			{
				string fbxBoneName = FbxBoneNameFromBoneId(skeletonType, (OVRPlugin.BoneId)bi);
				Transform t = transform.FindChildRecursive(fbxBoneName);

				if (t != null)
				{
					_customBones[(int)bi] = t;
				}
			}
		}
	}

	private static string FbxBoneNameFromBoneId(OVRPlugin.SkeletonType skeletonType, OVRPlugin.BoneId bi)
	{
		if (bi >= OVRPlugin.BoneId.Hand_ThumbTip && bi <= OVRPlugin.BoneId.Hand_PinkyTip)
		{
			return _handPrefix[(int)skeletonType] + _fbxFingerNames[(int)bi - (int)OVRPlugin.BoneId.Hand_ThumbTip] + "_finger_tip_marker";
		}
		else
		{
			return "b_" + _handPrefix[(int)skeletonType] + _fbxBoneNames[(int)bi];
		}
	}
	#endif
	#endregion
	private void Awake()
	{
		if (_dataProvider == null)
		{
			_dataProvider = GetComponent<OVRSkeleton.IOVRSkeletonDataProvider>();
		}

		_bones = new List<OVRBone>();
		Bones = _bones.AsReadOnly();

		_bindPoses = new List<OVRBone>();
		BindPoses = _bindPoses.AsReadOnly();

		_capsules = new List<OVRBoneCapsule>();
		Capsules = _capsules.AsReadOnly();
	}

	private void Start()
	{
		if (_skeletonType != OVRPlugin.SkeletonType.None)
		{
			Initialize();
		}
	}

	private void Initialize()
	{
		var skeleton = new OVRPlugin.Skeleton();
		if (OVRPlugin.GetSkeleton(_skeletonType, out skeleton))
		{
			InitializeBones(skeleton);
			InitializeBindPose(skeleton);
			InitializeCapsules(skeleton);

			IsInitialized = true;
		}
	}

	/*virtual protected void InitializeBones(OVRPlugin.Skeleton skeleton)
	{
		_bones = new List<OVRBone>(new OVRBone[skeleton.NumBones]);
		Bones = _bones.AsReadOnly();

		if (!_bonesGO)
		{
			_bonesGO = new GameObject("Bones");
			_bonesGO.transform.SetParent(transform, false);
			_bonesGO.transform.localPosition = Vector3.zero;
			_bonesGO.transform.localRotation = Quaternion.identity;
		}

		// pre-populate bones list before attempting to apply bone hierarchy
		for (int i = 0; i < skeleton.NumBones; ++i)
		{
			OVRSkeleton.BoneId id = (OVRSkeleton.BoneId)skeleton.Bones[i].Id;
			short parentIdx = skeleton.Bones[i].ParentBoneIndex;
			Vector3 pos = skeleton.Bones[i].Pose.Position.FromFlippedXVector3f();
			Quaternion rot = skeleton.Bones[i].Pose.Orientation.FromFlippedXQuatf();

			var boneGO = new GameObject(id.ToString());
			boneGO.transform.localPosition = pos;
			boneGO.transform.localRotation = rot;
			_bones[i] = new OVRBone(id, parentIdx, boneGO.transform);
		}

		for (int i = 0; i < skeleton.NumBones; ++i)
		{
			if (((OVRPlugin.BoneId)skeleton.Bones[i].ParentBoneIndex) == OVRPlugin.BoneId.Invalid)
			{
				_bones[i].Transform.SetParent(_bonesGO.transform, false);
			}
			else
			{
				_bones[i].Transform.SetParent(_bones[_bones[i].ParentBoneIndex].Transform, false);
			}
		}
	}*/
	#region From OVRCustomSkeleton.cs
	protected virtual void InitializeBones(OVRPlugin.Skeleton skeleton)
	{
		_bones = new List<OVRBone>(new OVRBone[skeleton.NumBones]);
		Bones = _bones.AsReadOnly();

		for (int i = 0; i < skeleton.NumBones; ++i)
		{
			OVRSkeleton.BoneId id = (OVRSkeleton.BoneId)skeleton.Bones[i].Id;
			short parentIdx = skeleton.Bones[i].ParentBoneIndex;
			Transform t = _customBones[(int)id];
			_bones[i] = new OVRBone(id, parentIdx, t);
		}
	}
	#endregion

	private void InitializeBindPose(OVRPlugin.Skeleton skeleton)
	{
		_bindPoses = new List<OVRBone>(new OVRBone[skeleton.NumBones]);
		BindPoses = _bindPoses.AsReadOnly();

		if (!_bindPosesGO)
		{
			_bindPosesGO = new GameObject("BindPoses");
			_bindPosesGO.transform.SetParent(transform, false);
			_bindPosesGO.transform.localPosition = Vector3.zero;
			_bindPosesGO.transform.localRotation = Quaternion.identity;
		}

		for (int i = 0; i < skeleton.NumBones; ++i)
		{
			OVRSkeleton.BoneId id = (OVRSkeleton.BoneId)skeleton.Bones[i].Id;
			short parentIdx = skeleton.Bones[i].ParentBoneIndex;
			var bindPoseGO = new GameObject(id.ToString());
			OVRBone bone = _bones[i];

			if (bone.Transform != null)
			{
				bindPoseGO.transform.localPosition = bone.Transform.localPosition;
				bindPoseGO.transform.localRotation = bone.Transform.localRotation;
			}

			_bindPoses[i] = new OVRBone(id, parentIdx, bindPoseGO.transform);
		}

		for (int i = 0; i < skeleton.NumBones; ++i)
		{
			if (((OVRPlugin.BoneId)skeleton.Bones[i].ParentBoneIndex) == OVRPlugin.BoneId.Invalid)
			{
				_bindPoses[i].Transform.SetParent(_bindPosesGO.transform, false);
			}
			else
			{
				_bindPoses[i].Transform.SetParent(_bindPoses[_bones[i].ParentBoneIndex].Transform, false);
			}
		}
	}

	private void InitializeCapsules(OVRPlugin.Skeleton skeleton)
	{
		if (_enablePhysicsCapsules)
		{
			_capsules = new List<OVRBoneCapsule>(new OVRBoneCapsule[skeleton.NumBoneCapsules]);
			Capsules = _capsules.AsReadOnly();

			if (!_capsulesGO)
			{
				_capsulesGO = new GameObject("Capsules");
				_capsulesGO.transform.SetParent(transform, false);
				_capsulesGO.transform.localPosition = Vector3.zero;
				_capsulesGO.transform.localRotation = Quaternion.identity;
			}

			_capsules = new List<OVRBoneCapsule>(new OVRBoneCapsule[skeleton.NumBoneCapsules]);
			Capsules = _capsules.AsReadOnly();

			for (int i = 0; i < skeleton.NumBoneCapsules; ++i)
			{
				var capsule = skeleton.BoneCapsules[i];
				Transform bone = Bones[capsule.BoneIndex].Transform;

				var capsuleRigidBodyGO = new GameObject((_bones[capsule.BoneIndex].Id).ToString() + "_CapsuleRigidBody");
				capsuleRigidBodyGO.transform.SetParent(_capsulesGO.transform, false);
				capsuleRigidBodyGO.transform.position = bone.position;
				capsuleRigidBodyGO.transform.rotation = bone.rotation;

				var capsuleRigidBody = capsuleRigidBodyGO.AddComponent<Rigidbody>();
				capsuleRigidBody.mass = 1.0f;
				capsuleRigidBody.isKinematic = true;
				capsuleRigidBody.useGravity = false;
#if UNITY_2018_3_OR_NEWER
				capsuleRigidBody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
#else
				capsuleRigidBody.collisionDetectionMode = CollisionDetectionMode.Continuous;
#endif

				var capsuleColliderGO = new GameObject((_bones[capsule.BoneIndex].Id).ToString() + "_CapsuleCollider");
				capsuleColliderGO.transform.SetParent(capsuleRigidBodyGO.transform, false);
				var capsuleCollider = capsuleColliderGO.AddComponent<CapsuleCollider>();
				var p0 = capsule.Points[0].FromFlippedXVector3f();
				var p1 = capsule.Points[1].FromFlippedXVector3f();
				var delta = p1 - p0;
				var mag = delta.magnitude;
				var rot = Quaternion.FromToRotation(Vector3.right, delta);
				capsuleCollider.radius = capsule.Radius;
				capsuleCollider.height = mag + capsule.Radius * 2.0f;
				capsuleCollider.isTrigger = false;
				capsuleCollider.direction = 0;
				capsuleColliderGO.transform.localPosition = p0;
				capsuleColliderGO.transform.localRotation = rot;
				capsuleCollider.center = Vector3.right * mag * 0.5f;

				_capsules[i] = new OVRBoneCapsule(capsule.BoneIndex, capsuleRigidBody, capsuleCollider);
			}
		}
	}

	private void Update()
	{
		if (!IsInitialized || _dataProvider == null)
		{
			IsDataValid = false;
			IsDataHighConfidence = false;

			return;
		}

		var data = _dataProvider.GetSkeletonPoseData();

		IsDataValid = data.IsDataValid;
		if (data.IsDataValid)
		{
			IsDataHighConfidence = data.IsDataHighConfidence;

			if (_updateRootPose)
			{
				transform.localPosition = data.RootPose.Position.FromFlippedZVector3f();
				transform.localRotation = data.RootPose.Orientation.FromFlippedZQuatf();
			}

			if (_updateRootScale)
			{
				transform.localScale = new Vector3(data.RootScale, data.RootScale, data.RootScale);
			}

			for (var i = 0; i < _bones.Count; ++i)
			{
				if (_bones[i].Transform != null)
				{
					_bones[i].Transform.localRotation = data.BoneRotations[i].FromFlippedXQuatf();
					if (_bones[i].Id == OVRSkeleton.BoneId.Hand_WristRoot)
					{
						_bones[i].Transform.localRotation *= wristFixupRotation;
					}
				}
			}
		}
	}

	private void FixedUpdate()
	{
		if (!IsInitialized || _dataProvider == null)
		{
			IsDataValid = false;
			IsDataHighConfidence = false;

			return;
		}

		Update();

		if (_enablePhysicsCapsules)
		{
			var data = _dataProvider.GetSkeletonPoseData();

			IsDataValid = data.IsDataValid;
			IsDataHighConfidence = data.IsDataHighConfidence;

			for (int i = 0; i < _capsules.Count; ++i)
			{
				OVRBoneCapsule capsule = _capsules[i];
				var capsuleGO = capsule.CapsuleRigidbody.gameObject;

				if (data.IsDataValid && data.IsDataHighConfidence)
				{
					Transform bone = _bones[(int)capsule.BoneIndex].Transform;

					if (capsuleGO.activeSelf)
					{
						capsule.CapsuleRigidbody.MovePosition(bone.position);
						capsule.CapsuleRigidbody.MoveRotation(bone.rotation);
					}
					else
					{
						capsuleGO.SetActive(true);
						capsule.CapsuleRigidbody.position = bone.position;
						capsule.CapsuleRigidbody.rotation = bone.rotation;
					}
				}
				else
				{
					if (capsuleGO.activeSelf)
					{
						capsuleGO.SetActive(false);
					}
				}
			}
		}
	}

	public OVRPlugin.BoneId GetCurrentStartBoneId()
	{
		switch (_skeletonType)
		{
			case OVRPlugin.SkeletonType.HandLeft:
			case OVRPlugin.SkeletonType.HandRight:
				return OVRPlugin.BoneId.Hand_Start;
			case OVRPlugin.SkeletonType.None:
			default:
				return OVRPlugin.BoneId.Invalid;
		}
	}

	public OVRPlugin.BoneId GetCurrentEndBoneId()
	{
		switch (_skeletonType)
		{
			case OVRPlugin.SkeletonType.HandLeft:
			case OVRPlugin.SkeletonType.HandRight:
				return OVRPlugin.BoneId.Hand_End;
			case OVRPlugin.SkeletonType.None:
			default:
				return OVRPlugin.BoneId.Invalid;
		}
	}

	private OVRPlugin.BoneId GetCurrentMaxSkinnableBoneId()
	{
		switch (_skeletonType)
		{
			case OVRPlugin.SkeletonType.HandLeft:
			case OVRPlugin.SkeletonType.HandRight:
				return OVRPlugin.BoneId.Hand_MaxSkinnable;
			case OVRPlugin.SkeletonType.None:
			default:
				return OVRPlugin.BoneId.Invalid;
		}
	}

	public int GetCurrentNumBones()
	{
		switch (_skeletonType)
		{
			case OVRPlugin.SkeletonType.HandLeft:
			case OVRPlugin.SkeletonType.HandRight:
				return GetCurrentEndBoneId() - GetCurrentStartBoneId();
			case OVRPlugin.SkeletonType.None:
			default:
				return 0;
		}
	}

	public int GetCurrentNumSkinnableBones()
	{
		switch (_skeletonType)
		{
			case OVRPlugin.SkeletonType.HandLeft:
			case OVRPlugin.SkeletonType.HandRight:
				return GetCurrentMaxSkinnableBoneId() - GetCurrentStartBoneId();
			case OVRPlugin.SkeletonType.None:
			default:
				return 0;
		}
	}
}
