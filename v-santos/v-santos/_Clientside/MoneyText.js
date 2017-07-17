/* Copyright (C) Przemys³aw Postrach - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Przemys³aw Postrach <toyerek@gmail.com> July 2017
 */

var drawMoney = false;
var moneyString = "";

var drawMoneyInfo = false;
var moneyInfo = "";
var color = null;

var resX = API.getScreenResolutionMaintainRatio().Width;
var resY = API.getScreenResolutionMaintainRatio().Height;

var lastMoney;
var currentMoney;
var timeToStop = null;

function replaceAt(string, index, char) {
    var a = string.split("");
    a[index] = char;
    return a.join("");
}

class RGBColor {
    constructor(r, g, b) {
        this.r = r;
        this.g = g;
        this.b = b;
    }
}

API.onServerEventTrigger.connect((eventName, args) => {
    if (eventName === "MoneyTextVisibilityChanged") {
        drawMoney = args[0];
    }
    else if (eventName === "MoneyChanged")
    {
        if (moneyString == "") {
            moneyString = `$${args[0].toString()}`;
            drawMoney = true;
            return;
        }

        lastMoney = parseFloat(moneyString.substring(1));
        currentMoney = parseFloat(args[0]);

        //strata gotówki
        if (currentMoney - lastMoney < 0)
        {
            moneyInfo = `-$${Math.abs(currentMoney - lastMoney)}`;
            color = new RGBColor(195, 51, 51);
        }
        //przyp³yw gotówki
        else if (currentMoney - lastMoney > 0) {
            moneyInfo = `+$${currentMoney - lastMoney}`;
            color = new RGBColor(106, 154, 40);
        }

        timeToStop = API.getGlobalTime() + 4000;
        drawMoneyInfo = true;

        moneyString = `$${currentMoney.toString()}`;
    }
});

API.onUpdate.connect(() => {
    if (drawMoney)
    {
        API.drawText(moneyString, resX - 15, 50, 0.60, 255, 255, 255, 255, 7, 2, true, true, 0);
    }
    if (drawMoneyInfo)
    {
        if (timeToStop <= API.getGlobalTime())
        {
            drawMoneyInfo = false;
            return;
        }
        API.drawText(moneyInfo, resX - 10, 85, 0.50, color.r, color.g, color.b, 255, 7, 2, true, true, 0);
    }
});

////Dodaje okreœlon¹ iloœæ spacji na pocz¹tek. 
//function padStart(string, size) {
//    var s = "";
//    for (var i = 0; i < size; i++)
//    {
//        s = s + " ";
//    }
//    return s + string;
//}

////Usuwa okreœlon¹ iloœæ znaków z pocz¹tku.
//function popStart(string, size) {
    
//    var s = "";
//    var array = string.split("").reverse();
//    for (var i = 0; i < (string.length - size); i++) {
//        s = s + array[i];
//    }
//    return s;
//}

