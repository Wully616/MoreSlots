using System;
using ThunderRoad;
using UnityEngine;

namespace Wully.MoreSlots
{

	[Serializable]
	public class MoreSlotsData : CustomData {
		public string holderDataId;
		public Interactable.HandSide handSide = Interactable.HandSide.Both;
		public Vector3 localPosition;
		public Vector3 localRotation;
		public string ragdollPartName;
		public bool enableCollisions = false;
		public bool ignoreCollisionsWithPlayer = false;
		public bool ignoreCollisionsWithPlayerHands = false;
		public bool ignoreCollisionsWithEquippedItems = false;
		public bool physicsParent = false;
		public bool scaleToPlayer = false;
		public string rotateRelativeTo;
		public Dir holderDir;
		public Dir relativeToDir;
		public Dir relativePartAxis;

		public override string ToString() {
			return $"{nameof(holderDataId)}: {holderDataId}, {nameof(handSide)}: {handSide}, {nameof(localPosition)}: {localPosition}, {nameof(localRotation)}: {localRotation}, {nameof(ragdollPartName)}: {ragdollPartName}, {nameof(enableCollisions)}: {enableCollisions}, {nameof(ignoreCollisionsWithPlayer)}: {ignoreCollisionsWithPlayer}, {nameof(ignoreCollisionsWithPlayerHands)}: {ignoreCollisionsWithPlayerHands}, {nameof(ignoreCollisionsWithEquippedItems)}: {ignoreCollisionsWithEquippedItems}, {nameof(physicsParent)}: {physicsParent}, {nameof(scaleToPlayer)}: {scaleToPlayer}";
		}

		public enum Dir {
			up,down,left,right,forward,back

		}
	}
}