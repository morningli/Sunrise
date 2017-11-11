using System.Collections.Generic;
using System;
using UnityEngine;

namespace SignalModule
{
	public delegate void FuncWithParam0();
	public delegate void FuncWithParam1<T>(T t1);
	public delegate void FuncWithParam2<T1, T2>(T1 t1, T2 t2);	
	public delegate void FuncWithParam3<T1, T2, T3>(T1 t1, T2 t2, T3 t3);
	public delegate void FuncWithParam4<T1, T2, T3, T4>(T1 t1, T2 t2, T3 t3, T4 t4);	
	public delegate void FuncWithParam5<T1, T2, T3, T4, T5>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5);

    public enum GAME_EVT
	{
		// game common event
		LOCAL_EVENT = 0,
		DATA_SYNC = 1, //数据同步
		SIGNAL_CUSTOM_MIN = 1000, //自定义信号起始位置
	}
    
	// define event name here
	public class SignalMgr	{		

		public static SignalMgr instance = new SignalMgr();
		Dictionary<ulong, System.Delegate> _msgMap = null;
		Dictionary<ulong, System.Delegate> _signal_auto = null;
			
        public SignalMgr()
        {
			_msgMap = new Dictionary<ulong, Delegate>();
			_signal_auto = new Dictionary<ulong, Delegate>();
        }
        
        public void Dispose()
        {
            _msgMap.Clear();
        }

		public void Subscribe(ulong signal, Delegate listener, bool auto_remove = false)
		{			
			if (listener == null)
			{
				return;
			}

			lock (_msgMap) {
				Delegate func;
				if (_msgMap.ContainsKey (signal)) {
					func = _msgMap [signal];
					func = Delegate.Combine (func, listener);
				} else {
					func = listener;
				}
				_msgMap [signal] = func;
			}

			if (auto_remove)
			{
				lock (_signal_auto) {
					Delegate func_auto;
					if (_signal_auto.ContainsKey (signal)) {
						func_auto = _signal_auto [signal];
						func_auto = Delegate.Combine (func_auto, listener);
					} else {
						func_auto = listener;
					}
					_signal_auto [signal] = func_auto;
				}
			}

			Debug.Log ("add signal:" + signal.ToString());
		}

		public void Unsubscribe(ulong signal, Delegate listener)
		{		
			if (listener == null)
			{
				return;
			}
		
			lock (_msgMap) {
				if (_msgMap.ContainsKey (signal)) {
					Delegate func = _msgMap [signal];
					func = Delegate.RemoveAll (func, listener);
					_msgMap [signal] = func;
				} 
			}
		}
			
		public void RemoveAuto(ulong signal)
		{
			if (_signal_auto.ContainsKey (signal)) {
				Unsubscribe (signal, _signal_auto [signal]);
				lock (_signal_auto) {
					_signal_auto.Remove (signal);
				}
			}
		}

		public void Raise(ulong signal) 
		{
			if(_msgMap.ContainsKey(signal))
			{
				Delegate func = _msgMap[signal];
				FuncWithParam0 tmp = (FuncWithParam0)func;
				tmp();
			}	
			RemoveAuto(signal);
		}

		public void Raise<T1>(ulong signal, T1 args) 
		{
			if(_msgMap.ContainsKey(signal))
			{
				Delegate func = _msgMap[signal];
				FuncWithParam1<T1> tmp = (FuncWithParam1<T1>)func;
				tmp(args);

				Debug.Log ("raise signal succ.signal:" + signal.ToString());
			}	
			RemoveAuto(signal);
		}


		public void Raise<T1, T2>(ulong signal, T1 arg1, T2 arg2) 
		{
			if(_msgMap.ContainsKey(signal))
			{
				Delegate func = _msgMap[signal];
				FuncWithParam2<T1, T2> tmp = (FuncWithParam2<T1, T2>)func;
				tmp(arg1, arg2);
			}	
			RemoveAuto(signal);
		}

		public void Raise<T1, T2, T3>(ulong signal, T1 arg1, T2 arg2, T3 arg3) 
		{
			if(_msgMap.ContainsKey(signal))
			{
				Delegate func = _msgMap[signal];
				FuncWithParam3<T1, T2, T3> tmp = (FuncWithParam3<T1, T2, T3>)func;
				tmp(arg1, arg2, arg3);
			}	
			RemoveAuto(signal);
		}


		public void Raise<T1, T2, T3, T4>(ulong signal, T1 arg1, T2 arg2, T3 arg3, T4 arg4) 
		{
			if(_msgMap.ContainsKey(signal))
			{
				Delegate func = _msgMap[signal];
				FuncWithParam4<T1, T2, T3, T4> tmp = (FuncWithParam4<T1, T2, T3, T4>)func;
				tmp(arg1, arg2, arg3, arg4);
			}	
			RemoveAuto(signal);
		}

		public void Raise<T1, T2, T3, T4, T5>(ulong signal, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) 
		{
			if(_msgMap.ContainsKey(signal))
			{
				Delegate func = _msgMap[signal];
				FuncWithParam5<T1, T2, T3, T4, T5> tmp = (FuncWithParam5<T1, T2, T3, T4, T5>)func;
				tmp(arg1, arg2, arg3, arg4, arg5);
			}	
			RemoveAuto(signal);
		}
	}
}
