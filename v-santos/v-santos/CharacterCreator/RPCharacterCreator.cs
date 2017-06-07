using System;
using System.Collections.Generic;
using System.Globalization;
using GTANetworkServer;
//using Serverside.Bank;
using Serverside.Core.Extensions;

namespace Serverside.CharacterCreator
{
    public class RPCharacterCreator : Script
    {
        private readonly List<CharacterCreator> _creators;

        public RPCharacterCreator()
        {
            API.onResourceStart += API_onResourceStart;
            API.onClientEventTrigger += API_onClientEventTrigger;
            _creators = new List<CharacterCreator>();
        }

        private void API_onResourceStart()
        {
            APIExtensions.ConsoleOutput("[RPCharacterCreator] Uruchomione pomyslnie.", ConsoleColor.DarkMagenta);
        }

        #region Subskrypcja zdarzenia, i dopasowanie zmienianego obiektu
        private void API_onClientEventTrigger(Client sender, string eventName, params object[] arguments)
        {
            if (eventName == "OnCreatorFatherChanged")
            {
                if (!sender.hasData("CharacterCreatorID")) return;
                var characterCreator = _creators[Convert.ToInt32(sender.getData("CharacterCreatorID"))] as CharacterCreator;

                if (characterCreator != null)
                {
                    characterCreator.FatherId = Convert.ToInt32(arguments[0]);
                    _creators[Convert.ToInt32(sender.getData("CharacterCreatorID"))] = characterCreator;
                }
            }
            else if (eventName == "OnCreatorMotherChanged")
            {
                if (!sender.hasData("CharacterCreatorID")) return;
                var characterCreator = _creators[Convert.ToInt32(sender.getData("CharacterCreatorID"))] as CharacterCreator;

                if (characterCreator != null)
                {
                    characterCreator.MotherId = Convert.ToInt32(arguments[0]);
                    _creators[Convert.ToInt32(sender.getData("CharacterCreatorID"))] = characterCreator;
                }
            }
            else if (eventName == "OnCreatorShapeMixChanged")
            {
                if (!sender.hasData("CharacterCreatorID")) return;
                var characterCreator = _creators[Convert.ToInt32(sender.getData("CharacterCreatorID"))] as CharacterCreator;

                if (characterCreator != null)
                {
                    characterCreator.ShapeMix = float.Parse(arguments[0].ToString(), CultureInfo.InvariantCulture.NumberFormat);
                    _creators[Convert.ToInt32(sender.getData("CharacterCreatorID"))] = characterCreator;
                }
            }
            else if (eventName == "OnCreatorSkinMixChanged")
            {
                if (!sender.hasData("CharacterCreatorID")) return;
                var characterCreator = _creators[Convert.ToInt32(sender.getData("CharacterCreatorID"))] as CharacterCreator;

                if (characterCreator != null)
                {
                    characterCreator.SkinMix = float.Parse(arguments[0].ToString(), CultureInfo.InvariantCulture.NumberFormat);
                    _creators[Convert.ToInt32(sender.getData("CharacterCreatorID"))] = characterCreator;
                }
            }
            else if (eventName == "OnCreatorHairChanged")
            {
                if (!sender.hasData("CharacterCreatorID")) return;
                var characterCreator = _creators[Convert.ToInt32(sender.getData("CharacterCreatorID"))] as CharacterCreator;

                if (characterCreator != null)
                {
                    characterCreator.HairId = Convert.ToInt32(arguments[0]);
                    _creators[Convert.ToInt32(sender.getData("CharacterCreatorID"))] = characterCreator;
                }
            }
            else if (eventName == "OnCreatorHairTextureChanged")
            {
                if (!sender.hasData("CharacterCreatorID")) return;
                var characterCreator = _creators[Convert.ToInt32(sender.getData("CharacterCreatorID"))] as CharacterCreator;

                if (characterCreator != null)
                {
                    characterCreator.HairTexture = Convert.ToInt32(arguments[0]);
                    _creators[Convert.ToInt32(sender.getData("CharacterCreatorID"))] = characterCreator;
                }
            }
            else if (eventName == "OnCreatorHairColorChanged")
            {
                if (!sender.hasData("CharacterCreatorID")) return;
                var characterCreator = _creators[Convert.ToInt32(sender.getData("CharacterCreatorID"))] as CharacterCreator;

                if (characterCreator != null)
                {
                    characterCreator.HairColor = Convert.ToInt32(arguments[0]);
                    _creators[Convert.ToInt32(sender.getData("CharacterCreatorID"))] = characterCreator;
                }
            }
            else if (eventName == "OnCreatorMakeupChanged")
            {
                if (!sender.hasData("CharacterCreatorID")) return;
                var characterCreator = _creators[Convert.ToInt32(sender.getData("CharacterCreatorID"))] as CharacterCreator;

                if (characterCreator != null)
                {
                    characterCreator.MakeupId = Convert.ToInt32(arguments[0]);
                    _creators[Convert.ToInt32(sender.getData("CharacterCreatorID"))] = characterCreator;
                }
            }
            else if (eventName == "OnCreatorFirstMakeupColorChanged")
            {
                if (!sender.hasData("CharacterCreatorID")) return;
                var characterCreator = _creators[Convert.ToInt32(sender.getData("CharacterCreatorID"))] as CharacterCreator;

                if (characterCreator != null)
                {
                    characterCreator.FirstMakeupColor = Convert.ToInt32(arguments[0]);
                    _creators[Convert.ToInt32(sender.getData("CharacterCreatorID"))] = characterCreator;
                }
            }
            else if (eventName == "OnCreatorSecondMakeupColorChanged")
            {
                if (!sender.hasData("CharacterCreatorID")) return;
                var characterCreator = _creators[Convert.ToInt32(sender.getData("CharacterCreatorID"))] as CharacterCreator;

                if (characterCreator != null)
                {
                    characterCreator.SecondMakeupColor = Convert.ToInt32(arguments[0]);
                    _creators[Convert.ToInt32(sender.getData("CharacterCreatorID"))] = characterCreator;
                }
            }
            else if (eventName == "OnCreatorMakeupOpacityChanged")
            {
                if (!sender.hasData("CharacterCreatorID")) return;
                var characterCreator = _creators[Convert.ToInt32(sender.getData("CharacterCreatorID"))] as CharacterCreator;

                if (characterCreator != null)
                {
                    characterCreator.MakeupOpacity = float.Parse(arguments[0].ToString(), CultureInfo.InvariantCulture.NumberFormat);
                    _creators[Convert.ToInt32(sender.getData("CharacterCreatorID"))] = characterCreator;
                }
            }
            else if (eventName == "OnCreatorEyeBrowsChanged")
            {
                if (!sender.hasData("CharacterCreatorID")) return;
                var characterCreator = _creators[Convert.ToInt32(sender.getData("CharacterCreatorID"))] as CharacterCreator;

                if (characterCreator != null)
                {
                    characterCreator.EyeBrowsId = Convert.ToInt32(arguments[0]);
                    _creators[Convert.ToInt32(sender.getData("CharacterCreatorID"))] = characterCreator;
                }
            }
            else if (eventName == "OnCreatorFirstEyeBrowsColorChanged")
            {
                if (!sender.hasData("CharacterCreatorID")) return;
                var characterCreator = _creators[Convert.ToInt32(sender.getData("CharacterCreatorID"))] as CharacterCreator;

                if (characterCreator != null)
                {
                    characterCreator.FirstEyeBrowsColor = Convert.ToInt32(arguments[0]);
                    _creators[Convert.ToInt32(sender.getData("CharacterCreatorID"))] = characterCreator;
                }
            }
            else if (eventName == "OnCreatorSecondEyeBrowsColorChanged")
            {
                if (!sender.hasData("CharacterCreatorID")) return;
                var characterCreator = _creators[Convert.ToInt32(sender.getData("CharacterCreatorID"))] as CharacterCreator;

                if (characterCreator != null)
                {
                    characterCreator.SecondEyeBrowsColor = Convert.ToInt32(arguments[0]);
                    _creators[Convert.ToInt32(sender.getData("CharacterCreatorID"))] = characterCreator;
                }
            }
            else if (eventName == "OnCreatorEyeBrowsOpacityChanged")
            {
                if (!sender.hasData("CharacterCreatorID")) return;
                var characterCreator = _creators[Convert.ToInt32(sender.getData("CharacterCreatorID"))] as CharacterCreator;

                if (characterCreator != null)
                {
                    characterCreator.EyeBrowsOpacity = float.Parse(arguments[0].ToString(), CultureInfo.InvariantCulture.NumberFormat);
                    _creators[Convert.ToInt32(sender.getData("CharacterCreatorID"))] = characterCreator;
                }
            }
            else if (eventName == "OnCreatorLipstickChanged")
            {
                if (!sender.hasData("CharacterCreatorID")) return;
                var characterCreator = _creators[Convert.ToInt32(sender.getData("CharacterCreatorID"))] as CharacterCreator;

                if (characterCreator != null)
                {
                    characterCreator.LipstickId = Convert.ToInt32(arguments[0]);
                    _creators[Convert.ToInt32(sender.getData("CharacterCreatorID"))] = characterCreator;
                }
            }
            else if (eventName == "OnCreatorFirstLipstickColorChanged")
            {
                if (!sender.hasData("CharacterCreatorID")) return;
                var characterCreator = _creators[Convert.ToInt32(sender.getData("CharacterCreatorID"))] as CharacterCreator;

                if (characterCreator != null)
                {
                    characterCreator.FirstLipstickColor = Convert.ToInt32(arguments[0]);
                    _creators[Convert.ToInt32(sender.getData("CharacterCreatorID"))] = characterCreator;
                }
            }
            else if (eventName == "OnCreatorSecondLipstickColorChanged")
            {
                if (!sender.hasData("CharacterCreatorID")) return;
                var characterCreator = _creators[Convert.ToInt32(sender.getData("CharacterCreatorID"))] as CharacterCreator;

                if (characterCreator != null)
                {
                    characterCreator.SecondLipstickColor = Convert.ToInt32(arguments[0]);
                    _creators[Convert.ToInt32(sender.getData("CharacterCreatorID"))] = characterCreator;
                }
            }
            else if (eventName == "OnCreatorLipstickOpacityChanged")
            {
                if (!sender.hasData("CharacterCreatorID")) return;
                var characterCreator = _creators[Convert.ToInt32(sender.getData("CharacterCreatorID"))] as CharacterCreator;

                if (characterCreator != null)
                {
                    characterCreator.LipstickOpacity = float.Parse(arguments[0].ToString(), CultureInfo.InvariantCulture.NumberFormat);
                    _creators[Convert.ToInt32(sender.getData("CharacterCreatorID"))] = characterCreator;
                }
            }
            else if (eventName == "OnCreatorFeetsChanged")
            {
                if (!sender.hasData("CharacterCreatorID")) return;
                var characterCreator = _creators[Convert.ToInt32(sender.getData("CharacterCreatorID"))] as CharacterCreator;

                if (characterCreator != null)
                {
                    characterCreator.FeetsId = Convert.ToInt32(arguments[0]);
                    _creators[Convert.ToInt32(sender.getData("CharacterCreatorID"))] = characterCreator;
                }
            }
            else if (eventName == "OnCreatorFeetsTextureChanged")
            {
                if (!sender.hasData("CharacterCreatorID")) return;
                var characterCreator = _creators[Convert.ToInt32(sender.getData("CharacterCreatorID"))] as CharacterCreator;

                if (characterCreator != null)
                {
                    characterCreator.FeetsTexture = Convert.ToInt32(arguments[0]);
                    _creators[Convert.ToInt32(sender.getData("CharacterCreatorID"))] = characterCreator;
                }
            }
            else if (eventName == "OnCreatorLegsChanged")
            {
                if (!sender.hasData("CharacterCreatorID")) return;
                var characterCreator = _creators[Convert.ToInt32(sender.getData("CharacterCreatorID"))] as CharacterCreator;

                if (characterCreator != null)
                {
                    characterCreator.LegsId = Convert.ToInt32(arguments[0]);
                    _creators[Convert.ToInt32(sender.getData("CharacterCreatorID"))] = characterCreator;
                }
            }
            else if (eventName == "OnCreatorLegsTextureChanged")
            {
                if (!sender.hasData("CharacterCreatorID")) return;
                var characterCreator = _creators[Convert.ToInt32(sender.getData("CharacterCreatorID"))] as CharacterCreator;

                if (characterCreator != null)
                {
                    characterCreator.LegsTexture = Convert.ToInt32(arguments[0]);
                    _creators[Convert.ToInt32(sender.getData("CharacterCreatorID"))] = characterCreator;
                }
            }
            else if (eventName == "OnCreatorTopChanged")
            {
                if (!sender.hasData("CharacterCreatorID")) return;
                var characterCreator = _creators[Convert.ToInt32(sender.getData("CharacterCreatorID"))] as CharacterCreator;

                if (characterCreator != null)
                {
                    characterCreator.TopId = Convert.ToInt32(arguments[0]);
                    _creators[Convert.ToInt32(sender.getData("CharacterCreatorID"))] = characterCreator;
                }
            }
            else if (eventName == "OnCreatorTopTextureChanged")
            {
                if (!sender.hasData("CharacterCreatorID")) return;
                var characterCreator = _creators[Convert.ToInt32(sender.getData("CharacterCreatorID"))] as CharacterCreator;

                if (characterCreator != null)
                {
                    characterCreator.TopTexture = Convert.ToInt32(arguments[0]);
                    _creators[Convert.ToInt32(sender.getData("CharacterCreatorID"))] = characterCreator;
                }
            }
            else if (eventName == "OnCreatorHatChanged")
            {
                if (!sender.hasData("CharacterCreatorID")) return;
                var characterCreator = _creators[Convert.ToInt32(sender.getData("CharacterCreatorID"))] as CharacterCreator;

                if (characterCreator != null)
                {
                    characterCreator.HatId = Convert.ToInt32(arguments[0]);
                    _creators[Convert.ToInt32(sender.getData("CharacterCreatorID"))] = characterCreator;
                }
            }
            else if (eventName == "OnCreatorHatTextureChanged")
            {
                if (!sender.hasData("CharacterCreatorID")) return;
                var characterCreator = _creators[Convert.ToInt32(sender.getData("CharacterCreatorID"))] as CharacterCreator;

                if (characterCreator != null)
                {
                    characterCreator.HatTexture = Convert.ToInt32(arguments[0]);
                    _creators[Convert.ToInt32(sender.getData("CharacterCreatorID"))] = characterCreator;
                }
            }
            else if (eventName == "OnCreatorGlassesChanged")
            {
                if (!sender.hasData("CharacterCreatorID")) return;
                var characterCreator = _creators[Convert.ToInt32(sender.getData("CharacterCreatorID"))] as CharacterCreator;

                if (characterCreator != null)
                {
                    characterCreator.GlassesId = Convert.ToInt32(arguments[0]);
                    _creators[Convert.ToInt32(sender.getData("CharacterCreatorID"))] = characterCreator;
                }
            }
            else if (eventName == "OnCreatorGlassesTextureChanged")
            {
                if (!sender.hasData("CharacterCreatorID")) return;
                var characterCreator = _creators[Convert.ToInt32(sender.getData("CharacterCreatorID"))] as CharacterCreator;

                if (characterCreator != null)
                {
                    characterCreator.GlassesTexture = Convert.ToInt32(arguments[0]);
                    _creators[Convert.ToInt32(sender.getData("CharacterCreatorID"))] = characterCreator;
                }
            }
            else if (eventName == "OnCreatorEarsChanged")
            {
                if (!sender.hasData("CharacterCreatorID")) return;
                var characterCreator = _creators[Convert.ToInt32(sender.getData("CharacterCreatorID"))] as CharacterCreator;

                if (characterCreator != null)
                {
                    characterCreator.EarsId = Convert.ToInt32(arguments[0]);
                    _creators[Convert.ToInt32(sender.getData("CharacterCreatorID"))] = characterCreator;
                }
            }
            else if (eventName == "OnCreatorEarsTextureChanged")
            {
                if (!sender.hasData("CharacterCreatorID")) return;
                var characterCreator = _creators[Convert.ToInt32(sender.getData("CharacterCreatorID"))] as CharacterCreator;

                if (characterCreator != null)
                {
                    characterCreator.EarsTexture = Convert.ToInt32(arguments[0]);
                    _creators[Convert.ToInt32(sender.getData("CharacterCreatorID"))] = characterCreator;
                }
            }
            else if (eventName == "OnCreatorAccessoryChanged")
            {
                if (!sender.hasData("CharacterCreatorID")) return;
                var characterCreator = _creators[Convert.ToInt32(sender.getData("CharacterCreatorID"))] as CharacterCreator;

                if (characterCreator != null)
                {
                    characterCreator.AccessoryId = Convert.ToInt32(arguments[0]);
                    _creators[Convert.ToInt32(sender.getData("CharacterCreatorID"))] = characterCreator;
                }
            }
            else if (eventName == "OnCreatorAccessoryTextureChanged")
            {
                if (!sender.hasData("CharacterCreatorID")) return;
                var characterCreator = _creators[Convert.ToInt32(sender.getData("CharacterCreatorID"))] as CharacterCreator;

                if (characterCreator != null)
                {
                    characterCreator.AccessoryTexture = Convert.ToInt32(arguments[0]);
                    _creators[Convert.ToInt32(sender.getData("CharacterCreatorID"))] = characterCreator;
                }
            }
            else if (eventName == "OnCreatorTorsoChanged")
            {
                if (!sender.hasData("CharacterCreatorID")) return;
                var characterCreator = _creators[Convert.ToInt32(sender.getData("CharacterCreatorID"))] as CharacterCreator;

                if (characterCreator != null)
                {
                    characterCreator.TorsoId = Convert.ToInt32(arguments[0]);
                    _creators[Convert.ToInt32(sender.getData("CharacterCreatorID"))] = characterCreator;
                }
            }
            else if (eventName == "OnCreatorUndershirtChanged")
            {
                if (!sender.hasData("CharacterCreatorID")) return;
                var characterCreator = _creators[Convert.ToInt32(sender.getData("CharacterCreatorID"))] as CharacterCreator;

                if (characterCreator != null)
                {
                    characterCreator.UndershirtId = Convert.ToInt32(arguments[0]);
                    _creators[Convert.ToInt32(sender.getData("CharacterCreatorID"))] = characterCreator;
                }
            }
            else if (eventName == "OnCreatorUndershirtTextureChanged")
            {
                if (!sender.hasData("CharacterCreatorID")) return;
                var characterCreator = _creators[Convert.ToInt32(sender.getData("CharacterCreatorID"))] as CharacterCreator;

                if (characterCreator != null)
                {
                    characterCreator.UndershirtTexture = Convert.ToInt32(arguments[0]);
                    _creators[Convert.ToInt32(sender.getData("CharacterCreatorID"))] = characterCreator;
                }
            }
        }
        #endregion

        private void RPLogin_OnCharacterNotCreated(Client player)
        {
            API.triggerClientEvent(player, "OnPlayerCreateCharacter");
            CharacterCreator creator = new CharacterCreator(player);
            _creators.Add(creator);
            player.SetData("CharacterCreatorID", _creators.IndexOf(creator));
        }

        [Command("test")]
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

            CharacterCreator creator = new CharacterCreator(sender)
            {
                FatherId = 0,
                MotherId = 0
            };

            _creators.Add(creator);
            sender.SetData("CharacterCreatorID", _creators.IndexOf(creator));

        }
    }
}