using BepInEx;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace TwistedLoopMod
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.Derslayr.TwistedLoopMod", "TwistedLoopMod", "1.1.1")]
    public class TwistedLoopMod : BaseUnityPlugin
    {

        //Method for spawning portal/handling where to place portal
        private void SpawnPortal(){

            //setting position of portal spawn to center of arena on the floor
            Vector3 portalPosition = new Vector3 (0f,-10f,0f);

            //create the object of the Portal and try to spawn it using DirectorCore instance
            GameObject portalObject = DirectorCore.instance.TrySpawnObject(new DirectorSpawnRequest(Resources.Load<SpawnCard>("spawncards/interactablespawncard/iscShopPortal"), new DirectorPlacementRule
            {
                
                //set placement mode of the Portal Spawner
                placementMode = DirectorPlacementRule.PlacementMode.Direct,
                //set the actual position of the portal
                position = portalPosition,
                //set the distance the portal is allowed to spawn away from specified location
                minDistance = 10,
                maxDistance = 15

            }, RoR2Application.rng));

        }

        //Method run on game launch
        public void Awake()
        {
            //debugging message to ensure mod loading
            Logger.LogMessage("Loaded TwistedLoopMod mod by Derslayr!");

            //event trigger hook for Portal Spawn. Set to beginning of Fade Out transition after killing Twisted Scavenger
            On.EntityStates.Missions.LunarScavengerEncounter.FadeOut.OnEnter += (orig, self) => {

                //do not fuck up the Fade Out
                orig(self);

                //run spawn portal method
                SpawnPortal();

                //debugging message to run after portal has spawned
                Chat.AddMessage("Blue Portal has spawned in the field!");

            };

        }
    }
}