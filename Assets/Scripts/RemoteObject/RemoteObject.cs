using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace RemoteObject
{
    public abstract class RemoteObject : MonoBehaviour
    {
        public virtual void Start()
        {
            
        }

        public virtual void Update()
        {
        }

        public static string ID { get; protected set; }
        public string ObjectType { get; protected set; }

        /// <summary>
        /// 원격 객체 자신이 할당받은 객체 유형이 가지는 객체 상태 목록
        /// </summary>
        protected Dictionary<string, RemoteStateAttribute> StateAttributes;
        /// <summary>
        /// 원격 객체 자신이 할당받은 객체 유형이 가지는 객체 기능 목록
        /// </summary>
        protected Dictionary<string, RemoteActionAttribute> ActionAttributes;

        /// <summary>
        /// StateAttributes에 존재하는 객체 상태를 인스턴스에 등록
        /// </summary>
        private void RegisterState()
        {
            
        }

        /// <summary>
        /// ActionAttributes에 존재하는 객체 기능 인스턴스에 등록
        /// </summary>
        private void RegisterAction()
        {
            
        }

        /// <summary>
        /// 새로운 객체 상태 정보 등록
        /// </summary>
        /// <param name="stateID">새로운 값을 넣을 상태 정보 ID</param>
        /// <param name="newRemoteState">새로운 상태 정보 값</param>
        /// <returns>StateAttributes에 stateID 키가 존재하는 가?</returns>
        public bool SetState(string stateID, RemoteStateAttribute newRemoteState)
        {
            // 유효하지 않은 stateID 값
            if (!StateAttributes.ContainsKey(stateID)) return false;

            StateAttributes[stateID] = newRemoteState;
            return true;
        }

        // public RemoteObject ExecuteAction(string actionID, RemoteActionAttribute newRemoteAction)
        // {
        //     
        // }
    }
}