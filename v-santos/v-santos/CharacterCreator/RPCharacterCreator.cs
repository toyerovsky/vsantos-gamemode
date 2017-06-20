using System;
using System.Globalization;
using GTANetworkServer;
using Serverside.Core.Extensions;
using Serverside.Core.Extenstions;

namespace Serverside.CharacterCreator
{
    public class RPCharacterCreator : Script
    {
        public RPCharacterCreator()
        {
            API.onResourceStart += API_onResourceStart;
            API.onClientEventTrigger += API_onClientEventTrigger;
        }

        private void API_onResourceStart()
        {
            APIExtensions.ConsoleOutput("[RPCharacterCreator] Uruchomione pomyslnie.", ConsoleColor.DarkMagenta);
        }

        #region Subskrypcja zdarzenia, i dopasowanie zmienianego obiektu
        private void API_onClientEventTrigger(Client sender, string eventName, params object[] arguments)
        {
            switch (eventName)
            {
                case "OnCreatorFatherChanged":
                    {

                        var characterCreator = sender.GetAccountController().CharacterController.CharacterCreator;

                        if (characterCreator != null)
                        {
                            characterCreator.FatherId = Convert.ToInt32(arguments[0]);

                        }
                    }
                    break;
                case "OnCreatorMotherChanged":
                    {

                        var characterCreator = sender.GetAccountController().CharacterController.CharacterCreator;

                        if (characterCreator != null)
                        {
                            characterCreator.MotherId = Convert.ToInt32(arguments[0]);

                        }
                    }
                    break;
                case "OnCreatorShapeMixChanged":
                    {

                        var characterCreator = sender.GetAccountController().CharacterController.CharacterCreator;

                        if (characterCreator != null)
                        {
                            characterCreator.ShapeMix = float.Parse(arguments[0].ToString(), CultureInfo.InvariantCulture.NumberFormat);

                        }
                    }
                    break;
                case "OnCreatorSkinMixChanged":
                    {

                        var characterCreator = sender.GetAccountController().CharacterController.CharacterCreator;

                        if (characterCreator != null)
                        {
                            characterCreator.SkinMix = float.Parse(arguments[0].ToString(), CultureInfo.InvariantCulture.NumberFormat);

                        }
                    }
                    break;
                case "OnCreatorHairChanged":
                    {

                        var characterCreator = sender.GetAccountController().CharacterController.CharacterCreator;

                        if (characterCreator != null)
                        {
                            characterCreator.HairId = Convert.ToInt32(arguments[0]);

                        }
                    }
                    break;
                case "OnCreatorHairTextureChanged":
                    {

                        var characterCreator = sender.GetAccountController().CharacterController.CharacterCreator;

                        if (characterCreator != null)
                        {
                            characterCreator.HairTexture = Convert.ToInt32(arguments[0]);

                        }
                    }
                    break;
                case "OnCreatorHairColorChanged":
                    {

                        var characterCreator = sender.GetAccountController().CharacterController.CharacterCreator;

                        if (characterCreator != null)
                        {
                            characterCreator.HairColor = Convert.ToInt32(arguments[0]);

                        }
                    }
                    break;
                case "OnCreatorMakeupChanged":
                    {

                        var characterCreator = sender.GetAccountController().CharacterController.CharacterCreator;

                        if (characterCreator != null)
                        {
                            characterCreator.MakeupId = Convert.ToInt32(arguments[0]);

                        }
                    }
                    break;
                case "OnCreatorFirstMakeupColorChanged":
                    {

                        var characterCreator = sender.GetAccountController().CharacterController.CharacterCreator;

                        if (characterCreator != null)
                        {
                            characterCreator.FirstMakeupColor = Convert.ToInt32(arguments[0]);

                        }
                    }
                    break;
                case "OnCreatorSecondMakeupColorChanged":
                    {

                        var characterCreator = sender.GetAccountController().CharacterController.CharacterCreator;

                        if (characterCreator != null)
                        {
                            characterCreator.SecondMakeupColor = Convert.ToInt32(arguments[0]);

                        }
                    }
                    break;
                case "OnCreatorMakeupOpacityChanged":
                    {

                        var characterCreator = sender.GetAccountController().CharacterController.CharacterCreator;

                        if (characterCreator != null)
                        {
                            characterCreator.MakeupOpacity = float.Parse(arguments[0].ToString(), CultureInfo.InvariantCulture.NumberFormat);

                        }
                    }
                    break;
                case "OnCreatorEyeBrowsChanged":
                    {

                        var characterCreator = sender.GetAccountController().CharacterController.CharacterCreator;

                        if (characterCreator != null)
                        {
                            characterCreator.EyeBrowsId = Convert.ToInt32(arguments[0]);

                        }
                    }
                    break;
                case "OnCreatorFirstEyeBrowsColorChanged":
                    {

                        var characterCreator = sender.GetAccountController().CharacterController.CharacterCreator;

                        if (characterCreator != null)
                        {
                            characterCreator.FirstEyeBrowsColor = Convert.ToInt32(arguments[0]);

                        }
                    }
                    break;
                case "OnCreatorSecondEyeBrowsColorChanged":
                    {

                        var characterCreator = sender.GetAccountController().CharacterController.CharacterCreator;

                        if (characterCreator != null)
                        {
                            characterCreator.SecondEyeBrowsColor = Convert.ToInt32(arguments[0]);

                        }
                    }
                    break;
                case "OnCreatorEyeBrowsOpacityChanged":
                    {

                        var characterCreator = sender.GetAccountController().CharacterController.CharacterCreator;

                        if (characterCreator != null)
                        {
                            characterCreator.EyeBrowsOpacity = float.Parse(arguments[0].ToString(), CultureInfo.InvariantCulture.NumberFormat);

                        }
                    }
                    break;
                case "OnCreatorLipstickChanged":
                    {

                        var characterCreator = sender.GetAccountController().CharacterController.CharacterCreator;

                        if (characterCreator != null)
                        {
                            characterCreator.LipstickId = Convert.ToInt32(arguments[0]);

                        }
                    }
                    break;
                case "OnCreatorFirstLipstickColorChanged":
                    {

                        var characterCreator = sender.GetAccountController().CharacterController.CharacterCreator;

                        if (characterCreator != null)
                        {
                            characterCreator.FirstLipstickColor = Convert.ToInt32(arguments[0]);

                        }
                    }
                    break;
                case "OnCreatorSecondLipstickColorChanged":
                    {

                        var characterCreator = sender.GetAccountController().CharacterController.CharacterCreator;

                        if (characterCreator != null)
                        {
                            characterCreator.SecondLipstickColor = Convert.ToInt32(arguments[0]);

                        }
                    }
                    break;
                case "OnCreatorLipstickOpacityChanged":
                    {

                        var characterCreator = sender.GetAccountController().CharacterController.CharacterCreator;

                        if (characterCreator != null)
                        {
                            characterCreator.LipstickOpacity = float.Parse(arguments[0].ToString(), CultureInfo.InvariantCulture.NumberFormat);

                        }
                    }
                    break;
                case "OnCreatorFeetsChanged":
                    {

                        var characterCreator = sender.GetAccountController().CharacterController.CharacterCreator;

                        if (characterCreator != null)
                        {
                            characterCreator.FeetsId = Convert.ToInt32(arguments[0]);

                        }
                    }
                    break;
                case "OnCreatorFeetsTextureChanged":
                    {

                        var characterCreator = sender.GetAccountController().CharacterController.CharacterCreator;

                        if (characterCreator != null)
                        {
                            characterCreator.FeetsTexture = Convert.ToInt32(arguments[0]);

                        }
                    }
                    break;
                case "OnCreatorLegsChanged":
                    {

                        var characterCreator = sender.GetAccountController().CharacterController.CharacterCreator;

                        if (characterCreator != null)
                        {
                            characterCreator.LegsId = Convert.ToInt32(arguments[0]);

                        }
                    }
                    break;
                case "OnCreatorLegsTextureChanged":
                    {

                        var characterCreator = sender.GetAccountController().CharacterController.CharacterCreator;

                        if (characterCreator != null)
                        {
                            characterCreator.LegsTexture = Convert.ToInt32(arguments[0]);

                        }
                    }
                    break;
                case "OnCreatorTopChanged":
                    {

                        var characterCreator = sender.GetAccountController().CharacterController.CharacterCreator;

                        if (characterCreator != null)
                        {
                            characterCreator.TopId = Convert.ToInt32(arguments[0]);

                        }
                    }
                    break;
                case "OnCreatorTopTextureChanged":
                    {

                        var characterCreator = sender.GetAccountController().CharacterController.CharacterCreator;

                        if (characterCreator != null)
                        {
                            characterCreator.TopTexture = Convert.ToInt32(arguments[0]);

                        }
                    }
                    break;
                case "OnCreatorHatChanged":
                    {

                        var characterCreator = sender.GetAccountController().CharacterController.CharacterCreator;

                        if (characterCreator != null)
                        {
                            characterCreator.HatId = Convert.ToInt32(arguments[0]);

                        }
                    }
                    break;
                case "OnCreatorHatTextureChanged":
                    {

                        var characterCreator = sender.GetAccountController().CharacterController.CharacterCreator;

                        if (characterCreator != null)
                        {
                            characterCreator.HatTexture = Convert.ToInt32(arguments[0]);

                        }
                    }
                    break;
                case "OnCreatorGlassesChanged":
                    {

                        var characterCreator = sender.GetAccountController().CharacterController.CharacterCreator;

                        if (characterCreator != null)
                        {
                            characterCreator.GlassesId = Convert.ToInt32(arguments[0]);

                        }
                    }
                    break;
                case "OnCreatorGlassesTextureChanged":
                    {

                        var characterCreator = sender.GetAccountController().CharacterController.CharacterCreator;

                        if (characterCreator != null)
                        {
                            characterCreator.GlassesTexture = Convert.ToInt32(arguments[0]);

                        }
                    }
                    break;
                case "OnCreatorEarsChanged":
                    {

                        var characterCreator = sender.GetAccountController().CharacterController.CharacterCreator;

                        if (characterCreator != null)
                        {
                            characterCreator.EarsId = Convert.ToInt32(arguments[0]);

                        }
                    }
                    break;
                case "OnCreatorEarsTextureChanged":
                    {

                        var characterCreator = sender.GetAccountController().CharacterController.CharacterCreator;

                        if (characterCreator != null)
                        {
                            characterCreator.EarsTexture = Convert.ToInt32(arguments[0]);

                        }
                    }
                    break;
                case "OnCreatorAccessoryChanged":
                    {

                        var characterCreator = sender.GetAccountController().CharacterController.CharacterCreator;

                        if (characterCreator != null)
                        {
                            characterCreator.AccessoryId = Convert.ToInt32(arguments[0]);

                        }
                    }
                    break;
                case "OnCreatorAccessoryTextureChanged":
                    {

                        var characterCreator = sender.GetAccountController().CharacterController.CharacterCreator;

                        if (characterCreator != null)
                        {
                            characterCreator.AccessoryTexture = Convert.ToInt32(arguments[0]);

                        }
                    }
                    break;
                case "OnCreatorTorsoChanged":
                    {

                        var characterCreator = sender.GetAccountController().CharacterController.CharacterCreator;

                        if (characterCreator != null)
                        {
                            characterCreator.TorsoId = Convert.ToInt32(arguments[0]);

                        }
                    }
                    break;
                case "OnCreatorUndershirtChanged":
                    {

                        var characterCreator = sender.GetAccountController().CharacterController.CharacterCreator;

                        if (characterCreator != null)
                        {
                            characterCreator.UndershirtId = Convert.ToInt32(arguments[0]);

                        }
                    }
                    break;
                case "OnCreatorUndershirtTextureChanged":
                    {

                        var characterCreator = sender.GetAccountController().CharacterController.CharacterCreator;

                        if (characterCreator != null)
                        {
                            characterCreator.UndershirtTexture = Convert.ToInt32(arguments[0]);

                        }
                    }
                    break;
            }
        }
        #endregion

