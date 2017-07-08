
using System.IO;
using System.Collections.Generic;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Constant;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Shared;
using GrandTheftMultiplayer.Shared.Math;
using Newtonsoft.Json;

using Serverside.Core;

namespace Serverside.Autonomic.FuelStation
{
    class RPFuelStation : Script
    {
        private class MapSet
        {
            public Blip Blip { get; set; }
            public TextLabel Tl { get; set; }
            public ColShape Cs { get; set; }
            public Marker Marker { get; set; }
        }

        static string _dataFile = "";
        //List<FuelStation> Data = new List<FuelStation>();
        List<MapSet> fsMapSets = new List<MapSet>();
        //List<MapSet> distMapSets = new List<MapSet>();

        public RPFuelStation()
        {
            API.onResourceStart += API_onResourceStart;
            API.onClientEventTrigger += API_onClientEventTrigger;
        }

        private void API_onResourceStart()
        {
            string dataDir = Path.Combine(API.getResourceFolder(), "Data");
            _dataFile = Path.Combine(dataDir, "FuelStations.json");
            if (!Directory.Exists(dataDir))
            {
                Directory.CreateDirectory(dataDir);
            }

            if (!File.Exists(_dataFile))
            {
                File.Create(_dataFile).Dispose();
                //Data = new List<FuelStation>();
                saveData();
            }
            loadData();
        }

        private void API_onClientEventTrigger(Client sender, string eventName, params object[] arguments)
        {
            //throw new NotImplementedException();
        }

        public void saveData()
        {
            string json = JsonConvert.SerializeObject(RPEntityManager.GetFuelStations(), Formatting.Indented);
            using (StreamWriter oF = new StreamWriter(_dataFile))
            {
                oF.WriteLine(json);
                oF.Close();
            }
        }

        public void loadData()
        {
            //FuelStation s = new FuelStation();
            using (StreamReader sr = new StreamReader(_dataFile))
            {
                List<FuelStation> Data = JsonConvert.DeserializeObject<List<FuelStation>>(sr.ReadToEnd());
                foreach (FuelStation item in Data)
                {
                    RPEntityManager.Add(item);
                }
                sr.Close();
            }
        }

        private void setFuelStationAvailable(FuelStation fs, bool state)
        {
            RPEntityManager.GetFuelStation(fs).Enabled = state;
        }

        private void setFuelStationAvailable(int id, bool state)
        {
            RPEntityManager.GetFuelStation(id).Enabled = state;
        }

        private void setPoints()
        {
            foreach (FuelStation fs in RPEntityManager.GetFuelStations())
            {
                prepareFuelStationMarkers(fs);
            }
        }

        private void prepareFuelStationMarkers(FuelStation fs)
        {
            MapSet fsMapSet = new MapSet();
            fsMapSet.Blip = API.createBlip(fs.MarkerPostition, 0, 0);
            API.setBlipSprite(fsMapSet.Blip, 11);
            API.setBlipName(fsMapSet.Blip, fs.Name);

            fsMapSet.Marker = API.createMarker(2, fs.MarkerPostition - new Vector3(0, 0, 1f), new Vector3(), new Vector3(), new Vector3(1f, 1f, 1f), 30, fs.Color.red, fs.Color.green, fs.Color.blue);
            fsMapSet.Tl = API.createTextLabel("~b~[" + fs.Name + "]", fs.MarkerPostition + new Vector3(0.0f, 0.0f, 0.5f), 10.0f, 0.2f);
            fsMapSet.Cs = API.createCylinderColShape(fs.MarkerPostition, 1f, 5f);

            fsMapSet.Cs.setData("COL_FS", fs.Id);
            fsMapSets.Add(fsMapSet);

            foreach (FuelStation.FuelDistributor dist in fs.Distributors)
            {
                MapSet distMapSet = new MapSet();
                distMapSet.Blip = API.createBlip(dist.MarkerPostition, 0, 0);
                API.setBlipSprite(distMapSet.Blip, 415);
                API.setBlipName(distMapSet.Blip, "");

                distMapSet.Marker = API.createMarker(23, dist.MarkerPostition - new Vector3(0, 0, 1f), new Vector3(), new Vector3(), new Vector3(3f, 3f, 3f), 30, 255, 255, 255);
                //TextLabel textLabel = API.createTextLabel("~b~[" + fs.Name + "]", fs.MarkerPostition + new Vector3(0.0f, 0.0f, 0.5f), 50.0f, 1f);
                distMapSet.Tl = null;
                distMapSet.Cs = API.createCylinderColShape(dist.MarkerPostition, 3f, 5f);

                distMapSet.Cs.setData("COL_DIST", fs.Id);
                fsMapSets.Add(distMapSet);
            }
        }

