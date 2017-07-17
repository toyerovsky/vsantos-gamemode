/* Copyright (C) Przemysław Postrach - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Przemysław Postrach <toyerek@gmail.com> July 2017
 */

using System;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Constant;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Shared;
using GrandTheftMultiplayer.Shared.Math;
using Serverside.Controllers;

namespace Serverside.Core.Description
{
    public class Description : IDisposable
    {
        private TextLabel DescriptionLabel { get; }
        private TextLabelProperties Properties { get; set; }
        private ServerAPI Api { get; set; }


        public Description(AccountController player)
        {
            Api = API.shared;
            DescriptionLabel = Api.createTextLabel("", player.Client.position, 10f, 1f, true,
                player.Client.dimension);

            DescriptionLabel.color = new Color(192, 192, 192);
            DescriptionLabel.collisionless = true;
            DescriptionLabel.seethrough = true;
            DescriptionLabel.invincible = true;
            DescriptionLabel.transparency = 250;
            DescriptionLabel.attachTo(player.Client, "SKEL_ROOT", new Vector3(50f, 1f, 1f), player.Client.rotation);

            //Jak gracz zmieni wymiar to żeby miał opis np. jak wejdzie do interioru
            //to zmieniamy wymiar temu labelowi opisu

            player.CharacterController.OnPlayerDimensionChanged += (e, args) =>
            {
                Api.setEntityDimension(DescriptionLabel.handle, args.CurrentDimension);
            };
        }

        public Description(VehicleController vehicle)
        {
            
        }

        private string value;

        public string Value
        {
            get { return value; }
            set
            {
                this.value = value;
                Redraw(value);
            }
        } 

        public void ResetCurrentDescription()
        {
            Value = String.Empty;   
        }

        private void Redraw(string text)
        {
            DescriptionLabel.text = text;
        }

        public void Dispose()
        {
            DescriptionLabel.detach(true);
            DescriptionLabel.delete();
        }
    }
}