        [Command("kreator")]
        public void Test(Client sender)
        {
            API.shared.setPlayerSkin(sender, PedHash.FreemodeMale01);

            var player = sender.GetAccountController().CharacterController.Character;

            API.triggerClientEvent(sender, "OnPlayerCreateCharacter", CharacterCreator.Mens, CharacterCreator.Womans,
                player.Gender ? CharacterCreator.MensHairId : CharacterCreator.WomansHairId, player.Gender ? CharacterCreator.MenFeet : CharacterCreator.WomanFeets,
                player.Gender ? CharacterCreator.MenLegs : CharacterCreator.WomanLegs, player.Gender ? CharacterCreator.MenLegs : CharacterCreator.WomanLegs,
                player.Gender ? CharacterCreator.MenHats : CharacterCreator.WomanHats, player.Gender ? CharacterCreator.MenGlasses : CharacterCreator.WomanGlasses,
                player.Gender ? CharacterCreator.MenEars : CharacterCreator.WomanEars, player.Gender ? CharacterCreator.MenAccesories : CharacterCreator.WomanAccesories,
                player.Name + " " + player.Surname);

            CharacterCreator creator = new CharacterCreator(sender.GetAccountController().CharacterController)
            {
                FatherId = 0,
                MotherId = 0
            };
        }
    }
}