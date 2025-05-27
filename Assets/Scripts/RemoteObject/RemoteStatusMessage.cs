using System.Collections.Generic;

namespace RemoteObject
{
    public class RemoteStatusMessage
    {
        /// <summary>
        /// 원격 객체의 상태 정보 리스트
        /// </summary>
        public List<RemoteObjectStatus> RemoteObjectStatusList;

        /// <summary>
        /// 원격 객체 배열로부터 객체 상태 정보를 받아 메시지에 추가
        /// </summary>
        /// <param name="remoteObjects"></param>
        public void AddRemoteObjectStatus(RemoteObject[] remoteObjects)
        {
            foreach(RemoteObject targetObject in remoteObjects)
            {
                RemoteObjectStatus targetObjectStatus = new RemoteObjectStatus(targetObject.StateAttributes);
                RemoteObjectStatusList.Add(targetObjectStatus);
            }
        }

        public override string ToString()
        {
            // TODO: LLM에 보낼 수 있는 형태로 RemoteObjectStatusList를 직렬화
            return "Error(RemoteStatusMessage) : Failed to convert from RemoteObjectStatusList to string - Empty List";
        }
    }
}