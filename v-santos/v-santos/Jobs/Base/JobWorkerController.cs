/* Copyright (C) Przemysław Postrach - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Przemysław Postrach <toyerek@gmail.com> July 2017
 */

using System;
using GrandTheftMultiplayer.Server.API;
using Serverside.Controllers;
using Serverside.Core.Extensions;

namespace Serverside.Jobs.Base
{
    public abstract class JobWorkerController
    {
        public AccountController Player { get; set; }
        public JobVehicleController JobVehicle { get; set; }

        protected API Api { get; set; }

        protected JobWorkerController(API api, AccountController player, JobVehicleController jobVehicle)
        {
            Api = api;
            Player = player;
            JobVehicle = jobVehicle;
        }

        public virtual decimal CurrentSalary
        {
            get => Player.CharacterController.Character.MoneyJob ?? 0;
            set
            {
                if (value < 0)
                {
                    Player.CharacterController.Character.MoneyJob = 0;
                    Release();
                }
                else
                {
                    Player.CharacterController.Character.MoneyJob = value;
                }
                Player.CharacterController.Save();
            }
        }

        /// <summary>
        /// Metoda do zwolnienia pracownika w razie nieprawidłowego postępowania
        /// </summary>
        public void Release()
        {
            Player.Client.Notify("Nie wypracowałeś wystarczającej ilości gotówki na pokrycie szkód.");
            Player.Client.Notify("Zostałeś zwolniony. Zanim ponownie znajdziesz zatrudnienie minie trochę czasu.");
            Stop();
            Player.CharacterController.Character.Job = 0;
            Player.CharacterController.Character.JobReleaseDate = DateTime.Now;
            Player.CharacterController.Save();
        }

        public virtual void Start()
        {
            
        }

        public virtual void Stop()
        {
            
        }
    }
}