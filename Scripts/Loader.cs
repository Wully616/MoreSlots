using System.Collections.Generic;
using ThunderRoad;
using UnityEngine;

namespace Wully.MoreSlots
{
    public class Loader : CustomData
    {
        
        public bool debug;

        private List<Holder> holders;
        private List<MoreSlotsData> moreSlotsDatas;
        private Dictionary<string, MoreSlotsData> dataLookup;

        public static Loader local;
        
        public override void OnCatalogRefresh()
        {
            //Only want one instance of the loader running
            if (local != null) return;
            local = this;
            
            Debug.Log($"MoreSlots Loader!");
            
            moreSlotsDatas = Catalog.GetDataList<MoreSlotsData>();

            dataLookup = new Dictionary<string, MoreSlotsData>();
            foreach (MoreSlotsData moreSlotsData in moreSlotsDatas)
            {
                dataLookup[moreSlotsData.id] = moreSlotsData;
            }

            holders = new List<Holder>(moreSlotsDatas.Count);
            EventManager.onPossess += EventManagerOnPossess;
        }

        private void EventManagerOnPossess(Creature creature, EventTime eventTime)
        {
            if (eventTime == EventTime.OnStart) return;
            if (!Player.currentCreature) return; // no creature set, return
            DestroySlots();
            OverrideExistingSlots();
            AddSlots();
        }
        
        protected void DestroySlots()
        {
            //remove old ones
            int count = holders.Count;
            for (var i = 0; i < count; i++)
            {
                var customHolder = holders[i];
                //destroy it if its a moreslotsholder

                if (customHolder && customHolder is MoreSlotsHolder)
                {
                    Debug.Log($"Destroying old slot: {customHolder.name}");
                    GameObject.Destroy(customHolder);
                }
            }
        }

        private void OverrideExistingSlots()
        { 
            //Get all the current slots, this should get the base game slots - and possibly other modders ones
            Holder[] currentHolders = Player.currentCreature.gameObject.GetComponentsInChildren<Holder>();
            foreach (var currentHolder in currentHolders)
            {
                // skip holders which may be on items in the players current holders
                if (currentHolder.parentItem) continue; 
                
                //check if the holder is in our data to check if needs to be moved or disabled
                if (dataLookup.TryGetValue(currentHolder.name, out var moreSlotsData))
                {
                    Debug.Log($"Overriding existing holder {currentHolder.name} with MoreSlots configuration {moreSlotsData.ToString()}");
                    //found data to override this holder with
                    currentHolder.gameObject.SetActive(moreSlotsData.enabled); // enable/disable the slot

                    //override position
                    if(moreSlotsData.localPosition != Vector3.zero) currentHolder.transform.localPosition = moreSlotsData.localPosition;
                    if(moreSlotsData.localRotation != Vector3.zero) currentHolder.transform.localPosition = currentHolder.transform.localEulerAngles = moreSlotsData.localRotation;
                    
                    //remove this holderData from the list, so we dont try to create a duplicate
                    dataLookup.Remove(currentHolder.name);
                    moreSlotsDatas.Remove(moreSlotsData);
                }
                holders.Add(currentHolder);
            }
        }
        
        protected void AddSlots()
        {
            // add holders to the player
            int count = moreSlotsDatas.Count;
            for (var i = 0; i < count; i++)
            {
                MoreSlotsData customHolder = moreSlotsDatas[i];
                if (customHolder.enabled)
                {
                    Debug.Log($"Attempting to add customHolderData:{customHolder}");
                    CreateHolder(customHolder);
                }
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
            GameObject holderGameObject = new GameObject($"{moreSlotsData.id}");

            //parent our holderGameobject under the ragdollpart
            Transform holderTransform = holderGameObject.transform;
            holderTransform.parent = part.transform;

            //set its position
            holderTransform.localPosition = moreSlotsData.localPosition;
            holderTransform.localEulerAngles = moreSlotsData.localRotation;

            //add the holder
            MoreSlotsHolder holder = holderGameObject.AddComponent<MoreSlotsHolder>();

            //Add it to our list so we can clean it up later
            holders.Add(holder);

            //not really needed since were not doing custom stuff, but useful to have references to the data which created this holder on the holder itself
            holder.moreSlotsData = moreSlotsData;
            holder.part = part;

            //setup the touch collider
            (holder.touchCollider as SphereCollider).radius = moreSlotsData.triggerColliderRadius;

            //Add holderdata
            holder.ignoredColliders = new List<Collider>();
            holder.Load(data);

            //Override player hand touching
            holder.data.forceAllowTouchOnPlayer = false;
            holder.data.disableTouch = false;
            holder.allowedHandSide = moreSlotsData.handSide;

            holder.RefreshChildAndParentHolder();
            Debug.Log($"Added customHolderData:{moreSlotsData.id}");
        }
    }
}
