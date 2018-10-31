using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

public static class AutoRagdollUtility
{
	/// <summary>
	/// If you rotate collider, the collider rotates via an additional
	/// node that have the same name + this text.
	/// </summary>
	public const string ColliderNodeSufix = "_ColliderRotator";

	/// <summary>
	/// Get rotation of collider object
	/// </summary>
	public static Quaternion GetRotatorRotation(Transform boneTransform)
	{
		Collider collider = GetCollider(boneTransform);
		return collider.transform.rotation;
	}

	/// <summary>
	/// Get position of collider center
	/// </summary>
	public static Vector3 GetRotatorPosition(Transform boneTransform)
	{
		Collider collider = GetCollider(boneTransform);
		CapsuleCollider cCollider = collider as CapsuleCollider;
		BoxCollider bCollider = collider as BoxCollider;
		SphereCollider sCollider = collider as SphereCollider;

		Vector3 colliderCenter;
		if (cCollider != null) colliderCenter = cCollider.center;
		else if (bCollider != null) colliderCenter = bCollider.center;
		else if (sCollider != null) colliderCenter = sCollider.center;
		else
			throw new InvalidOperationException("Unsupported Collider type: " + collider.GetType().FullName);

		var colliderTransform = collider.transform;
		return colliderTransform.TransformPoint(colliderCenter);
	}

	/// <summary>
	/// Rotate collider without rotating "transform" object.
	/// </summary>
	public static void RotateCollider(Transform transform, Quaternion rotate)
	{
		Vector3 prevPosition = GetColliderPosition(transform);

		Undo.RecordObject(transform, "Rotate collider");
		transform.rotation = rotate;

		SetColliderPosition(transform, prevPosition);
	}

	/// <summary>
	/// Get colliders' center in world space
	/// </summary>
	public static Vector3 GetColliderPosition(Transform transform)
	{
		Collider collider = GetCollider(transform);
		CapsuleCollider cCollider = collider as CapsuleCollider;
		BoxCollider bCollider = collider as BoxCollider;
		SphereCollider sCollider = collider as SphereCollider;

		Vector3 center;
		if (cCollider != null) center = cCollider.center;
		else if (bCollider != null) center = bCollider.center;
		else if (sCollider != null) center = sCollider.center;
		else
			throw new InvalidOperationException("Unsupported Collider type: " + collider.GetType().FullName);

		return collider.transform.TransformPoint(center);
	}

	/// <summary>
	/// Set colliders' center in world space
	/// </summary>
	public static void SetColliderPosition(Transform transform, Vector3 position)
	{
		Collider collider = GetCollider(transform);
		Undo.RecordObject(collider, "Set collider position");

		CapsuleCollider cCollider = collider as CapsuleCollider;
		BoxCollider bCollider = collider as BoxCollider;
		SphereCollider sCollider = collider as SphereCollider;

		Vector3 center = collider.transform.InverseTransformPoint(position);
		
		if (cCollider != null)
		{
			cCollider.center = center;
		}	
		else if (bCollider != null)	
		{
			bCollider.center = center;
		}
		else if (sCollider != null)
		{
			sCollider.center = center;
		} 
		else
		{
			throw new InvalidOperationException("Unsupported Collider type: " + collider.GetType().FullName);
		}
	}

	/// <summary>
	/// Get object a collider attached to. 
	/// </summary>
	public static Collider GetCollider(Transform transform)
	{
		Collider collider = transform.GetComponent<Collider>();
		if (collider == null)
		{
			var rotatorName = transform.name + ColliderNodeSufix;
			var rotatorTransform = transform.Find(rotatorName);
			if (rotatorTransform != null)
				collider = rotatorTransform.GetComponent<Collider>();
		}

		if (collider == null)
		{
			throw new ArgumentException("transform '" + transform.name + "' does not contain collider");
		}

		return collider;
	}
	
	/// <summary>
	/// Gets object a collider attached to.
	/// Collider must have separate GameObject to allow a collider to rotate via it.
	/// So if that GameObject do not exists, creates it.
	/// </summary>
	public static Transform GetRotatorTransform(Transform boneTransform)
	{
		var colliderRotatorName = boneTransform.name + AutoRagdollUtility.ColliderNodeSufix;

		// find rotator node
		var rotatorTransform = boneTransform.Find(colliderRotatorName);
		if (rotatorTransform != null)
			return rotatorTransform;

		// if rotator node was not found, create it
		Collider collider = boneTransform.GetComponent<Collider>();
		if (collider == null)
		{
			throw new ArgumentException("Bone '" + boneTransform.name + "' does not have collider attached to it or ColliderRotatorNode");
		}

		GameObject colliderRotator = new GameObject(colliderRotatorName);
		Undo.RegisterCreatedObjectUndo(colliderRotator, "Create Rotator");
		rotatorTransform = colliderRotator.transform;

		ReattachCollider(boneTransform.gameObject, colliderRotator);
		rotatorTransform.parent = boneTransform;
		rotatorTransform.localPosition = Vector3.zero;
		rotatorTransform.localRotation = Quaternion.identity;
		rotatorTransform.localScale = Vector3.one;

		return colliderRotator.transform;
	}
	
	/// <summary>
	/// Duplicate collider from "from" to "to" and delete it from "from"
	/// </summary>
	static void ReattachCollider(GameObject from, GameObject to)
	{
		var oldCollider = from.GetComponent<Collider>();
		CapsuleCollider cCollider = oldCollider as CapsuleCollider;
		BoxCollider bCollider = oldCollider as BoxCollider;
		if (cCollider != null)
		{
			CapsuleCollider newCapsuleCollider = to.AddComponent<CapsuleCollider>();
			newCapsuleCollider.direction = cCollider.direction;
			newCapsuleCollider.radius = cCollider.radius;
			newCapsuleCollider.height = cCollider.height;
			newCapsuleCollider.center = cCollider.center;
		}
		else if (bCollider != null)
		{
			BoxCollider newBoxCollider = to.AddComponent<BoxCollider>();
			newBoxCollider.size = bCollider.size;
			newBoxCollider.center = bCollider.center;
		}
		else
		{
			GameObject.DestroyImmediate(to);
			throw new NotSupportedException("No need to rotate head!");
		}

		Undo.DestroyObjectImmediate(oldCollider);
	}
		
		
		
}
