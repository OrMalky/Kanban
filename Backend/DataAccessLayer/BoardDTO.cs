using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend.BusinessLayer;


namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    public class BoardDTO : IDTO
    {
        //Members
        private int id;
        private string name;
        private string owner;
        private List<int> maxTasks;
        private List<string> members;

        //Properties
        public int Id { get => id; }
        public string Name { get => name; }
        public string Owner { get => owner;  set => owner = value;}
        public List<int> MaxTasks { get => maxTasks; set=> maxTasks =value ;}
        public List<string> Members { get => members; set => members = value;}

        public BoardDTO() { }

        public BoardDTO(int id, string name, string owner, List<int> maxTasks, List<string> members)
        {
            this.id = id;
            this.name = name;
            this.owner = owner;
            this.maxTasks = maxTasks;
            this.members = members;
        }
    }
}
