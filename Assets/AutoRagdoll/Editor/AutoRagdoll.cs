using UnityEngine;
using UnityEngine.Networking;
using UnityEditor;
using System;
using System.IO;
using System.Reflection;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
public class AutoRagdoll11 : EditorWindow 
{
	enum SelectedMode
	{
		ColliderMove,
		ColliderRotate,
		ColliderScale,
	}
	
	GameObject m_Character;
	Collider[] _colliders;
	Transform[] _transforms;
	SelectedMode _selectedMode = SelectedMode.ColliderMove;
	Quaternion _lastRotation;
	bool _buttonPressed;
	int _curPointIndex = -1;// current selected collider
	SelectedMode _lastSelectedMode;	// to detect, if mode changed from last frame
	PivotMode _lastPivotMode;
	PivotRotation _lastPivotRotation;
	Dictionary<string, Transform> _symmetricBones;
	
	void OnGUI ()
	{
		m_Character = EditorGUILayout.ObjectField("Character", m_Character, typeof(GameObject), true) as GameObject;
		if (GUILayout.Button("Build"))
		{
			AddRagdoll();
			OnSelectionChange();
		}
		//if (GUILayout.Button("Remove"))
		//{
		//	RemoveRagdoll();
		//}
	}
	
	[MenuItem("Tools/AutoRagdoll &r", false, 11)]
	public static void ShowWindow()
	{
		var window = EditorWindow.GetWindow<AutoRagdoll11>(true, "AutoRagdoll");
		window.minSize = window.maxSize = new Vector2(360, 60);
	}
	
	/// <summary>
	/// Opens Unity's Ragdoll Builder and populates as many fields as it can.
	/// </summary>
	private void AddRagdoll()
	{
		var ragdollBuilderType = Type.GetType("UnityEditor.RagdollBuilder, UnityEditor");
		var windows = Resources.FindObjectsOfTypeAll(ragdollBuilderType);
		// Open the Ragdoll Builder if it isn't already opened.
		if (windows == null || windows.Length == 0) 
		{
			EditorApplication.ExecuteMenuItem("GameObject/3D Object/Ragdoll...");
			windows = Resources.FindObjectsOfTypeAll(ragdollBuilderType);
		}

		if (windows != null && windows.Length > 0) 
		{
			var ragdollWindow = windows[0] as ScriptableWizard;
			if(m_Character==null)
			{
				Debug.LogError("Please asign the Character!");
			}
			
			var animator = m_Character.GetComponentInChildren<Animator>();
			SetFieldValue(ragdollWindow, "pelvis", animator.GetBoneTransform(HumanBodyBones.Hips));
			SetFieldValue(ragdollWindow, "leftHips", animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg));
			SetFieldValue(ragdollWindow, "leftKnee", animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg));
			SetFieldValue(ragdollWindow, "leftFoot", animator.GetBoneTransform(HumanBodyBones.LeftFoot));
			SetFieldValue(ragdollWindow, "rightHips", animator.GetBoneTransform(HumanBodyBones.RightUpperLeg));
			SetFieldValue(ragdollWindow, "rightKnee", animator.GetBoneTransform(HumanBodyBones.RightLowerLeg));
			SetFieldValue(ragdollWindow, "rightFoot", animator.GetBoneTransform(HumanBodyBones.RightFoot));
			SetFieldValue(ragdollWindow, "leftArm", animator.GetBoneTransform(HumanBodyBones.LeftUpperArm));
			SetFieldValue(ragdollWindow, "leftElbow", animator.GetBoneTransform(HumanBodyBones.LeftLowerArm));
			SetFieldValue(ragdollWindow, "rightArm", animator.GetBoneTransform(HumanBodyBones.RightUpperArm));
			SetFieldValue(ragdollWindow, "rightElbow", animator.GetBoneTransform(HumanBodyBones.RightLowerArm));
			SetFieldValue(ragdollWindow, "middleSpine", animator.GetBoneTransform(HumanBodyBones.Spine));
			SetFieldValue(ragdollWindow, "head", animator.GetBoneTransform(HumanBodyBones.Head));

