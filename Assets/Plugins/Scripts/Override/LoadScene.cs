﻿using UnityEngine;

namespace Override
{
	/// <summary>
	///     <para> LoadScene </para>
	///     <author> @TeodorHMX1 </author>
	/// </summary>
	public class LoadScene : MonoBehaviour
	{

		private static LoadScene _mInstance;
		private static AsyncOperation _loadingOperation;
		
		/// <summary>
		///     <para> LoadingOperation </para>
		///     <author> @TeodorHMX1 </author>
		/// </summary>
		public static AsyncOperation LoadingOperation
		{
			get => _loadingOperation;
			set => _loadingOperation = value;
		}

		/// <summary>
		///     <para> Awake </para>
		///     <author> @TeodorHMX1 </author>
		/// </summary>
		private void Awake()
		{
			if (_mInstance == null)
			{
				_mInstance = this;
			}
			else
			{
				Destroy(this);
			}
		}
		
		/// <summary>
		///     <para> Update </para>
		///     <author> @TeodorHMX1 </author>
		/// </summary>
		private void Update()
		{
			Time.timeScale = 1f;
		}
	}
}