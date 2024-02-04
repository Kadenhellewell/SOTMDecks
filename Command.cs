using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOTMDecks
{
    internal abstract class Command
    {
        protected Player player_;

        public Command(Player player) { this.player_ = player; }

        public abstract bool Execute();
        public abstract void Undo();
    }
}
