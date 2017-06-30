using GTANetworkServer.Constant;
using Serverside.Controllers;
using Serverside.Database.Models;

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
        }

    }
}