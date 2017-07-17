/* Copyright (C) Przemysław Postrach - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Przemysław Postrach <toyerek@gmail.com> July 2017
 */

using System.Linq;
using GrandTheftMultiplayer.Server.Constant;
using Serverside.Controllers;
using Serverside.Database;
using Serverside.Database.Models;
using Serverside.Groups.Enums;

namespace Serverside.Groups.Base
{
    public class CrimeGroup : GroupController
    {
        /* OPCJONALNE PRAWA
         * 1 - wzywanie Crime Bota
         */
        public CrimeBot.CrimeBot CrimeBot { get; set; }

        public CrimeGroup(Group editor) : base(editor)
        {
        }

        public CrimeGroup(string name, string tag, GroupType type, Color color) : base(name, tag, type, color)
        {
            Database.Models.CrimeBot cb = new Database.Models.CrimeBot
            {
                Name = "",
                Group = this.GroupData
            };
            ContextFactory.Instance.CrimeBots.Add(cb);
            ContextFactory.Instance.SaveChanges();
        }

        public bool CanPlayerCallCrimeBot(AccountController account)
        {
            if (!ContainsWorker(account)) return false;
            Worker worker = GroupData.Workers.First(w => w.Character.Id == account.CharacterController.Character.Id);
            return worker.FirstRight.HasValue && worker.FirstRight.Value;
        }

    }
}