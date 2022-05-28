﻿using UnityEngine;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System;
using AOT;


namespace agora_rtm {
	public sealed class RtmCallEventHandler {
        private static int id = 0;
        private static Dictionary<int, RtmCallEventHandler> _rtmCallEventHandlerDic = new Dictionary<int, RtmCallEventHandler>();
        private IntPtr _rtmCallEventHandlerPtr = IntPtr.Zero;
        private int _currentIdIndex = 0;
		private IntPtr _eventPtr;

		/// <summary>
		/// 被叫已收到呼叫邀请回调。
		/// </summary>
		/// <param name="localInvitation">一个 \ref agora_rtm.LocalInvitation "LocalInvitation" 对象。</param>
		public delegate void OnLocalInvitationReceivedByPeerHandler(LocalInvitation localInvitation);
		
		/// <summary>
		/// 呼叫邀请已被取消回调。
		/// </summary>
		/// <param name="localInvitation">一个 \ref agora_rtm.LocalInvitation "LocalInvitation" 对象。</param>
		public delegate void OnLocalInvitationCanceledHandler(LocalInvitation localInvitation);
		
		/// <summary>
		/// 呼叫邀请过程失败回调。
		/// </summary>
		/// <param name="localInvitation">一个 \ref agora_rtm.LocalInvitation "LocalInvitation" 对象。</param>
		/// <param name="errorCode">错误码。详见 \ref agora_rtm.LOCAL_INVITATION_ERR_CODE "LOCAL_INVITATION_ERR_CODE"。</param>
		public delegate void OnLocalInvitationFailureHandler(LocalInvitation localInvitation, LOCAL_INVITATION_ERR_CODE errorCode);
		
		/// <summary>
		/// 被叫已接受呼叫邀请回调。
		/// </summary>
		/// <param name="localInvitation">一个 \ref agora_rtm.LocalInvitation "LocalInvitation" 对象。</param>
		/// <param name="response">	被叫设置的响应内容。</param>
		public delegate void OnLocalInvitationAcceptedHandler(LocalInvitation localInvitation, string response);
		
		/// <summary>
		/// 被叫已拒绝呼叫邀请回调。
		/// </summary>
		/// <param name="localInvitation">一个 \ref agora_rtm.LocalInvitation "LocalInvitation" 对象。</param>
		/// <param name="response">被叫设置的响应内容。</param>
		public delegate void OnLocalInvitationRefusedHandler(LocalInvitation localInvitation, string response);
		
		/// <summary>
		/// 拒绝呼叫邀请成功回调。
		/// </summary>
		/// <param name="remoteInvitation">一个 \ref agora_rtm.RemoteInvitation "RemoteInvitation" 对象。</param>
		public delegate void OnRemoteInvitationRefusedHandler(RemoteInvitation remoteInvitation);
		
		/// <summary>
		/// 接受呼叫邀请成功回调。
		/// </summary>
		/// <param name="remoteInvitation">一个 \ref agora_rtm.RemoteInvitation "RemoteInvitation" 对象。</param>
		public delegate void OnRemoteInvitationAcceptedHandler(RemoteInvitation remoteInvitation);
		
		/// <summary>
		/// 收到呼叫邀请回调。
		/// </summary>
		/// <param name="remoteInvitation">一个 \ref agora_rtm.RemoteInvitation "RemoteInvitation" 对象。</param>
		public delegate void OnRemoteInvitationReceivedHandler(RemoteInvitation remoteInvitation);
		
		/// <summary>
		/// 来自主叫的呼叫邀请过程失败回调。
		/// </summary>
		/// <param name="remoteInvitation">一个 \ref agora_rtm.RemoteInvitation "RemoteInvitation" 对象。</param>
		/// <param name="errorCode">错误码。详见 \ref agora_rtm.REMOTE_INVITATION_ERR_CODE "REMOTE_INVITATION_ERR_CODE"。</param>
		public delegate void OnRemoteInvitationFailureHandler(RemoteInvitation remoteInvitation, REMOTE_INVITATION_ERR_CODE errorCode);
		
		/// <summary>
		/// 主叫已取消呼叫邀请回调。
		/// </summary>
		/// <param name="remoteInvitation">一个 \ref agora_rtm.RemoteInvitation "RemoteInvitation" 对象。</param>
		public delegate void OnRemoteInvitationCanceledHandler(RemoteInvitation remoteInvitation);

