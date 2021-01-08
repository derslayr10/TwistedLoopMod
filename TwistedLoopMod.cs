using BepInEx;
using RoR2;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using static RoR2.Navigation.MapNodeGroup;

namespace TwistedLoopMod
{
    [BepInDependency("com.bepis.r2api")]
    //Change these
    [BepInPlugin("com.Derslayr.TwistedLoopMod", "TwistedLoopMod", "1.1.0")]
    public class TwistedLoopMod : BaseUnityPlugin
    {
        //Method for spawning portal/handling where to place portal
        private void SpawnPortal(Vector3 portalPosition){

            //create SpawnCard for Blue Portal
            SpawnCard portalSpawn = ScriptableObject.CreateInstance<SpawnCard>();
            portalSpawn.prefab = Resources.Load<GameObject>("prefabs/networkedobjects/PortalShop");

            //create the object of the Portal and try to spawn it using DirectorCore instance
            GameObject portalObject = DirectorCore.instance.TrySpawnObject(new DirectorSpawnRequest(portalSpawn, new DirectorPlacementRule
            {
                
                //set placement mode of the Portal Spawner
                placementMode = DirectorPlacementRule.PlacementMode.Approximate,
                //set the actual position of the portal from the player
                position = portalPosition,
                //set the distance the portal is allowed to spawn away from the portal
                minDistance = 30,
                maxDistance = 60

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

                //Original method of spawning portal. Works but is inconsistent.
                //GameObject gameObject = Resources.Load<GameObject>("prefabs/networkedobjects/PortalShop");
                //UnityEngine.Object.Instantiate<GameObject>(gameObject);

                //Determine whether server is active
                if (NetworkServer.active) {

                    //create a "player" that is equal to the Host of the session
                    var tempPlayer = LocalUserManager.GetFirstLocalUser();

                    //verify that tempPlayer is not empty
                    if (tempPlayer != null) {

                        //make sure tempPlayer has a CharacterBody
                        if (tempPlayer.cachedBody) {

                            //take the corePosition from tempPlayer's CharacterBody and spawn portal at that location
                            SpawnPortal(tempPlayer.cachedBody.corePosition);

                            //debugging message to run after portal has spawned
                            Chat.AddMessage("Blue Portal has spawned in the field!");

                        }
                    
                    }

                }

            };

        }
    }
}