let drawScaleform = false;
let scaleForm = null;

API.onKeyDown.connect(function (sender, e) {
    if (e.KeyCode === Keys.E && API.hasEntityData(API.getLocalPlayer(), "DoorsTarget")) {
        API.triggerServerEvent("PassDoors");
    }
    else if (e.KeyCode === Keys.K && API.hasEntityData(API.getLocalPlayer(), "DoorsTarget")) {
        API.triggerServerEvent("KnockDoors");
    }
});

API.onServerEventTrigger.connect(function (eventName, args) {
    if (eventName == "DrawBuildingComponents") {
        //TODO: wyświetlanie panelu budynku
        scaleForm = API.requestScaleform('instructional_buttons');
        scaleForm.CallFunction('SET_DATA_SLOT_EMPTY');
        scaleForm.CallFunction('SET_DATA_SLOT', 0, 'E', 'Wejdź');
        scaleForm.CallFunction('SET_DATA_SLOT', 1, 'K', 'Zapukaj');
        scaleForm.CallFunction('DRAW_INSTRUCTIONAL_BUTTONS', -1);
        drawScaleform = true;
    }
    else if (eventName == "DisposeBuildingComponents") {
        drawScaleform = false;
    }
});

API.onUpdate.connect(function () {
    if (drawScaleform) API.renderScaleform(scaleForm, 0, 0, 1280, 720);
});