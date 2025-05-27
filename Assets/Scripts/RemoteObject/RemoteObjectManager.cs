using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RemoteObject
{
    public class RemoteObjectManager : MonoBehaviour
    {
        #region Singleton

        private static RemoteObjectManager _instance;

        public static RemoteObjectManager Instance
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

        #region Event function

        public void Start()
        {
            RemoteObject[] remoteObjects = FindObjectsOfType<RemoteObject>();
            foreach(RemoteObject remoteObject in remoteObjects)
            {
                RegisterRemoteObject(remoteObject);
            }
        }

        public void Update()
        {
        }

        #endregion

        [SerializeField]
        private List<RemoteObject> _registeredRemoteObjects = new List<RemoteObject>();

        /// <summary>
        /// Manager의 관리 대상으로 원격 객체 추가
        /// </summary>
        /// <param name="newRemoteObject">Scene에 존재하는 원격 객체 중 관리 대상으로 추가할 객체 ID</param>
        /// <returns>추가가 완료되었는가?</returns>
        public bool RegisterRemoteObject(RemoteObject newRemoteObject)
        {
            if (newRemoteObject is null
                || _registeredRemoteObjects.Exists(ro => ro.ID.Equals(newRemoteObject.ID))) return false;

            _registeredRemoteObjects.Add(newRemoteObject);
            return true;
        }

        /// <summary>
        /// Manager의 관리 대상에서 원격 객체 제거
        /// </summary>
        /// <param name="targetRemoteObjectID">제거할 원격 객체 ID</param>
        /// <returns>제거가 완료되었는가?</returns>
        public bool UnregisterRemoteObject(string targetRemoteObjectID)
        {
            if (!_registeredRemoteObjects.Exists(ro => ro.ID.Equals(targetRemoteObjectID))) return false;

            int targetIndex = _registeredRemoteObjects.FindIndex(ro => ro.ID.Equals(targetRemoteObjectID));
            _registeredRemoteObjects.RemoveAt(targetIndex);
            return true;
        }

        /// <summary>
        /// 각 대상 원격 객체들에게 LLM으로부터 전달받은 명령을 수행하도록 지시
        /// </summary>
        /// <param name="commandMessage">LLM으로부터 전달 받은 명령 목록</param>
        public void OrderCommands(List<RemoteCommandMessage> commandMessage)
        {
            if (!commandMessage.Any()) return;

            foreach (RemoteCommandMessage message in commandMessage)
            {
                RemoteObject targetRemoteObject =
                    _registeredRemoteObjects.Find(o => o.ID == message.TargetRemoteObjectID);

                if (targetRemoteObject is null) continue;
                
                foreach (StateChangeCommand stateCommand in message.StateChangeCommands)
                {
                    /* TODO:
                    stateCommand의 StateMethodID를 활용해 RemoteObjectDefinition에서 해당하는 state method를 검색 후
                    List<dynamic> 형태로 전달된 파라미터를 정렬하여 RemoteStateAttribute 객체로 변환
                    */

                    // RemoteStateAttribute NewStateAttribute = ParseParametersToAttribute(stateCommand.Parameters);
                    // targetRemoteObject.SetState(stateCommand.StateMethodID, NewStateAttribute);
                }

                foreach (ActionCommand actionCommand in message.ActionCommands)
                {
                    /* TODO:
                    actionCommand ActionMethodID를 활용해 RemoteObjectDefinition에서 해당하는 action method를 검색 후
                    List<dynamic> 형태로 전달된 파라미터를 정렬하여 RemoteActionAttribute 객체로 변환
                    */

                    // RemoteActionAttribute NewActionAttribute = ParseParametersToAttribute(actionCommand.Parameters);
                    // targetRemoteObject.ExecuteAction(actionCommand.ActionMethodID, NewActionAttribute);
                }
            }
        }

        public RemoteStatusMessage CreateStatusMessage()
        {
            RemoteStatusMessage newStatusMessage = new RemoteStatusMessage();
            return newStatusMessage;
        }
    }
}