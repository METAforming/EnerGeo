using System.Collections.Generic;

namespace RemoteObject
{
    public class RemoteCommandMessage
    {
        public string ID { get; }

        public List<StateChangeCommand> StateChangeCommands;
        public List<ActionCommand> ActionCommands;
    }
}