        private void prepareEvents()
        {
            foreach (MapSet mapset in fsMapSets)
            {
                mapset.Cs.onEntityEnterColShape += (s, e) =>
                {
                    var idfs = s.getData("COL_FS");
                    var idd = s.getData("COL_DIST");
                    if (API.getEntityType(e) == EntityType.Player)
                    {
                        if (idfs != null)
                        {
                            FuelStation fs = RPEntityManager.GetFuelStation(idfs);
                            API.sendNotificationToPlayer(API.getPlayerFromHandle(e), "Witaj w " + fs.Name.Replace('_', ' ') + " !");
                        }
                    }

                    if (API.getEntityType(e) == EntityType.Vehicle)
                    {
                        mapset.Marker.color = new Color(255, 0, 0, 30);
                        if (idd != null)
                        {
                            FuelStation fs = RPEntityManager.GetFuelStation(idd);
                            API.sendNotificationToPlayer(API.getEntityFromHandle<Vehicle>(e).occupants[0], "To jest dystrybutor stacji " + fs.Name.Replace('_', ' ') + " !");
                        }
                    }
                };

                mapset.Cs.onEntityExitColShape += (s, e) =>
                {
                    var idfs = s.getData("COL_FS");
                    var idd = s.getData("COL_DIST");
                    if (API.getEntityType(e) == EntityType.Player)
                    {
                        if (idfs != null)
                        {
                            FuelStation fs = RPEntityManager.GetFuelStation(idfs);
                            API.sendNotificationToPlayer(API.getPlayerFromHandle(e), "Dziekujemy za wizyte w " + fs.Name.Replace('_', ' ') + " !");
                        }
                    }

                    if (API.getEntityType(e) == EntityType.Vehicle)
                    {
                        mapset.Marker.color = new Color(255, 255, 255, 30);
                        if (idd != null)
                        {
                            FuelStation fs = RPEntityManager.GetFuelStation(idd);
                            API.sendNotificationToPlayer(API.getEntityFromHandle<Vehicle>(e).occupants[0], "To jest dystrybutor stacji " + fs.Name.Replace('_', ' ') + " !");
                        }
                    }
                };
            }
        }
        [Command("listfs")]
        public void ListDist(Client sender)
        {
            foreach (FuelStation fs in RPEntityManager.GetFuelStations())
            {
                API.sendChatMessageToPlayer(sender, "ID: " + RPEntityManager.GetFuelStations().IndexOf(fs) + " | Name: " + fs.Name);
            }
        }

        [Command("addfs1")]
        public void AddFuelStation(Client sender, string name)
        {
            FuelStation fs = new FuelStation
            {
                Id = RPEntityManager.GetFuelStations().Count == 0 ? 0 : RPEntityManager.GetFuelStations().Count + 1,
                Name = name,
                MarkerPostition = new Vector3(sender.position.X, sender.position.Y, sender.position.Z + 1),
                Color = new Color(255, 0, 0, 255),
                Distributors = new List<FuelStation.FuelDistributor>(),
                Enabled = true
            };
            RPEntityManager.Add(fs);
        }

        [Command("adddist")]
        public void AddDist(Client sender, string id)
        {
            FuelStation.FuelDistributor dist = new FuelStation.FuelDistributor
            {
                Occupied = false,
                MarkerPostition = new Vector3(sender.position.X, sender.position.Y, sender.position.Z + 0.5f),
            };
            RPEntityManager.GetFuelStation(int.Parse(id)).Distributors.Add(dist);
        }

        [Command("savefs")]
        public void SaveFS(Client sender)
        {
            saveData();
        }

        [Command("delfs")]
        public void DelFS(Client sender, string id)
        {
            RPEntityManager.RemoveFuelStation(int.Parse(id));
        }

        [Command("loadfs")]
        public void LoadFS(Client sender)
        {
            setPoints();
            prepareEvents();
        }
    }
}
