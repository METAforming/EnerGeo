using System.Collections.Generic;

namespace RemoteObject
{
    public class ActionCommand
    {
        public ActionCommand(string actionName)
        {
            ActionName = actionName;
        }
        
        public string ActionName;
        // TODO: 지금은 특정 Action을 수행하기 위한 parameter명과 실제 값을 모두 string으로 처리
        // 일반화 가능하고 최적화 방법 고안 필요
        public Dictionary<string, string> Parameters;
    }
}