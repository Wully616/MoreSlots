using System;
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
            moreSlotsDatas = GetMoreSlotDatas();
            moreSlotsHolders = new List<MoreSlotsHolder>(moreSlotsDatas.Count);
            EventManager.onPossess += EventManagerOnPossess;
        }
        
        private void EventManagerOnPossess(Creature creature, EventTime eventTime)
        {
            if (eventTime == EventTime.OnStart) return;
            
            //Lookup the players container and get all of the custom holders
            if ( Player.currentCreature ) {
                //remove old ones
                foreach ( var customHolder in moreSlotsHolders ) {
                    //destroy it
                    if (customHolder) {
                        GameObject.Destroy(customHolder);
                    }
                }
                // add holders to the player
                moreSlotsHolders.Clear();
                foreach ( MoreSlotsData customHolder in moreSlotsDatas ) {
                    Debug.Log($"Attempting to add customHolderData:{customHolder}");
                    CreateHolder(customHolder);
                }

            }
        }
        
        private void CreateHolder(MoreSlotsData moreSlotsData)
        {
            RagdollPart part = Player.currentCreature.ragdoll.GetPartByName(moreSlotsData.ragdollPartName);
            HolderData data = Catalog.GetData<HolderData>(moreSlotsData.holderDataId, true);
            if (!(part is null) && data != null) {
				
                // create a new object to hold the holder.
                GameObject holderGameObject = new GameObject($"{moreSlotsData.id}-holder");
				
                //parent our holderGameobject under the ragdollpart
                holderGameObject.transform.parent = part.transform;
                //set its position
                holderGameObject.transform.localPosition = moreSlotsData.localPosition;
                holderGameObject.transform.localEulerAngles = moreSlotsData.localRotation;
                //add the holder
                MoreSlotsHolder holder = holderGameObject.AddComponent<MoreSlotsHolder>();
                moreSlotsHolders.Add(holder);
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
        
        public static List<MoreSlotsData> GetMoreSlotDatas()
        {
            return Catalog.GetDataList<MoreSlotsData>();
        }
        
    }
}
