using System.Collections.Generic;
using UnityEngine;

namespace RemoteObject
{
    public class RemoteObjectManager : MonoBehaviour
    {
        #region Singleton
        private static RemoteObjectManager _instance;
        public RemoteObjectManager Instance
        {
            get
            {
                if (_instance is null)
                {
                    _instance = FindObjectOfType<RemoteObjectManager>();

                    if (_instance is null)
                    {
                        _instance = new GameObject($"{nameof(RemoteObjectManager)}")
                            .AddComponent<RemoteObjectManager>();
                    }
                }

                return _instance;
            }
        }
        #endregion

        private List<RemoteObject> _registeredRemoteObjects;
        public bool RegisterRemoteObject(RemoteObject NewRemoteObject)
        {
            return false;
        }

        public bool UnregisterRemoteObejct(string TargetID)
        {
            return false;
        }
        
        /// <summary>
        /// 각 대상 원격 객체들에게 LLM으로부터 전달받은 명령을 수행하도록 지시
        /// </summary>
        /// <param name="CommandMessage">LLM으로부터 전달 받은 명령 목록</param>
        public void OrderCommands(RemoteCommandMessage CommandMessage)
        {
            
        }
    }
}
