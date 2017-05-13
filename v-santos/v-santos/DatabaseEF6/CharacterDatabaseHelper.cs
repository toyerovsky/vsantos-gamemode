using Serverside.DatabaseEF6.Models;
using System.Collections.Generic;
using System.Linq;

namespace Serverside.DatabaseEF6
{
    public class CharacterDatabaseHelper
    {
        public List<Character> SelectCharactersList(long accountId)
        {
            return RoleplayConnection.Instance.Characters.Where(x => x.AID == accountId).ToList();
        }

        public Character SelectCharacter(long cid)
        {
            return RoleplayConnection.Instance.Characters.Where(x => x.CID == cid).FirstOrDefault();
        }

        public void AddCharacter(Character buliding)
        {
            RoleplayConnection.Instance.Characters.Add(buliding);
            RoleplayConnection.Instance.SaveChanges();
        }

        public void UpdateCharacter(Character buliding)
        {
            RoleplayConnection.Instance.Characters.Attach(buliding);
            RoleplayConnection.Instance.Entry(buliding).State = System.Data.Entity.EntityState.Modified;
            RoleplayConnection.Instance.SaveChanges();
        }

        public void DeleteCharacter(long cid)
        {
            Character delobj = RoleplayConnection.Instance.Characters.Where(x => x.CID == cid).FirstOrDefault();
            RoleplayConnection.Instance.Characters.Remove(delobj);
            RoleplayConnection.Instance.SaveChanges();
        }
    }
}
