/* Copyright (C) Przemys³aw Postrach - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Przemys³aw Postrach <toyerek@gmail.com> July 2017
 */

var menuPool = null;

API.onServerEventTrigger.connect(function (eventName, args)
{
    if (eventName === "OnPlayerCreateCharacter") {
        menuPool = API.getMenuPool();

        var editorMenu = API.createMenu(args[10].toString(), 0, 0, 6);
        API.setMenuTitle(editorMenu, "Kreator postaci");
        API.setMenuBannerRectangle(editorMenu, 100, 106, 154, 40);
        
        var basics = API.createMenu(args[10].toString(), 0, 0, 6);
        API.setMenuTitle(basics, "Rodzice");
        API.setMenuBannerRectangle(basics, 100, 0, 255, 0);

        var accessories = API.createMenu(args[10].toString(), 0, 0, 6);
        API.setMenuTitle(accessories, "Akcesoria");
        API.setMenuBannerRectangle(accessories, 100, 0, 255, 0);

        var advanced = API.createMenu(args[10].toString(), 0, 0, 6);
        API.setMenuTitle(advanced, "Zaawansowane");
        API.setMenuBannerRectangle(advanced, 100, 0, 255, 0);

        var clothes = API.createMenu(args[10].toString(), 0, 0, 6);
        API.setMenuTitle(clothes, "Ubranie");
        API.setMenuBannerRectangle(clothes, 100, 0, 255, 0);

        var mensList = args[0];
        var mensListClient = new List(String);

        var womansList = args[1];
        var womansListClient = new List(String);

        var opacityListClient = new List(String);

        var hairsList = args[2];
        var hairsListClient = new List(String);

        var hairColorsListClient = new List(String);

        var textures = new List(String);
        textures.Add("0");
        textures.Add("1");
        textures.Add("2");
        textures.Add("3");
        textures.Add("4");
        textures.Add("5");
        textures.Add("6");
        textures.Add("7");
        textures.Add("8");
        textures.Add("9");
        textures.Add("10");

        var makeupsListClient = new List(String);

        var makeupColorsListClient = new List(String);

        var eyeBrowsListClient = new List(String);
       
        var lipsticksListClient = new List(String);

        for (var i = 0; i < mensList.Count; i++) {
            mensListClient.Add(mensList[i]);
        }

        for (var j = 0; j < womansList.Count; j++) {
            womansListClient.Add(womansList[j]);
        }

        for (var k = 0.0; k <= 1; k = k + 0.1) {
            opacityListClient.Add(k.toString().substr(0,3));
        }

        for (var l = 0; l < hairsList.Count; l++) {
            hairsListClient.Add(hairsList[l].toString());
        }

        var hairColorsCount = API.returnNative("_GET_NUM_HAIR_COLORS", 0);
        
        for (var m = 0; m < hairColorsCount; m++)
        {
            hairColorsListClient.Add(m.toString());
        }

        var makeups = API.returnNative("_GET_NUM_HEAD_OVERLAY_VALUES", 0, 4);

        for (var n = 0; n < makeups; n++) {
            makeupsListClient.Add(n.toString());
        }

        var makeupColors = API.returnNative("_GET_NUM_MAKEUP_COLORS", 0);

        for (var o = 0; o < makeupColors; o++) {
            makeupColorsListClient.Add(o.toString());
        }

        var eyeBrows = API.returnNative("_GET_NUM_HEAD_OVERLAY_VALUES", 0, 2);

        for (var p = 0; p < eyeBrows; p++) {
            eyeBrowsListClient.Add(p.toString());
        }

        var lipsticks = API.returnNative("_GET_NUM_HEAD_OVERLAY_VALUES", 0, 8);

        for (var r = 0; r < lipsticks; r++) {
            lipsticksListClient.Add(r.toString());
        }

        var feetsList = args[3];
        var feetsListClient = new List(String);

        for (var q = 0; q < feetsList.Count; q++) {
            feetsListClient.Add(feetsList[q].toString());
        }

        var legsList = args[4];
        var legsListClient = new List(String);

        for (var t = 0; t < legsList.Count; t++)
        {
            legsListClient.Add(legsList[t].toString());
        }

        //var topsList = args[5];
        var topsListClient = new List(String);

        for (var v = 0; v < 200; v++) {
            topsListClient.Add(v.toString());
        }

        var hatsList = args[6];
        var hatsListClient = new List(String);

        for (var x = 0; x < hatsList.Count; x++) {
            hatsListClient.Add(hatsList[x].toString());
        }

        var glassesList = args[7];
        var glassesListClient = new List(String);

        for (var i1 = 0; i1 < glassesList.Count; i1++) {
            glassesListClient.Add(glassesList[i1].toString());
        }

        var earsList = args[8];
        var earsListClient = new List(String);

        for (var k1 = 0; k1 < earsList.Count; k1++) {
            earsListClient.Add(earsList[k1].toString());
        }

        var accessoriesList = args[9];
        var accessoriesListClient = new List(String);

        for (var m1 = 0; m1 < accessoriesList.Count; m1++)
        {
            accessoriesListClient.Add(accessoriesList[m1].toString());
        }

        var torsoListClient = new List(String);

        for (var s = 0; s < 25; s++)
        {
            torsoListClient.Add(s.toString());
        }

        var undershirtListClient = new List(String);

        for (var u = 0; u < 80; u++) {

            undershirtListClient.Add(u.toString());
        }


        var basicsItem = API.createMenuItem("Podstawowe", "");
        var advancedItem = API.createMenuItem("Zaawansowane", "");
        var clothesItem = API.createMenuItem("Ubrania", "Wybierz ubrania dla twojej postaci.");
        var accessoriesItem = API.createMenuItem("Akcesoria", "Wybierz akcesoria dla twojej postaci.");

        var fathers = API.createListItem("Ojciec", "Wybierz ojca swojej postaci.", mensListClient, 0);
        var mothers = API.createListItem("Matka", "Wybierz matke swojej postaci.", womansListClient, 0);

        var shape = API.createListItem("Rysy", "Wybierz strone, ktorej rysy twarzy sa dominujace.", opacityListClient, 0);
        var skinColor = API.createListItem("Skora", "Wybierz strone, ktorej kolor skory jest dominujacy.", opacityListClient, 0);

        var hair = API.createListItem("Wlosy", "Wybierz fryzure dla swojej postaci.", hairsListClient, 0);
        var hairTexture = API.createListItem("Wariant", "Wybierz teksture fryzury dla swojej postaci.", textures, 0);
        var hairColor = API.createListItem("Kolor wlosow", "Wybierz kolor fryzury swojej postaci.", hairColorsListClient, 0);

        var makeup = API.createListItem("Makijaz", "Wybierz makijaz dla swojej postaci.", makeupsListClient, 0);
        var firstMakeupColor = API.createListItem("Pierwszy kolor makijazu", "Wybierz pierwszy kolor makijazu dla swojej postaci.", makeupColorsListClient, 0);
        var secondMakeupColor = API.createListItem("Drugi kolor makijazu", "Wybierz drugi kolor makijazu dla swojej postaci.", makeupColorsListClient, 0);
        var makeupOpacity = API.createListItem("Widocznosc makijazu", "Ustal widocznosc makijazu swojej postaci.", opacityListClient, 0);

        var eyebrows = API.createListItem("Cienie", "Wybierz cienie pod oczami dla swojej postaci.", eyeBrowsListClient, 0);
        var firstEyebrowsColor = API.createListItem("Pierwszy kolor cieni", "Wybierz pierwszy kolor cieni dla swojej postaci.", makeupColorsListClient, 0);
        var secondEyebrowsColor = API.createListItem("Drugi kolor cieni", "Wybierz drugi kolor cieni dla swojej postaci.", makeupColorsListClient, 0);
        var eyebrowsOpacity = API.createListItem("Widocznosc cieni", "Ustal widocznosc cieni swojej postaci.", opacityListClient, 0);

        var lipstick = API.createListItem("Szminka", "Wybierz szminke dla swojej postaci.", lipsticksListClient, 0);
        var firstLipstickColor = API.createListItem("Pierwszy kolor szminki", "Wybierz pierwszy kolor szminki dla swojej postaci.", makeupColorsListClient, 0);
        var secondLipstickColor = API.createListItem("Drugi kolor szminki", "Wybierz drugi kolor szminki dla swojej postaci.", makeupColorsListClient, 0);
        var lipstickOpacity = API.createListItem("Widocznosc szminki", "Ustal widocznosc szminki swojej postaci.", opacityListClient, 0);

        var feets = API.createListItem("Buty", "Wybierz buty dla swojej postaci.", hairsListClient, 0);
        var feetsTexture = API.createListItem("Wariant", "Wybierz teksture butow dla swojej postaci.", textures, 0);

        var legs = API.createListItem("Spodnie", "Wybierz spodnie dla swojej postaci.", hairsListClient, 0);
        var legsTexture = API.createListItem("Wariant", "Wybierz teksture spodni dla swojej postaci.", textures, 0);

        var top = API.createListItem("Ubranie", "Wybierz ubranie dla swojej postaci.", hairsListClient, 0);
        var topTexture = API.createListItem("Wariant", "Wybierz ubranie dla swojej postaci.", textures, 0);

        var hat = API.createListItem("Kapelusz", "Wybierz ubranie dla swojej postaci.", hairsListClient, 0);
        var hatTexture = API.createListItem("Wariant", "Wybierz ubranie dla swojej postaci.", textures, 0);

        var glasses = API.createListItem("Okulary", "Wybierz ubranie dla swojej postaci.", hairsListClient, 0);
        var glassesTexture = API.createListItem("Wariant", "Wybierz ubranie dla swojej postaci.", textures, 0);

        var ears = API.createListItem("Uszy", "Wybierz ubranie dla swojej postaci.", hairsListClient, 0);
        var earsTexture = API.createListItem("Wariant", "Wybierz ubranie dla swojej postaci.", textures, 0);

        var accessory = API.createListItem("Akcesoria", "Wybierz ubranie dla swojej postaci.", hairsListClient, 0);
        var accessoryTexture = API.createListItem("Wariant", "Wybierz ubranie dla swojej postaci.", textures, 0);

        var undershirt = API.createListItem("Podkoszulek", "Wybierz podkoszulek dla swojej postaci.", undershirtListClient, 0);
        var undershirtTexture = API.createListItem("Wariant", "Wybierz ubranie dla swojej postaci.", textures, 0);       

        var torso = API.createListItem("Tors", "Wybierz tors dla swojej postaci.", torsoListClient, 0);

        editorMenu.AddItem(basicsItem);
        editorMenu.AddItem(advancedItem);
        editorMenu.AddItem(clothesItem);
        editorMenu.AddItem(accessoriesItem);

        editorMenu.BindMenuToItem(basics, basicsItem);
        editorMenu.BindMenuToItem(advanced, advancedItem);
        editorMenu.BindMenuToItem(clothes, clothesItem);
        editorMenu.BindMenuToItem(accessories, accessoriesItem);

        basics.AddItem(fathers);
        basics.AddItem(mothers);

        basics.AddItem(shape);
        basics.AddItem(skinColor);

        basics.AddItem(hair);
        basics.AddItem(hairTexture);
        basics.AddItem(hairColor);

        advanced.AddItem(makeup);
        advanced.AddItem(firstMakeupColor);
        advanced.AddItem(secondMakeupColor);
        advanced.AddItem(makeupOpacity);

        advanced.AddItem(eyebrows);
        advanced.AddItem(firstEyebrowsColor);
        advanced.AddItem(secondEyebrowsColor);
        advanced.AddItem(eyebrowsOpacity);

        advanced.AddItem(lipstick);
        advanced.AddItem(firstLipstickColor);
        advanced.AddItem(secondLipstickColor);
        advanced.AddItem(lipstickOpacity);

        clothes.AddItem(feets);
        clothes.AddItem(feetsTexture);

        clothes.AddItem(legs);
        clothes.AddItem(legsTexture);

        clothes.AddItem(top);
        clothes.AddItem(topTexture);
        clothes.AddItem(torso);

        clothes.AddItem(undershirt);
        clothes.AddItem(undershirtTexture);

        accessories.AddItem(hat);
        accessories.AddItem(hatTexture);

        accessories.AddItem(glasses);
        accessories.AddItem(glassesTexture);

        accessories.AddItem(ears);
        accessories.AddItem(earsTexture);

        accessories.AddItem(accessory);
        accessories.AddItem(accessoryTexture);

        menuPool.Add(basics);
        menuPool.Add(advanced);
        menuPool.Add(clothes);
        menuPool.Add(accessories);
        menuPool.Add(editorMenu);
        editorMenu.Visible = true;

        fathers.OnListChanged.connect(function (sender, newIndex)
        {
            API.triggerServerEvent("OnCreatorFatherChanged", sender.IndexToItem(sender.Index));
            
        });

        mothers.OnListChanged.connect(function(sender, newIndex)
        {
            API.triggerServerEvent("OnCreatorMotherChanged", sender.IndexToItem(sender.Index));
            
        });

        shape.OnListChanged.connect(function(sender, newIndex) {
            API.triggerServerEvent("OnCreatorShapeMixChanged", sender.IndexToItem(sender.Index).toString());
        });

        skinColor.OnListChanged.connect(function (sender, newIndex)
        {
            API.triggerServerEvent("OnCreatorSkinMixChanged", sender.IndexToItem(sender.Index).toString());
        });

        hair.OnListChanged.connect(function (sender, newIndex)
        {
            API.triggerServerEvent("OnCreatorHairChanged", sender.IndexToItem(sender.Index).toString());
            
        });

        hairTexture.OnListChanged.connect(function (sender, newIndex)
        {
            API.triggerServerEvent("OnCreatorHairTextureChanged", sender.IndexToItem(sender.Index).toString());
            
        });

        hairColor.OnListChanged.connect(function (sender, newIndex)
        {
            API.triggerServerEvent("OnCreatorHairColorChanged", sender.IndexToItem(sender.Index).toString());
            
        });

        makeup.OnListChanged.connect(function (sender, newIndex)
        {
            API.triggerServerEvent("OnCreatorMakeupColorChanged", sender.IndexToItem(sender.Index).toString());
            
        });

        firstMakeupColor.OnListChanged.connect(function (sender, newIndex)
        {
            API.triggerServerEvent("OnCreatorFirstMakeupColorChanged", sender.IndexToItem(sender.Index).toString());
            
        });

        secondMakeupColor.OnListChanged.connect(function (sender, newIndex)
        {
            API.triggerServerEvent("OnCreatorSecondMakeupColorChanged", sender.IndexToItem(sender.Index).toString());
            
        });

        makeupOpacity.OnListChanged.connect(function (sender, newIndex)
        {
            API.triggerServerEvent("OnCreatorMakeupOpacityChanged", sender.IndexToItem(sender.Index).toString());
            
        });

        eyebrows.OnListChanged.connect(function (sender, newIndex)
        {
            API.triggerServerEvent("OnCreatorEyebrowsChanged", sender.IndexToItem(sender.Index).toString());
            
        });

        firstEyebrowsColor.OnListChanged.connect(function (sender, newIndex)
        {
            API.triggerServerEvent("OnCreatorFirstEyebrowsColorChanged", sender.IndexToItem(sender.Index).toString());
            
        });

        secondEyebrowsColor.OnListChanged.connect(function (sender, newIndex)
        {
            API.triggerServerEvent("OnCreatorSecondEyebrowsColorChanged", sender.IndexToItem(sender.Index).toString());
            
        });

        eyebrowsOpacity.OnListChanged.connect(function (sender, newIndex)
        {
            API.triggerServerEvent("OnCreatorEyebrowsOpacityChanged", sender.IndexToItem(sender.Index).toString());
            
        });

        lipstick.OnListChanged.connect(function (sender, newIndex)
        {
            API.triggerServerEvent("OnCreatorLipstickChanged", sender.IndexToItem(sender.Index).toString());
            
        });

        firstLipstickColor.OnListChanged.connect(function (sender, newIndex)
        {
            API.triggerServerEvent("OnCreatorFirstLipstickColorChanged", sender.IndexToItem(sender.Index).toString());
            
        });

        secondLipstickColor.OnListChanged.connect(function (sender, newIndex)
        {
            API.triggerServerEvent("OnCreatorSecondLipstickColorChanged", sender.IndexToItem(sender.Index).toString());
            
        });

        lipstickOpacity.OnListChanged.connect(function (sender, newIndex)
        {
            API.triggerServerEvent("OnCreatorLipstickOpacityChanged", sender.IndexToItem(sender.Index).toString());
            
        });

        feets.OnListChanged.connect(function (sender, newIndex)
        {
            API.triggerServerEvent("OnCreatorFeetsChanged", sender.IndexToItem(sender.Index).toString());
            
        });

        feetsTexture.OnListChanged.connect(function (sender, newIndex)
        {
            API.triggerServerEvent("OnCreatorFeetsTextureChanged", sender.IndexToItem(sender.Index).toString());
            
        });

        legs.OnListChanged.connect(function (sender, newIndex)
        {
            API.triggerServerEvent("OnCreatorLegsChanged", sender.IndexToItem(sender.Index).toString());
            
        });

        legsTexture.OnListChanged.connect(function (sender, newIndex)
        {
            API.triggerServerEvent("OnCreatorLegsTextureChanged", sender.IndexToItem(sender.Index).toString());
            
        });

        top.OnListChanged.connect(function (sender, newIndex)
        {
            API.triggerServerEvent("OnCreatorTopChanged", sender.IndexToItem(sender.Index).toString());
            
        });

        topTexture.OnListChanged.connect(function (sender, newIndex)
        {
            API.triggerServerEvent("OnCreatorTopTextureChanged", sender.IndexToItem(sender.Index).toString());
            
        });

        hat.OnListChanged.connect(function (sender, newIndex)
        {
            API.triggerServerEvent("OnCreatorHatChanged", sender.IndexToItem(sender.Index).toString());
            
        });

        hatTexture.OnListChanged.connect(function (sender, newIndex)
        {
            API.triggerServerEvent("OnCreatorHatTextureChanged", sender.IndexToItem(sender.Index).toString());
            
        });

        glasses.OnListChanged.connect(function (sender, newIndex)
        {
            API.triggerServerEvent("OnCreatorGlassesChanged", sender.IndexToItem(sender.Index).toString());
            
        });

        glassesTexture.OnListChanged.connect(function (sender, newIndex)
        {
            API.triggerServerEvent("OnCreatorGlassesTextureChanged", sender.IndexToItem(sender.Index).toString());
            
        });

        ears.OnListChanged.connect(function (sender, newIndex)
        {
            API.triggerServerEvent("OnCreatorEarsChanged", sender.IndexToItem(sender.Index).toString());
            
        });

        earsTexture.OnListChanged.connect(function (sender, newIndex)
        {
            API.triggerServerEvent("OnCreatorEarsTextureChanged", sender.IndexToItem(sender.Index).toString());
            
        });

        accessory.OnListChanged.connect(function (sender, newIndex)
        {
            API.triggerServerEvent("OnCreatorAccessoryChanged", sender.IndexToItem(sender.Index).toString());
            
        });

        accessoryTexture.OnListChanged.connect(function (sender, newIndex)
        {
            API.triggerServerEvent("OnCreatorAccessoryTextureChanged", sender.IndexToItem(sender.Index).toString());
            
        });

        torso.OnListChanged.connect(function (sender, newIndex)
        {
            API.triggerServerEvent("OnCreatorTorsoChanged", sender.IndexToItem(sender.Index).toString());
            
        });

        undershirt.OnListChanged.connect(function (sender, newIndex)
        {
            API.triggerServerEvent("OnCreatorUndershirtChanged", sender.IndexToItem(sender.Index).toString());
            
        });

        undershirtTexture.OnListChanged.connect(function (sender, newIndex)
        {
            API.triggerServerEvent("OnCreatorUndershirtTextureChanged", sender.IndexToItem(sender.Index).toString());
            
        });
    }
});

API.onUpdate.connect(function ()
{
    if (menuPool != null)
    {
        menuPool.ProcessMenus();
    }
});