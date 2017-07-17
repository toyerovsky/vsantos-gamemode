/* Copyright (C) Przemysław Postrach - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Przemysław Postrach <toyerek@gmail.com> July 2017
 */

var menuPool = null;
let menu = null;

class DriveThruItem
{
    constructor(name, hp, cost)
    {
        this.name = name;
        this.hp = hp;
        this.cost = cost;
    }
}

let driveThruItems = [
    new DriveThruItem("Hamburger", 50, 20), new DriveThruItem("HotDog", 40, 15), new DriveThruItem("Frytki", 30, 10)
];

API.onServerEventTrigger.connect((eventName, args) => {
    if (eventName == "ShowDriveThruMenu") {

        menuPool = API.getMenuPool();
        menu = API.createMenu("Drive-Thru", 0, 0, 6);

        for (var i = 0; i < driveThruItems.length; i++) {
            var nameItem = API.createMenuItem(driveThruItems[i].name, `Koszt: $${driveThruItems[i].cost}`);
            menu.AddItem(nameItem);
        }

        menu.OnItemSelect.connect((sender, item, index) => {
            API.triggerServerEvent("OnPlayerDriveThruBought",
                driveThruItems[index].name,
                driveThruItems[index].hp,
                driveThruItems[index].cost);
            menu.Visible = false;
        });

        menuPool.Add(menu);
        menu.Visible = true;
    }
    else if (eventName == "DisposeDriveThruMenu") {
        menu.Visible = false;
    }
});

API.onUpdate.connect(function () {
    if (menuPool != null) {
        menuPool.ProcessMenus();
    }
});
