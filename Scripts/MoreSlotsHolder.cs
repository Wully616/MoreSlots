using ThunderRoad;
using UnityEngine;

namespace Wully.MoreSlots
{
    public class MoreSlotsHolder : Holder
    {
        public MoreSlotsData moreSlotsData;
        public RagdollPart part;
        protected override void Awake()
        {
            if (Loader.local.debug)
            {
                SetupDebug();
            }
            //set this holders container to the creatures
            linkedContainer = Player.local.creature.container;
            //manually call content load to populate this holder with contents of the container if appropriate
            OnLinkedContainerContentLoad();
            base.Awake();
        }

        //Override the Managed Loops getter to get its value from our Define method
        //This is evaluated during OnEnable/OnDisable
        protected override ManagedLoops ManagedLoops => DefineManagedLoops();

        private ManagedLoops DefineManagedLoops()
        {
            //Mod checks if debug is enabled and enables the update loop
            if (Loader.local.debug)
            {
                return ManagedLoops.Update;
            }
            return 0;
        }

        private LineRenderer lineRendererX;
        private LineRenderer lineRendererY;
        private LineRenderer lineRendererZ;
        protected override void ManagedUpdate()
        {
            if (!moreSlotsData.enabled) return;
            
            //update stuff happens here only when debug enabled
            var pos = this.gameObject.transform.position;
            lineRendererX.SetPosition(0, pos);
            lineRendererX.SetPosition(1, pos + (this.gameObject.transform.right * 0.2f));
            
            lineRendererY.SetPosition(0, pos);
            lineRendererY.SetPosition(1, pos + (this.gameObject.transform.up * 0.2f));
            
            lineRendererZ.SetPosition(0, pos);
            lineRendererZ.SetPosition(1, pos + (this.gameObject.transform.forward * 0.2f));
        }

        private void SetupDebug()
        {
            lineRendererX = CreateLineRenderer();
            lineRendererX.startColor = Color.red;
            lineRendererX.endColor = Color.red;
            lineRendererY = CreateLineRenderer();
            lineRendererY.startColor = Color.green;
            lineRendererY.endColor = Color.green;
            lineRendererZ = CreateLineRenderer();
            lineRendererZ.startColor = Color.blue;
            lineRendererZ.endColor = Color.blue;
        }

        private LineRenderer CreateLineRenderer()
        {
            var go = new GameObject("line");
            go.transform.parent = this.transform;
            var lineRenderer = go.AddComponent<LineRenderer>();
            lineRenderer.startWidth = 0.01f;
            lineRenderer.endWidth = 0.01f;
            lineRenderer.textureMode = LineTextureMode.Tile;
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            return lineRenderer;
        }
    }
}
