﻿/* Copyright (C) Przemysław Postrach - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Przemysław Postrach <toyerek@gmail.com> July 2017
 */

using System.Linq;
using GrandTheftMultiplayer.Server.Constant;
using Serverside.Controllers;
using Serverside.Database.Models;
using Serverside.Groups.Enums;

namespace Serverside.Groups.Base
{
    public class Police : GroupController
    {
        /* OPCJONALNE PRAWA
         * 1 - Megafon
         * 2 - Kajdanki, Prowadzenie gracza, Wpychanie do pojazdu innego gracza
         * 3 - Blokady drogowe
         * 4 - Kolczatka
         */

        public Police(Group editor) : base(editor)
        {
        }

        public Police(string name, string tag, GroupType type, Color color) : base(name, tag, type, color)
        {
        }

        public bool CanPlayerUseMegaphone(AccountController account)
        {
            if (!ContainsWorker(account)) return false;
            Worker worker = GroupData.Workers.First(w => w.Character.Id == account.CharacterController.Character.Id);
            return worker.FirstRight.HasValue && worker.FirstRight.Value;
        }

        public bool CanPlayerDoPolice(AccountController account)
        {
            if (!ContainsWorker(account)) return false;
            Worker worker = GroupData.Workers.First(w => w.Character.Id == account.CharacterController.Character.Id);
            return worker.SecondRight.HasValue && worker.SecondRight.Value;
        }

        public bool CanPlayerPlaceRoadblocks(AccountController account)
        {
            if (!ContainsWorker(account)) return false;
            Worker worker = GroupData.Workers.First(w => w.Character.Id == account.CharacterController.Character.Id);
            return worker.SecondRight.HasValue && worker.SecondRight.Value;
        }

        public bool CanPlayerPlaceSpike(AccountController account)
        {
            if (!ContainsWorker(account)) return false;
            Worker worker = GroupData.Workers.First(w => w.Character.Id == account.CharacterController.Character.Id);
            return worker.SecondRight.HasValue && worker.SecondRight.Value;
        }
    }
}