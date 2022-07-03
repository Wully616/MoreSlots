﻿using System;
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
		public bool scaleToPlayer = false;

		public override string ToString()
		{
			return $"{nameof(holderDataId)}: {holderDataId}, {nameof(handSide)}: {handSide}, {nameof(localPosition)}: {localPosition}, {nameof(localRotation)}: {localRotation}, {nameof(ragdollPartName)}: {ragdollPartName}, {nameof(scaleToPlayer)}: {scaleToPlayer}";
		}
	}
}