		public OnLocalInvitationReceivedByPeerHandler OnLocalInvitationReceivedByPeer;
		public OnLocalInvitationCanceledHandler OnLocalInvitationCanceled;
		public OnLocalInvitationFailureHandler OnLocalInvitationFailure;
		public OnLocalInvitationAcceptedHandler OnLocalInvitationAccepted;
		public OnLocalInvitationRefusedHandler OnLocalInvitationRefused;
		public OnRemoteInvitationRefusedHandler OnRemoteInvitationRefused;
		public OnRemoteInvitationAcceptedHandler OnRemoteInvitationAccepted;
		public OnRemoteInvitationReceivedHandler OnRemoteInvitationReceived;
		public OnRemoteInvitationFailureHandler OnRemoteInvitationFailure;
		public OnRemoteInvitationCanceledHandler OnRemoteInvitationCanceled;

		public RtmCallEventHandler() {
			_currentIdIndex = id;
			_rtmCallEventHandlerDic.Add(_currentIdIndex, this);
			var _cRtmCallEventHandler = new CRtmCallEventHandler {
				_onLocalInvitationReceivedByPeer = Marshal.GetFunctionPointerForDelegate(new EngineEventOnLocalInvitationReceivedByPeerHandler(OnLocalInvitationReceivedByPeerCallback)),
				_onLocalInvitationCanceled = Marshal.GetFunctionPointerForDelegate(new EngineEventOnLocalInvitationCanceledHandler(OnLocalInvitationCanceledCallback)),
				_onLocalInvitationFailure = Marshal.GetFunctionPointerForDelegate(new EngineEventOnLocalInvitationFailureHandler(OnLocalInvitationFailureCallback)),
				_onLocalInvitationAccepted = Marshal.GetFunctionPointerForDelegate(new EngineEventOnLocalInvitationAcceptedHandler(OnLocalInvitationAcceptedCallback)),
				_onLocalInvitationRefused = Marshal.GetFunctionPointerForDelegate(new EngineEventOnLocalInvitationRefusedHandler(OnLocalInvitationRefusedCallback)),
				_onRemoteInvitationRefused = Marshal.GetFunctionPointerForDelegate(new EngineEventOnRemoteInvitationRefusedHandler(OnRemoteInvitationRefusedCallback)),
				_onRemoteInvitationAccepted = Marshal.GetFunctionPointerForDelegate(new EngineEventOnRemoteInvitationAcceptedHandler(OnRemoteInvitationAcceptedCallback)),
				_onRemoteInvitationReceived = Marshal.GetFunctionPointerForDelegate(new EngineEventOnRemoteInvitationReceivedHandler(OnRemoteInvitationReceivedCallback)),
				_onRemoteInvitationFailure = Marshal.GetFunctionPointerForDelegate(new EngineEventOnRemoteInvitationFailureHandler(OnRemoteInvitationFailureCallback)),
				_onRemoteInvitationCanceled = Marshal.GetFunctionPointerForDelegate(new EngineEventOnRemoteInvitationCanceledHandler(OnRemoteInvitationCanceledCallback))
			};
			_eventPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(CRtmCallEventHandler)));
			Marshal.StructureToPtr(_cRtmCallEventHandler, _eventPtr, true);

