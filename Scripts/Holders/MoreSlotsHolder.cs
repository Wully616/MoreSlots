using ThunderRoad;
using UnityEngine;
using Wully.Data;

namespace MoreSlots.Scripts.Holders
{
    public class MoreSlotsHolder : Holder
    {
        public MoreSlotsData moreSlotsData;
        public RagdollPart part;
        protected override void Awake()
        {
            //Get the creatures inventory container
            Debug.Log($"MoreSlotsHolder Awake called");
            //set this holders container to the creatures
            linkedContainer = Player.local.creature.container;
            //manually call content load to populate this holder with contents of the container if appropriate
            OnLinkedContainerContentLoad();
        }
        
    }
}
