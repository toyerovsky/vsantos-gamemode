using System.Data.Entity;
using Serverside.Database;
using Serverside.Groups;
using Serverside.Groups.Base;
using Group = Serverside.Database.Models.Group;
using GTANetworkServer.Constant;

namespace Serverside.Controllers
{
    public class GroupController
    {
        public Group Data { get; set; }

        public long GroupId { get; set; }

        public GroupController(Group data)
        {
            this.Data = data;
        }

        /// <summary>
        /// Dodawanie nowej grupy
        /// </summary>
        /// <param name="name"></param>
        /// <param name="tag"></param>
        /// <param name="type"></param>
        /// <param name="color"></param>
        public GroupController(string name, string tag, GroupType type, Color color)
        {
            this.Data.Name = name;
            this.Data.Tag = tag;
            this.Data.GroupType = type;
            this.Data.Color = color;
            ContextFactory.Instance.Groups.Add(Data);
            ContextFactory.Instance.SaveChanges();
        }

        public void Save()
        {
            ContextFactory.Instance.Groups.Attach(Data);
            ContextFactory.Instance.Entry(Data).State = EntityState.Modified;
            ContextFactory.Instance.SaveChanges();
        }
    }
}