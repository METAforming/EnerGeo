using System.Collections.Generic;

namespace RemoteObject
{
    /// <summary>
    /// LLM으로부터 전달 받은 객체 상태 정보 구조체
    /// </summary>
    public struct StateChangeCommand
    {
        /// <summary>
        /// 호출할 State method id(Name)
        /// </summary>
        public string StateMethodID;

        /// <summary>
        /// State method를 호출할 때 전달할 파라미터 목록
        /// </summary>
        public List<dynamic> Parameters;
    }
}