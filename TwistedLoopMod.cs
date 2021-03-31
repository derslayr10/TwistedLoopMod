using BepInEx;
using RoR2;
using UnityEngine;

namespace TwistedLoopMod
{
    [BepInPlugin("com.Derslayr.TwistedLoopMod", "TwistedLoopMod", "2.1.2")]
    public class TwistedLoopMod : BaseUnityPlugin
    {

        //create config options
        //create Portal Spawning Config
        public static int PortalTypeControl { get; set; }
        //create Fade Out multiplier
        public static float FadeOutMultiplier { get; set; }

        //method for running configs
        private void RunConfig() {

            //bind config to make file
            PortalTypeControl = Config.Bind<int>("PortalTypeControl", "portalType", 0, "Controls the portal type spawned in A Moment, Whole. 0 = Bazaar, 1 = Celestial, 2 = Gold, 3 = Null, 4 = All. Default: 0").Value;

            FadeOutMultiplier = Config.Bind<float>("FadeOutMultiplier", "fadeOutMultiplier", 3.0f, "Controls the value the FadeOut duration after the Scavenger fight is multiplied by. This is to allow revived hosts time to activate the portal. Default Value: 3.0").Value;

            Logger.LogMessage("The config for TwistedLoopMod is loaded!");

        }

        //method to revive dead players after Lunar Scavenger fight
        private void ReviveDeadPlayers() {
            
            //get list of all players
            var players = RoR2.PlayerCharacterMasterController.instances;

            //check if there are dead players
            if (RoR2.Run.instance.livingPlayerCount < RoR2.Run.instance.participatingPlayerCount) {

                //iterate through the player list of players and revive all that are dead
                for (int i = 0; i < RoR2.Run.instance.participatingPlayerCount; i++) {

                    if (players[i].master.IsDeadAndOutOfLivesServer()) {

                        players[i].master.RespawnExtraLife();
                    
                    }
                
                }
            
            }
        
        }

        //Method for spawning portal/handling where to place portal
        private void SpawnPortal(){

            //check for if AllPortals option is enabled, if not then decide which portal
            if (PortalTypeControl != 4)
            {

                //setting position of portal spawn to center of arena on the floor
                Vector3 portalPosition = new Vector3(0f, -10f, 0f);

                //create spawn card to use in SpawnRequest
                SpawnCard portalCard = ScriptableObject.CreateInstance<SpawnCard>();

                //Bazaar portal
                if (PortalTypeControl == 0)
                {

                    portalCard.prefab = Resources.Load<GameObject>("prefabs/networkedobjects/PortalShop");

                }

                //Celestial Portal
                else if (PortalTypeControl == 1)
                {

                    portalCard.prefab = Resources.Load<GameObject>("prefabs/networkedobjects/PortalMS");

                }

                //Gold Portal
                else if (PortalTypeControl == 2)
                {

                    portalCard.prefab = Resources.Load<GameObject>("prefabs/networkedobjects/PortalGoldshores");

                }

                //Null Portal
                else if (PortalTypeControl == 3)
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
            else if (PortalTypeControl == 4)
            {

                //create position to be able to spawn portals however
                float portalXPosition = 0f;
                float portalYPosition = -10f;
                float portalZPosition = 0f;

                //loop through to make 4 portals
                for (int i = 0; i < PortalTypeControl; i++)
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

            EntityStates.Missions.LunarScavengerEncounter.FadeOut.duration *= FadeOutMultiplier;

            //event trigger hook for Portal Spawn. Set to beginning of Fade Out transition after killing Twisted Scavenger
            On.EntityStates.Missions.LunarScavengerEncounter.FadeOut.OnEnter += (orig, self) => {

                //do not fuck up the Fade Out
                orig(self);

                //respawn dead players
                ReviveDeadPlayers();

                //run spawn portal method
                SpawnPortal();

            };

        }
    }
}