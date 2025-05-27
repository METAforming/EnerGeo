using System.Collections.Generic;

namespace RemoteObject
{
    public class RemoteCommandMessage
    {
        public RemoteCommandMessage()
        {
        }

        public string TargetRemoteObjectID;
        public List<StateChangeCommand> StateChangeCommands = null;
        public List<ActionCommand> ActionCommands = null;
    }
}