var drawVehicleHUD = true;
var drawAnimationHUD = false;
var drawStreetHUD = true;
var currentMoney = null;
var currentSpeed = null;
var currentMilage = null;
var currentFuel = null;
var maxFuel = null;
var res = API.getScreenResolutionMantainRatio();
var dtime = 0;
var lastUpdate = API.getGameTime();
var currentConsumption = 0;
var consumptionMultiplier = null;
var currentRpm = 0;


API.onUpdate.connect(function (sender, args) {
    var player = API.getLocalPlayer();
    var inVeh = API.isPlayerInAnyVehicle(player);
    //Rysowanie nazwy ulicy
    if (drawStreetHUD) 
    {
        var pos = API.getEntityPosition(player);
        var streetname = API.getStreetName(pos);
        var zoneNameLabel = API.getZoneNameLabel(pos);
        var zoneName = API.getZoneName(pos);
        API.drawText(`~y~${streetname}`, (99 * res.Width) / 100, (99 * res.Height) / 100 - 80, 0.5, 255, 255, 255, 255, 1, 2, false, true, 0);
        API.drawText(`${zoneName} (${zoneNameLabel})`, (99 * res.Width) / 100, (99 * res.Height) / 100 - 40, 0.4, 255, 255, 255, 255, 1, 2, false, true, 0);    
    }

    //Rysowanie rzeczy w pojeździe
    if (inVeh)
    {
        var gameTime = API.getGameTime(); // m/s
        var updateDelta = gameTime - lastUpdate;
        lastUpdate = gameTime;
        //updateDelta IMPORTANT!

        var veh = API.getPlayerVehicle(player);

        if (currentMilage == null) {
            currentMilage = API.getEntitySyncedData(veh, "_milage");
        } else {
            API.setEntitySyncedData(veh, "_milage", currentMilage);
        }

        if (currentFuel == null) {
            currentFuel = API.getEntitySyncedData(veh, "_fuel");
        } else {
            API.setEntitySyncedData(veh, "_fuel", currentFuel);
        }

        if (maxFuel == null) {
            maxFuel = API.getEntitySyncedData(veh, "_maxfuel");
        }

        var health = API.getVehicleHealth(veh);
        var maxhealth = API.returnNative("GET_ENTITY_MAX_HEALTH", 0, veh);
        var currentHealth = Math.trunc((health / maxhealth) * 100);

        var v = API.getEntityVelocity(veh);
        var speed = Math.sqrt(
            v.X * v.X +
            v.Y * v.Y +
            v.Z * v.Z
        ); // https://pl.wikipedia.org/wiki/Pr%C4%99dko%C5%9B%C4%87#Uk.C5.82ad_wsp.C3.B3.C5.82rz.C4.99dnych_kartezja.C5.84skich

        currentSpeed = speed * 3.6; // m/s to km/h
        currentMilage = Math.max(0, currentMilage + (speed * (updateDelta / 1000))); // speed = m/s, updateDelta = ms

        if (consumptionMultiplier == null) {
            consumptionMultiplier = API.getEntitySyncedData(veh, "_fuelConsumption");
        }

        currentRpm = API.getVehicleRPM(veh);
        currentConsumption = ((currentRpm * consumptionMultiplier) / 100000) * updateDelta;
        currentFuel = Math.max(0, currentFuel - currentConsumption);

        if (currentFuel <= 0 && maxFuel != 0 && API.getVehicleEngineStatus(veh)) {
            //API.callNative("2741540918328977952", veh, 0.5, 0, false); // nie działa
            API.setVehicleEngineStatus(veh, false);
        }

        if (drawVehicleHUD) {
            //PRZEBIEG
            var cMR = Math.trunc(currentMilage)+"";
            var currentMilageR = "~c~" + pad(Math.trunc(cMR / 1000), 6) + "~m~." + pad(cMR.substring(cMR.length - 3, cMR.length - 2), 1) + "~w~";
            API.drawText(`${currentMilageR}km`, (16 * res.Width) / 100, res.Height - 45, 0.5, 255, 255, 255, 255, 7, 0, false, true, 0);
            //PRĘDKOŚC
            API.drawText(`${Math.trunc(currentSpeed)}`, (16 * res.Width) / 100 + 95, res.Height - 110, 1, 255, 255, 255, 255, 4, 2, false, true, 0);
            if (Math.trunc(currentSpeed) < 60) {
                API.drawText(`km/h`, (16 * res.Width) / 100 + 95, res.Height - 95, 0.7, 255, 255, 255, 255, 4, 0, false, true, 0);
            } else if (Math.trunc(currentSpeed) < 140) {
                API.drawText(`~y~km/h`, (16 * res.Width) / 100 + 95, res.Height - 95, 0.7, 255, 255, 255, 255, 4, 0, false, true, 0);
            } else {
                API.drawText(`~r~km/h`, (16 * res.Width) / 100 + 95, res.Height - 95, 0.7, 255, 255, 255, 255, 4, 0, false, true, 0);
            }

            //API.drawText(`RPM:`, (20 * res.Width) / 100, res.Height - 125, 0.5, 255, 255, 255, 255, 4, 2, false, true, 0);
            //if (currentRpm >= 1) {
            //    API.drawText(`${currentRpm}`, (20 * res.Width) / 100, res.Height - 125, 0.5, 219, 46, 46, 255, 4, 0, false, true, 0);
            //} else if (currentRpm >= 0.8) {
            //    API.drawText(`${currentRpm}`, (20 * res.Width) / 100, res.Height - 125, 0.5, 219, 122, 46, 255, 4, 0, false, true, 0);
            //} else {
            //    API.drawText(`${currentRpm}`, (20 * res.Width) / 100, res.Height - 125, 0.5, 255, 255, 255, 255, 4, 0, false, true, 0);
            //}

            //HP POJAZDU
            var currentHealthR = Math.max(0, currentHealth);
            if (currentHealthR < 20) {
                API.drawText(`Pojazd: ~r~${currentHealthR}%`, (16 * res.Width) / 100, res.Height - 155, 0.5, 255, 255, 255, 255, 4, 0, false, true, 0);
            } else if (currentHealthR < 50) {
                API.drawText(`Pojazd: ~y~${currentHealthR}%`, (16 * res.Width) / 100, res.Height - 155, 0.5, 255, 255, 255, 255, 4, 0, false, true, 0);
            } else {
                API.drawText(`Pojazd: ${currentHealthR}%`, (16 * res.Width) / 100, res.Height - 155, 0.5, 255, 255, 255, 255, 4, 0, false, true, 0);
            }

            if (maxFuel != 0) {
                //PALIWO
                var currentFuelR = Math.round(currentFuel * 100) / 100;
                if (currentFuelR < 10) {
                    API.drawText(`Paliwo: ~r~${currentFuelR}~s~/${maxFuel}L`, (16 * res.Width) / 100, res.Height - 185, 0.5, 255, 255, 255, 255, 4, 0, false, true, 0);
                } else if (currentFuelR < 30) {
                    API.drawText(`Paliwo: ~y~${currentFuelR}~s~/${maxFuel}L`, (16 * res.Width) / 100, res.Height - 185, 0.5, 255, 255, 255, 255, 4, 0, false, true, 0);
                } else {
                    API.drawText(`Paliwo: ${currentFuelR}~s~/${maxFuel}L`, (16 * res.Width) / 100, res.Height - 185, 0.5, 255, 255, 255, 255, 4, 0, false, true, 0);
                }
                //API.drawText(`Spalanie:`, (20 * res.Width) / 100, res.Height - 215, 0.5, 255, 255, 255, 255, 4, 2, false, true, 0);
                //var currentConsumptionR = Math.round(currentConsumption * 10000000) / 100;
                //API.drawText(`${currentConsumptionR}L/KM`, (20 * res.Width) / 100, res.Height - 215, 0.5, 255, 255, 255, 255, 4, 0, false, true, 0);
                //if (currentConsumptionR < 60) {
                //    //API.drawText(`${currentConsumptionR}L`, (20 * res.Width) / 100, res.Height - 215, 0.5, 219, 122, 46, 255, 4, 0, false, true, 0);
                //}
                //if (currentConsumptionR < 30) {
                //    //API.drawText(`${currentConsumptionR}L`, (20 * res.Width) / 100, res.Height - 215, 0.5, 219, 46, 46, 255, 4, 0, false, true, 0);
                //}
            }
        }
    }
});

API.onPlayerEnterVehicle.connect(function (veh) {

});

API.onPlayerExitVehicle.connect(function (veh) {
    currentMilage = null;
    currentFuel = null;
    maxFuel = null;
    consumptionMultiplier = null;
});

API.onServerEventTrigger.connect(function (eventName, args) {

    if (eventName == "hide_vehicle_hud") {
        drawVehicleHUD = false;
    }
    else if (eventName == "show_vehicle_hud") {
        drawVehicleHUD = true;
    }
    else if (eventName == "ToggleHud")
    {
        drawStreetHUD = !drawStreetHUD;
    }
});

function pad(num, size) {
    var s = num + "";
    while (s.length < size) s = "0" + s;
    return s;
}
