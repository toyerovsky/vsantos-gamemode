﻿using System.Linq;
using Serverside.Controllers;
using Serverside.Database.Models;

namespace Serverside.Groups.Base
{
    public class CityHall : GroupController
    {
        /* OPCJONALNE PRAWA
         * 1 - wydawanie dowodu osobistego 
         * 2 - wydawanie prawa jazdy
         */

        public CityHall(Group editor) : base(editor)
        {
        }

        public bool CanPlayerGiveIdCard(AccountController account)
        {
            if (Data.Workers.All(w => w.Character.Id != account.CharacterController.Character.Id)) return false;
            Worker worker = Data.Workers.First(w => w.Character.Id == account.CharacterController.Character.Id);
            return worker.FirstRight.HasValue && worker.FirstRight.Value;
        }

        public bool CanPlayerGiveDrivingLicense(AccountController account)
        {
            if (Data.Workers.All(w => w.Character.Id != account.CharacterController.Character.Id)) return false;
            Worker worker = Data.Workers.First(w => w.Character.Id == account.CharacterController.Character.Id);
            return worker.SecondRight.HasValue && worker.SecondRight.Value;
        }
    }
}