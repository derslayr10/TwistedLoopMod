using BepInEx;
using BepInEx.Configuration;
using RoR2;
using UnityEngine;

namespace TwistedLoopMod
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.Derslayr.TwistedLoopMod", "TwistedLoopMod", "2.0.1")]
    public class TwistedLoopMod : BaseUnityPlugin
    {

        //create config options
        public static int portalTypeControl { get; set; }

        //method for running configs
        private void RunConfig() {

            //bind config to make file
            portalTypeControl = Config.Bind<int>("PortalTypeControl", "portalType", 0, "Controls the portal type spawned in A Moment, Whole. 0 = Bazaar, 1 = Celestial, 2 = Gold, 3 = Null, 4 = All. Default: 0").Value;
        
        }

        //Method for spawning portal/handling where to place portal
        private void SpawnPortal(){

            //check for if AllPortals option is enabled, if not then decide which portal
            if (portalTypeControl != 4)
            {

                //setting position of portal spawn to center of arena on the floor
                Vector3 portalPosition = new Vector3(0f, -10f, 0f);

                //create spawn card to use in SpawnRequest
                SpawnCard portalCard = ScriptableObject.CreateInstance<SpawnCard>();

                //Bazaar portal
                if (portalTypeControl == 0)
                {

                    portalCard.prefab = Resources.Load<GameObject>("prefabs/networkedobjects/PortalShop");

                }

                //Celestial Portal
                else if (portalTypeControl == 1)
                {

                    portalCard.prefab = Resources.Load<GameObject>("prefabs/networkedobjects/PortalMS");

                }

                //Gold Portal
                else if (portalTypeControl == 2)
                {

                    portalCard.prefab = Resources.Load<GameObject>("prefabs/networkedobjects/PortalGoldshores");

                }

                //Null Portal
                else if (portalTypeControl == 3)
                {

                    portalCard.prefab = Resources.Load<GameObject>("prefabs/networkedobjects/PortalArena");

                }

                //error logging
                else
                {

                    Logger.LogWarning("CONFIG ERROR FOR TWISTEDLOOPMOD! CONFIG CANNOT LOAD THIS VALUE!");

                }

                //create the object of the Portal and try to spawn it using DirectorCore instance
                GameObject portalObject = DirectorCore.instance.TrySpawnObject(new DirectorSpawnRequest(portalCard, new DirectorPlacementRule
                {

                    //set placement mode of the Portal Spawner
                    placementMode = DirectorPlacementRule.PlacementMode.Direct,
                    //set the actual position of the portal
                    position = portalPosition,
                    //set the distance the portal is allowed to spawn away from specified location
                    minDistance = 10,
                    maxDistance = 15

                }, RoR2Application.rng));

                //tell that the portal has spawned at location
                Chat.AddMessage("Portal has spawned at: " + portalPosition.ToString());

            }

            //if AllPortals active, handle it
            else if (portalTypeControl == 4)
            {

                //create position to be able to spawn portals however
                float portalXPosition = 0f;
                float portalYPosition = -10f;
                float portalZPosition = 0f;

                //loop through to make 4 portals
                for (int i = 0; i < portalTypeControl; i++)
                {

                    //create position for the portal
                    Vector3 portalPosition = new Vector3(portalXPosition, portalYPosition, portalZPosition);

                    //create spawn card to use in SpawnRequest
                    SpawnCard portalCard = ScriptableObject.CreateInstance<SpawnCard>();

                    //Bazaar portal
                    if (i == 0)
                    {

                        portalCard.prefab = Resources.Load<GameObject>("prefabs/networkedobjects/PortalShop");

                    }

                    //Celestial Portal
                    else if (i == 1)
                    {

                        portalCard.prefab = Resources.Load<GameObject>("prefabs/networkedobjects/PortalMS");

                    }

                    //Gold Portal
                    else if (i == 2)
                    {

                        portalCard.prefab = Resources.Load<GameObject>("prefabs/networkedobjects/PortalGoldshores");

                    }

                    //Null Portal
                    else if (i == 3)
                    {

                        portalCard.prefab = Resources.Load<GameObject>("prefabs/networkedobjects/PortalArena");

                    }

                    //error logging
                    else
                    {

                        Logger.LogWarning("CONFIG ERROR FOR TWISTEDLOOPMOD! CONFIG CANNOT LOAD THIS VALUE!");

                    }

                    //create the object of the Portal and try to spawn it using DirectorCore instance
                    GameObject portalObject = DirectorCore.instance.TrySpawnObject(new DirectorSpawnRequest(portalCard, new DirectorPlacementRule
                    {

                        //set placement mode of the Portal Spawner
                        placementMode = DirectorPlacementRule.PlacementMode.Direct,
                        //set the actual position of the portal
                        position = portalPosition,
                        //set the distance the portal is allowed to spawn away from specified location
                        minDistance = 10,
                        maxDistance = 15

                    }, RoR2Application.rng));

                    //tell that the portal has spawned at location
                    Chat.AddMessage("Portal has spawned at: " + portalPosition.ToString());

                    //change x position on portals
                    portalXPosition += 10f;

                }
            }
        }

        //Method run on game launch
        public void Awake()
        {
            //debugging message to ensure mod loading
            Logger.LogMessage("Loaded TwistedLoopMod mod by Derslayr!");

            //load mod configs
            RunConfig();
            Logger.LogMessage("The config for TwistedLoopMod is loaded!");

            //event trigger hook for Portal Spawn. Set to beginning of Fade Out transition after killing Twisted Scavenger
            On.EntityStates.Missions.LunarScavengerEncounter.FadeOut.OnEnter += (orig, self) => {

                //do not fuck up the Fade Out
                orig(self);

                //run spawn portal method
                SpawnPortal();

            };

        }
    }
}