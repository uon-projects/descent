﻿using UnityEngine;

namespace Map
{
	/// <summary>
	///     <para> MapScript2 </para>
	/// </summary>
	public class MapScript2 : MonoBehaviour
	{
		public static bool ColArea1;
		public static bool ColArea2;
		public static bool ColArea3;
		public bool mapOpen;
		public GameObject map;
		public GameObject area1;
		public GameObject area2;
		public GameObject area3;

		/// <summary>
		///     <para> Start </para>
		/// </summary>
		private void Start()
		{
			mapOpen = false;
			ColArea1 = false;
			ColArea2 = false;
			ColArea3 = false;
			area1.SetActive(false);
			area2.SetActive(false);
			area3.SetActive(false);
		}

		/// <summary>
		///     <para> Update </para>
		/// </summary>
		private void Update()
		{
			area1.SetActive(ColArea1);

			area2.SetActive(ColArea2);

			area3.SetActive(ColArea3);

			if (Input.GetKeyDown(KeyCode.M)) mapOpen = !mapOpen;

			if (mapOpen)
			{
				Open_Map();
			}
			else
			{
				area1.SetActive(false);
				area2.SetActive(false);
				area3.SetActive(false);
				Exit_Map();
			}
		}

		/// <summary>
		///		<para> OnTriggerEnter </para>
		/// </summary>
		/// <param name="collision"></param>
		// public void OnTriggerEnter(Collision collision)
		// {
		// 	if (collision.gameObject.name == "Area1")
		// 	{
		// 		Debug.Log("Collision Detected");
		// 	}
		// }

		/// <summary>
		///     <para> Exit_Map </para>
		/// </summary>
		private void Exit_Map()
		{
			map.SetActive(false);
			mapOpen = false;
			// Debug.Log("Exiting map");
		}

		/// <summary>
		///     <para> Open_Map </para>
		/// </summary>
		private void Open_Map()
		{
			map.SetActive(true);
			mapOpen = true;
			// Debug.Log("opening map");
		}
	}
}