using System.Collections.Generic;

namespace RemoteObject
{
    /// <summary>
    /// Visibility, Enabled, Transform(Position, Rotation) 과 같이 GameObject와 관련된 정보를 LLM에 보내는 메시지에 담기 위한
    /// Wrapper 클래스
    /// </summary>
    public class RemoteObjectStatus
    {
        public RemoteObjectStatus(Dictionary<string, RemoteStateAttribute> statusAttribute)
        {
            
        }

        public override string ToString()
        {
            // TODO: RemoteObject의 상태 정보를 직렬화
            return "Error(RemoteObjectStatus) : Failed to convert from RemoteObjectStatus to string";
        }
    }
}