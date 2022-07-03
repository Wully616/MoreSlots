using System.Collections.Generic;
using ThunderRoad;
using UnityEngine;

namespace Wully.MoreSlots
{
    public class Loader : CustomData
    {
        private List<MoreSlotsHolder> moreSlotsHolders;
        private List<MoreSlotsData> moreSlotsDatas;

        public override void OnCatalogRefresh()
        {
            Debug.Log($"MoreSlots Loader!");
            moreSlotsDatas = Catalog.GetDataList<MoreSlotsData>();
            moreSlotsHolders = new List<MoreSlotsHolder>(moreSlotsDatas.Count);
            EventManager.onPossess += EventManagerOnPossess;
        }

        private void EventManagerOnPossess(Creature creature, EventTime eventTime)
        {
            if (eventTime == EventTime.OnStart) return;
            if (!Player.currentCreature) return; // no creature set, return
            DestroySlots();
            AddSlots();

        }

        protected void AddSlots()
        {
            // add holders to the player
            moreSlotsHolders.Clear();
            int count = moreSlotsDatas.Count;
            for (var i = 0; i < count; i++)
            {
                MoreSlotsData customHolder = moreSlotsDatas[i];
                Debug.Log($"Attempting to add customHolderData:{customHolder}");
                CreateHolder(customHolder);
            }
        }

        protected void DestroySlots()
        {
            //remove old ones
            int count = moreSlotsHolders.Count;
            for (var i = 0; i < count; i++)
            {
                var customHolder = moreSlotsHolders[i];
                //destroy it
                if (customHolder) GameObject.Destroy(customHolder);
            }
        }

        private void CreateHolder(MoreSlotsData moreSlotsData)
        {
            RagdollPart part = Player.currentCreature.ragdoll.GetPartByName(moreSlotsData.ragdollPartName);
            HolderData data = Catalog.GetData<HolderData>(moreSlotsData.holderDataId, true);
            if (part is null || data == null)
            {
                Debug.LogWarning($"Could not create holder for {moreSlotsData.id}");
                return;
            }

            // create a new object to hold the holder.
            GameObject holderGameObject = new GameObject($"{moreSlotsData.id}-holder");

            //parent our holderGameobject under the ragdollpart
            Transform holderTransform = holderGameObject.transform;
            holderTransform.parent = part.transform;

            //set its position
            holderTransform.localPosition = moreSlotsData.localPosition;
            holderTransform.localEulerAngles = moreSlotsData.localRotation;

            //add the holder
            MoreSlotsHolder holder = holderGameObject.AddComponent<MoreSlotsHolder>();

            //Add it to our list so we can clean it up later
            moreSlotsHolders.Add(holder);

            //not really needed since were not doing custom stuff, but useful to have references to the data which created this holder on the holder itself
            holder.moreSlotsData = moreSlotsData;
            holder.part = part;

            //add the touch collider
            SphereCollider sphereCollider = holderGameObject.AddComponent<SphereCollider>();
            sphereCollider.radius = 0.15f;
            sphereCollider.isTrigger = true;
            holder.touchCollider = sphereCollider;

            //Add holderdata
            holder.ignoredColliders = new List<Collider>();
            holder.Load(data);

            holder.allowedHandSide = moreSlotsData.handSide;

            holder.RefreshChildAndParentHolder();
            Debug.Log($"Added customHolderData:{moreSlotsData.id}");
        }
    }
}
