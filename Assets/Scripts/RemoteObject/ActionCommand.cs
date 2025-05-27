using System.Collections.Generic;

namespace RemoteObject
{
    /// <summary>
    /// LLM으로부터 전달 받은 명령 정보 구조체
    /// </summary>
    public struct ActionCommand
    {
        /// <summary>
        /// 호출할 Action method id(Name)
        /// </summary>
        public string ActionMethodID;
        /// <summary>
        /// Action method를 호출할 때 전달하는 파라미터 목록
        /// </summary>
        public List<dynamic> Parameters;
    }
}