			_rtmCallEventHandlerPtr = IRtmApiNative.i_rtm_call_event_handler_createEventHandler(_currentIdIndex, _eventPtr);
			id ++;
		}


		public void Release() {
			Debug.Log("_rtmCallEventHandlerPtr Release");
			if (_rtmCallEventHandlerPtr == IntPtr.Zero) {
				return;
			}

			Marshal.FreeHGlobal(_eventPtr);
			_eventPtr = IntPtr.Zero;

			_rtmCallEventHandlerDic.Remove(_currentIdIndex);
			IRtmApiNative.i_rtm_call_event_releaseEventHandler(_rtmCallEventHandlerPtr);
			_rtmCallEventHandlerPtr = IntPtr.Zero;
		}

		internal IntPtr GetPtr()
        {
			return _rtmCallEventHandlerPtr;
        }
		
		[MonoPInvokeCallback(typeof(EngineEventOnLocalInvitationReceivedByPeerHandler))]
        private static void OnLocalInvitationReceivedByPeerCallback(int _id, IntPtr localInvitationPtr) {
			if (_rtmCallEventHandlerDic.ContainsKey(_id) && _rtmCallEventHandlerDic[_id].OnLocalInvitationReceivedByPeer != null) {
				if (AgoraCallbackObject.GetInstance()._CallbackQueue != null) {
					AgoraCallbackObject.GetInstance()._CallbackQueue.EnQueue(()=>{
						if (_rtmCallEventHandlerDic.ContainsKey(_id) && _rtmCallEventHandlerDic[_id].OnLocalInvitationReceivedByPeer != null) {
							LocalInvitation _localInvitation = new LocalInvitation(localInvitationPtr, false);
							_rtmCallEventHandlerDic[_id].OnLocalInvitationReceivedByPeer(_localInvitation);
						}
					});
				}
			}
        }

		[MonoPInvokeCallback(typeof(EngineEventOnLocalInvitationCanceledHandler))]
        private static void OnLocalInvitationCanceledCallback(int _id, IntPtr localInvitationPtr) {
			if (_rtmCallEventHandlerDic.ContainsKey(_id) && _rtmCallEventHandlerDic[_id].OnLocalInvitationCanceled != null) {
				if (AgoraCallbackObject.GetInstance()._CallbackQueue != null) {
					AgoraCallbackObject.GetInstance()._CallbackQueue.EnQueue(()=>{
						if (_rtmCallEventHandlerDic.ContainsKey(_id) && _rtmCallEventHandlerDic[_id].OnLocalInvitationCanceled != null) {
							LocalInvitation _localInvitation = new LocalInvitation(localInvitationPtr, false);
							_rtmCallEventHandlerDic[_id].OnLocalInvitationCanceled(_localInvitation);
						}
					});
				}
			}
        }

		[MonoPInvokeCallback(typeof(EngineEventOnLocalInvitationFailureHandler))]
        private static void OnLocalInvitationFailureCallback(int _id, IntPtr localInvitationPtr, LOCAL_INVITATION_ERR_CODE errorCode) {
			if (_rtmCallEventHandlerDic.ContainsKey(_id) && _rtmCallEventHandlerDic[_id].OnLocalInvitationFailure != null) {
				if (AgoraCallbackObject.GetInstance()._CallbackQueue != null) {
					AgoraCallbackObject.GetInstance()._CallbackQueue.EnQueue(()=>{
						if (_rtmCallEventHandlerDic.ContainsKey(_id) && _rtmCallEventHandlerDic[_id].OnLocalInvitationFailure != null) {
							LocalInvitation _localInvitation = new LocalInvitation(localInvitationPtr, false);
							_rtmCallEventHandlerDic[_id].OnLocalInvitationFailure(_localInvitation, errorCode);
						}
					});
				}
			}
        }

		[MonoPInvokeCallback(typeof(EngineEventOnLocalInvitationAcceptedHandler))]
        private static void OnLocalInvitationAcceptedCallback(int _id, IntPtr localInvitationPtr, string response) {
			if (_rtmCallEventHandlerDic.ContainsKey(_id) && _rtmCallEventHandlerDic[_id].OnLocalInvitationAccepted != null) {
				if (AgoraCallbackObject.GetInstance()._CallbackQueue != null) {
					AgoraCallbackObject.GetInstance()._CallbackQueue.EnQueue(()=>{
						if (_rtmCallEventHandlerDic.ContainsKey(_id) && _rtmCallEventHandlerDic[_id].OnLocalInvitationAccepted != null) {
							LocalInvitation _localInvitation = new LocalInvitation(localInvitationPtr, false);
							_rtmCallEventHandlerDic[_id].OnLocalInvitationAccepted(_localInvitation, response);
						}
					});
				}
			}
        }

		[MonoPInvokeCallback(typeof(EngineEventOnLocalInvitationRefusedHandler))]
        private static void OnLocalInvitationRefusedCallback(int _id, IntPtr localInvitationPtr, string response) {
			if (_rtmCallEventHandlerDic.ContainsKey(_id) && _rtmCallEventHandlerDic[_id].OnLocalInvitationRefused != null) {
				if (AgoraCallbackObject.GetInstance()._CallbackQueue != null) {
					AgoraCallbackObject.GetInstance()._CallbackQueue.EnQueue(()=>{
						if (_rtmCallEventHandlerDic.ContainsKey(_id) && _rtmCallEventHandlerDic[_id].OnLocalInvitationRefused != null) {
							LocalInvitation _localInvitation = new LocalInvitation(localInvitationPtr, false);
							_rtmCallEventHandlerDic[_id].OnLocalInvitationRefused(_localInvitation, response);
						}
					});
				}
			}
        }

		[MonoPInvokeCallback(typeof(EngineEventOnRemoteInvitationRefusedHandler))]
        private static void OnRemoteInvitationRefusedCallback(int _id, IntPtr localInvitationPtr) {
			if (_rtmCallEventHandlerDic.ContainsKey(_id) && _rtmCallEventHandlerDic[_id].OnRemoteInvitationRefused != null) {
				if (AgoraCallbackObject.GetInstance()._CallbackQueue != null) {
					AgoraCallbackObject.GetInstance()._CallbackQueue.EnQueue(()=>{
						if (_rtmCallEventHandlerDic.ContainsKey(_id) && _rtmCallEventHandlerDic[_id].OnRemoteInvitationRefused != null) {
							RemoteInvitation _localInvitation = new RemoteInvitation(localInvitationPtr, false);
							_rtmCallEventHandlerDic[_id].OnRemoteInvitationRefused(_localInvitation);
						}
					});
				}
			}
        }

		[MonoPInvokeCallback(typeof(EngineEventOnRemoteInvitationAcceptedHandler))]
        private static void OnRemoteInvitationAcceptedCallback(int _id, IntPtr localInvitationPtr) {
			if (_rtmCallEventHandlerDic.ContainsKey(_id) && _rtmCallEventHandlerDic[_id].OnRemoteInvitationAccepted != null) {
				if (AgoraCallbackObject.GetInstance()._CallbackQueue != null) {
					AgoraCallbackObject.GetInstance()._CallbackQueue.EnQueue(()=>{
						if (_rtmCallEventHandlerDic.ContainsKey(_id) && _rtmCallEventHandlerDic[_id].OnRemoteInvitationAccepted != null) {
							RemoteInvitation _localInvitation = new RemoteInvitation(localInvitationPtr, false);
							_rtmCallEventHandlerDic[_id].OnRemoteInvitationAccepted(_localInvitation);
						}
					});
				}
			}
        }

		[MonoPInvokeCallback(typeof(EngineEventOnRemoteInvitationReceivedHandler))]
        private static void OnRemoteInvitationReceivedCallback(int _id, IntPtr localInvitationPtr) {
			if (_rtmCallEventHandlerDic.ContainsKey(_id) && _rtmCallEventHandlerDic[_id].OnRemoteInvitationReceived != null) {
				if (AgoraCallbackObject.GetInstance()._CallbackQueue != null) {
					AgoraCallbackObject.GetInstance()._CallbackQueue.EnQueue(()=>{
						if (_rtmCallEventHandlerDic.ContainsKey(_id) && _rtmCallEventHandlerDic[_id].OnRemoteInvitationReceived != null) {
							RemoteInvitation _localInvitation = new RemoteInvitation(localInvitationPtr, false);
							_rtmCallEventHandlerDic[_id].OnRemoteInvitationReceived(_localInvitation);
						}
					});
				}
			}
        }

		[MonoPInvokeCallback(typeof(EngineEventOnRemoteInvitationFailureHandler))]
        private static void OnRemoteInvitationFailureCallback(int _id, IntPtr localInvitationPtr, REMOTE_INVITATION_ERR_CODE errorCode) {
			if (_rtmCallEventHandlerDic.ContainsKey(_id) && _rtmCallEventHandlerDic[_id].OnRemoteInvitationFailure != null) {
				if (AgoraCallbackObject.GetInstance()._CallbackQueue != null) {
					AgoraCallbackObject.GetInstance()._CallbackQueue.EnQueue(()=>{
						if (_rtmCallEventHandlerDic.ContainsKey(_id) && _rtmCallEventHandlerDic[_id].OnRemoteInvitationFailure != null) {
							RemoteInvitation _localInvitation = new RemoteInvitation(localInvitationPtr, false);
							_rtmCallEventHandlerDic[_id].OnRemoteInvitationFailure(_localInvitation, errorCode);
						}
					});
				}
			}
        }

		[MonoPInvokeCallback(typeof(EngineEventOnRemoteInvitationCanceledHandler))]
        private static void OnRemoteInvitationCanceledCallback(int _id, IntPtr localInvitationPtr) {
			if (_rtmCallEventHandlerDic.ContainsKey(_id) && _rtmCallEventHandlerDic[_id].OnRemoteInvitationCanceled != null) {
				if (AgoraCallbackObject.GetInstance()._CallbackQueue != null) {
					AgoraCallbackObject.GetInstance()._CallbackQueue.EnQueue(()=>{
						if (_rtmCallEventHandlerDic.ContainsKey(_id) && _rtmCallEventHandlerDic[_id].OnRemoteInvitationCanceled != null) {
							RemoteInvitation _localInvitation = new RemoteInvitation(localInvitationPtr, false);
							_rtmCallEventHandlerDic[_id].OnRemoteInvitationCanceled(_localInvitation);
						}
					});
				}
			}
        }
	}
}
