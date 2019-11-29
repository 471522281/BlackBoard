using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blackboard.Command
{
    public abstract class CommandItem
    {
        public abstract void Undo();

        public abstract void Redo();

        public abstract bool Merge(CommandItem newItem);

        public CommandStack commandStack;

        protected CommandItem(CommandStack stack)
        {
            commandStack = stack;
        }
    }
}
