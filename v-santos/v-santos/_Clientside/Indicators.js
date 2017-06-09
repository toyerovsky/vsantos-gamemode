/*
	By Xenius | 2016.12.29.
 */
var spamProtection;
var player = API.getLocalPlayer();

function getEntityData(entity, data){
	if (API.hasEntitySyncedData(entity, data)) {
		return API.getEntitySyncedData(entity, data);
	} else {
		return false;
	}
}

function setVehicleIndicatorState(ent, indicator, state){
	if (API.isVehicle(ent)) {
		API.callNative("0xB5D45264751B7DF0", ent, indicator, state);
	}
}

API.onKeyDown.connect(function (sender, key) {
	if (API.isPlayerInAnyVehicle(player) && API.getPlayerVehicleSeat(player) == -1) {
		if (spamProtection != true) {
			if (key.KeyCode === Keys.Left) {
				API.triggerServerEvent("toggle_indicator_left");
				spamProtection = true;
				API.sleep(1000);
				spamProtection = false;
			} else if (key.KeyCode === Keys.Right) {
				API.triggerServerEvent("toggle_indicator_right");
				spamProtection = true;
				API.sleep(1000);
				spamProtection = false;
			}
		}
	}
});

API.onEntityStreamIn.connect(function(entity, entType) {
	if (entType == 1) {
		if (getEntityData(entity, "indicator_left")) {
			setVehicleIndicatorState(entity, 1, true);
		}
		
		if (getEntityData(entity, "indicator_right")) {
			setVehicleIndicatorState(entity, 0, true);
		}
	}
});