			var method = ragdollWindow.GetType().GetMethod("CheckConsistency", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			if (method != null) 
			{
				ragdollWindow.errorString = (string)method.Invoke(ragdollWindow, null);
				ragdollWindow.isValid = string.IsNullOrEmpty(ragdollWindow.errorString);
			}
		}
	}

	/// <summary>
	/// Use reflection to set the value of the field.
	/// </summary>
	private void SetFieldValue(ScriptableWizard obj, string name, object value)
	{
		if (value == null)
		{
			return;
		}

		var field = obj.GetType().GetField(name);
		if (field != null) 
		{
			field.SetValue(obj, value);
		}
	}
	/// <summary>
	/// Find symmetric bones. (e.g. for left arm, it finds right arm and for right leg it finds left leg)
	/// </summary>
	static Dictionary<string, Transform> FindSymmetricBones(Animator animator)
	{
		
		if(animator==null) return null;
		
		var symBones = new Dictionary<string, Transform>();
		// feet
		symBones.Add(animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg).name, animator.GetBoneTransform(HumanBodyBones.RightLowerLeg));
		symBones.Add(animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg).name, animator.GetBoneTransform(HumanBodyBones.RightUpperLeg));

		symBones.Add(animator.GetBoneTransform(HumanBodyBones.RightLowerLeg).name, animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg));
		symBones.Add(animator.GetBoneTransform(HumanBodyBones.RightUpperLeg).name, animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg));

		// hands
		symBones.Add(animator.GetBoneTransform(HumanBodyBones.LeftLowerArm).name, animator.GetBoneTransform(HumanBodyBones.RightLowerArm));
		symBones.Add(animator.GetBoneTransform(HumanBodyBones.LeftUpperArm).name, animator.GetBoneTransform(HumanBodyBones.RightUpperArm));

		symBones.Add(animator.GetBoneTransform(HumanBodyBones.RightLowerArm).name, animator.GetBoneTransform(HumanBodyBones.LeftLowerArm));
		symBones.Add(animator.GetBoneTransform(HumanBodyBones.RightUpperArm).name, animator.GetBoneTransform(HumanBodyBones.LeftUpperArm));

		return symBones;
	}

	void OnSelectionChange()
	{
		SceneView.RepaintAll();
		if(Selection.activeGameObject)
		{
			_symmetricBones = FindSymmetricBones(Selection.activeGameObject.GetComponentInChildren<Animator>());
		}
	}
	
	
	void OnEnable()
	{
		SceneView.onSceneGUIDelegate += OnSceneGUI;
	}
	
	void OnDisable()
	{
		SceneView.onSceneGUIDelegate -= OnSceneGUI;
		Tools.hidden = false;
	}
	
	void OnSceneGUI(SceneView sceneView)
	{
		if(Selection.activeGameObject)
		{
			CheckSelectedMode();
			FindColliders();
			DrawControls();
			SceneView.RepaintAll();
		}
	}
	
	/// <summary>
	/// Method intended to be invoked before Drawing GUI
	/// </summary>
	void CheckSelectedMode()
	{
		_selectedMode = GetCurrentMode();
		
		// if selected item was changed, research colliders
		if (_selectedMode != _lastSelectedMode |
			_lastPivotMode != Tools.pivotMode |
			_lastPivotRotation != Tools.pivotRotation)
		{

			_lastSelectedMode = _selectedMode;
			_lastPivotMode = Tools.pivotMode;
			_lastPivotRotation = Tools.pivotRotation;
		}
	}
	
	/// <summary>
	/// Find all "CapsuleCollider", "BoxCollider" and "SphereCollider" colliders
	/// </summary>
	void FindColliders()
	{
		if (Selection.activeGameObject == null)
		{
			_transforms = new Transform[0];
			return;
		}

		var cColliders = Selection.activeGameObject.GetComponentsInChildren<CapsuleCollider>();
		var bColliders = Selection.activeGameObject.GetComponentsInChildren<BoxCollider>();
		var sColliders = Selection.activeGameObject.GetComponentsInChildren<SphereCollider>();
		_colliders = new Collider[cColliders.Length + bColliders.Length + sColliders.Length];
		cColliders.CopyTo(_colliders, 0);
		bColliders.CopyTo(_colliders, cColliders.Length);
		sColliders.CopyTo(_colliders, cColliders.Length + bColliders.Length);

		_transforms = new Transform[_colliders.Length];
		for (int i = 0; i < _colliders.Length; ++i)
		{
			Transform transform = _colliders[i].transform;
			if (transform.name.EndsWith(AutoRagdollUtility.ColliderNodeSufix, false, CultureInfo.InvariantCulture))
				transform = transform.parent;
			_transforms[i] = transform;
		}
	}
	
	
	SelectedMode GetCurrentMode()
	{
		Tools.hidden=true;
		switch (Tools.current)
		{
		case Tool.None:
		case Tool.Move:
			_selectedMode = SelectedMode.ColliderMove;
			break;
		case Tool.Rotate:
			_selectedMode = SelectedMode.ColliderRotate;

			break;
		case Tool.Scale:
		case Tool.Rect:
			_selectedMode = SelectedMode.ColliderScale;
			break;
		}

		return _selectedMode;
	}
	
	/// <summary>
	/// Draws controls of selected mode.
	/// </summary>
	void DrawControls()
	{
		for (int i = 0; i < _transforms.Length; i++)
		{
			Transform transform = _transforms[i];
			//if(transform.GetComponent<Rigidbody>()==null) continue;
			Vector3 pos = AutoRagdollUtility.GetRotatorPosition(transform);
			float size = HandleUtility.GetHandleSize(pos);

			if (Handles.Button(pos, Quaternion.identity, size / 6f, size / 6f, Handles.SphereHandleCap))
			{
				_curPointIndex = i;
				
				Quaternion rotatorRotation2 = AutoRagdollUtility.GetRotatorRotation(transform);
				
				if (!_buttonPressed)
				{
					_lastRotation = rotatorRotation2;
					_buttonPressed = true;
				}
				//Selection.activeGameObject = transform.gameObject;
			}
			else
				_buttonPressed = false;

			if (_curPointIndex != i)
				continue;

			// if current point controll was selected
			// draw other controls over it
			Quaternion rotatorRotation = AutoRagdollUtility.GetRotatorRotation(transform);
			switch (_selectedMode)
			{
			case SelectedMode.ColliderRotate:
				ProcessRotation(rotatorRotation, transform, pos);
				break;
			case SelectedMode.ColliderMove:
				ProcessColliderMove(rotatorRotation, transform, pos);
				break;
			case SelectedMode.ColliderScale:
				ProcessColliderScale(rotatorRotation, transform, pos);
				break;
			}
		}
	}
	
	/// <summary>
	/// Rotate node's colider though controls
	/// </summary>
	void ProcessRotation(Quaternion rotatorRotation, Transform transform, Vector3 pos)
	{
		Quaternion newRotation;
		bool changed;

		if (Tools.pivotRotation == PivotRotation.Global)
		{
			Quaternion fromStart = rotatorRotation * Quaternion.Inverse(_lastRotation);
			newRotation = Handles.RotationHandle(fromStart, pos);
			changed = fromStart != newRotation;
			newRotation = newRotation * _lastRotation;
		}
		else
		{
			newRotation = Handles.RotationHandle(rotatorRotation, pos);
			changed = rotatorRotation != newRotation;
		}

		if (changed)
		{
			transform = AutoRagdollUtility.GetRotatorTransform(transform);
			AutoRagdollUtility.RotateCollider(transform, newRotation);
		}
		
	}

	/// <summary>
	/// Resize collider though controls
	/// </summary>
	/// <param name="transform">The node the collider is attached to</param>
	static void ProcessColliderMove(Quaternion rotatorRotation, Transform transform, Vector3 pos)
	{
		if (Tools.pivotRotation == PivotRotation.Global)
			rotatorRotation = Quaternion.identity;

		Vector3 newPosition = Handles.PositionHandle(pos, rotatorRotation);
		Vector3 translateBy = newPosition - pos;

		if (translateBy != Vector3.zero)
			AutoRagdollUtility.SetColliderPosition(transform, newPosition);
	}

	/// <summary>
	/// Move collider though controls
	/// </summary>
	void ProcessColliderScale(Quaternion rotatorRotation, Transform transform, Vector3 pos)
	{
		float size = HandleUtility.GetHandleSize(pos);
		var collider = AutoRagdollUtility.GetCollider(transform);

		// process each collider type in its own way
		CapsuleCollider cCollider = collider as CapsuleCollider;
		BoxCollider bCollider = collider as BoxCollider;
		SphereCollider sCollider = collider as SphereCollider;

		if (cCollider != null)
		{
			// for capsule collider draw circle and two dot controllers
			Vector3 direction = DirectionIntToVector(cCollider.direction);

			var t = Quaternion.LookRotation(cCollider.transform.TransformDirection(direction));

			// method "Handles.ScaleValueHandle" multiplies size on 0.15f
			// so to send exact size to "Handles.CircleCap",
			// I needed to multiply size on 1f/0.15f
			// Then to get a size a little bigger (to 130%) than
			// collider (for nice looking purpose), I multiply size by 1.3f
			const float magicNumber = 1f / 0.15f * 1.3f;

			// draw radius controll
				
			float radius = Handles.ScaleValueHandle(cCollider.radius, pos, t, cCollider.radius * magicNumber, Handles.CircleHandleCap, 0);
			bool radiusChanged = cCollider.radius != radius;

			Vector3 scaleHeightShift = cCollider.transform.TransformDirection(direction * cCollider.height / 2);

			// draw height controlls
			Vector3 heightControl1Pos = pos + scaleHeightShift;
			Vector3 heightControl2Pos = pos - scaleHeightShift;

			float height1 = Handles.ScaleValueHandle(cCollider.height, heightControl1Pos, t, size * 0.5f, Handles.DotHandleCap, 0);
			float height2 = Handles.ScaleValueHandle(cCollider.height, heightControl2Pos, t, size * 0.5f, Handles.DotHandleCap, 0);
			float newHeight = 0f;
				
			bool moved = false;
			bool firstCtrlSelected = false;
			if (height1 != cCollider.height)
			{
				moved = true;
				firstCtrlSelected = true;
				newHeight = height1;
			}
			else if (height2 != cCollider.height)
			{
				moved = true;
				newHeight = height2;
			}
				
			if (moved | radiusChanged)
			{
				Undo.RecordObject(cCollider, "Resize capsule collider");

				bool upperSelected = false;
				if (moved)
				{
					if (newHeight < 0.01f)
						newHeight = 0.01f;

					bool firstIsUpper = FirstIsUpper(cCollider.transform, heightControl1Pos, heightControl2Pos);
					upperSelected = firstIsUpper == firstCtrlSelected;

					cCollider.center += direction * (newHeight - cCollider.height) / 2 * (firstCtrlSelected ? 1 : -1);
					cCollider.height = newHeight;
				}
				if (radiusChanged)
					cCollider.radius = radius;

				// resize symmetric colliders too
				Transform symBone;
				if (_symmetricBones != null && _symmetricBones.TryGetValue(transform.name, out symBone))
				{
					var symCapsule = AutoRagdollUtility.GetCollider(symBone) as CapsuleCollider;
					if (symCapsule == null)
						return;
						
					Undo.RecordObject(symCapsule, "Resize symetric capsule collider");

					if (moved)
					{
						Vector3 direction2 = DirectionIntToVector(symCapsule.direction);

						Vector3 scaleHeightShift2 = symCapsule.transform.TransformDirection(direction2 * symCapsule.height / 2);
						Vector3 pos2 = AutoRagdollUtility.GetRotatorPosition(symCapsule.transform);

						Vector3 heightControl1Pos2 = pos2 + scaleHeightShift2;
						Vector3 heightControl2Pos2 = pos2 - scaleHeightShift2;

						bool firstIsUpper2 = FirstIsUpper(symCapsule.transform, heightControl1Pos2, heightControl2Pos2);

						symCapsule.center += direction2 * (newHeight - symCapsule.height) / 2
							* (upperSelected ? 1 : -1)
							* (firstIsUpper2 ? 1 : -1);

						symCapsule.height = cCollider.height;
					}
					if (radiusChanged)
						symCapsule.radius = cCollider.radius;
				}
			}
			
		}
		else if (bCollider != null)
		{
			// resize Box collider

			var newSize = Handles.ScaleHandle(bCollider.size, pos, rotatorRotation, size);
			if (bCollider.size != newSize)
			{
				Undo.RecordObject(bCollider, "Resize box collider");
				bCollider.size = newSize;
			}
		}
		else if (sCollider != null)
		{
			// resize Sphere collider
			var newRadius = Handles.RadiusHandle(rotatorRotation, pos, sCollider.radius, true);
			if (sCollider.radius != newRadius)
			{
				Undo.RecordObject(sCollider, "Resize sphere collider");
				sCollider.radius = newRadius;
			}
		}
		else
			throw new InvalidOperationException("Unsupported Collider type: " + collider.GetType().FullName);
	}
	
	
	private static bool FirstIsUpper(Transform transform, Vector3 heightControl1Pos, Vector3 heightControl2Pos)
	{
		if (transform.parent == null)
			return true;

		Vector3 currentPos = transform.position;
		Vector3 parentPos;
		do
		{
			transform = transform.parent;
			parentPos = transform.position;
		}
			while (parentPos == currentPos & transform.parent != null);

		if (parentPos == currentPos)	return true;

		Vector3 limbDirection = currentPos - parentPos;

		limbDirection.Normalize();

		float d1 = Vector3.Dot(limbDirection, heightControl1Pos - parentPos);
		float d2 = Vector3.Dot(limbDirection, heightControl2Pos - parentPos);

			
		bool firstIsUpper = d1 < d2;
		return firstIsUpper;
	}

	/// <summary>
	/// Int (Physx spesific) direction to Vector3 direction
	/// </summary>
	static Vector3 DirectionIntToVector(int direction)
	{
		Vector3 v;
		switch (direction)
		{
		case 0:
			v = Vector3.right;
			break;
		case 1:
			v = Vector3.up;
			break;
		case 2:
			v = Vector3.forward;
			break;
		default:
			throw new InvalidOperationException();
		}
		return v;
	}

	/// <summary>
	/// Remove all colliders, joints, and rigids from Selection object  USE IT CAREFULLY
	/// </summary>
	void RemoveRagdoll()
	{
		foreach (var component in Selection.activeGameObject.GetComponentsInChildren<Collider>())
			GameObject.DestroyImmediate(component);
		foreach (var component in Selection.activeGameObject.GetComponentsInChildren<CharacterJoint>())
			GameObject.DestroyImmediate(component);
		foreach (var component in Selection.activeGameObject.GetComponentsInChildren<Rigidbody>())
			GameObject.DestroyImmediate(component);
	}
}
