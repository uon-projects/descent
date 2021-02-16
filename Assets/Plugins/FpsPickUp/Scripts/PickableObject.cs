﻿using UnityEngine;
using ZeoFlow.Pickup.Interfaces;

namespace ZeoFlow.Pickup
{
	public class PickableObject : MonoBehaviour
	{
		public new GameObject gameObject;
		public PhysicsSub physicsMenu = new PhysicsSub();
		public ThrowingSystemMenu throwingSystem = new ThrowingSystemMenu();
		public PlayerAttachSub playerAttachMenu = new PlayerAttachSub();
		public OutlinerSub outlinerMenu = new OutlinerSub();
		public PuzzleSub puzzleSub = new PuzzleSub();
		
		private bool _isAttached;
		private bool _wasAttached;
		private GameObject _newFlare;

		private void Update()
		{
			if (!_isAttached) return;

			if (gameObject.GetComponent<IOnAttached>() != null)
			{
				gameObject.GetComponent<IOnAttached>().ONUpdate(playerAttachMenu);
			}

			var position = playerAttachMenu.playerObject.transform.position;
			var positionOffset = playerAttachMenu.position;
			position = new Vector3(
				position.x + positionOffset.x,
				position.y + positionOffset.y,
				position.z + positionOffset.z
			);
			
			var eulerAngles = playerAttachMenu.playerObject.transform.eulerAngles;
			var rotationOffset = playerAttachMenu.rotation;
			eulerAngles = new Vector3(
				eulerAngles.x + rotationOffset.x,
				eulerAngles.y + rotationOffset.y,
				eulerAngles.z + rotationOffset.z
			);
			
			if (playerAttachMenu.createNewObject)
			{
				if (_newFlare != null) return;
				
				_newFlare = Instantiate(gameObject, position, Quaternion.Euler(eulerAngles));
				return;
			}

			gameObject.transform.position = position;
			gameObject.transform.eulerAngles = eulerAngles;
		}

		public void OnMovement(bool isRight)
		{
			if (gameObject.GetComponent<IOnPuzzle>() != null)
			{
				gameObject.GetComponent<IOnPuzzle>().ONMovement(isRight);
			}
		}

		public bool IsPuzzleMoving()
		{
			return gameObject.GetComponent<IOnPuzzle>() != null && gameObject.GetComponent<IOnPuzzle>().ONIsMoving();
		}

		public void OnAttach()
		{
			_isAttached = true;
		}

		public void OnDrop()
		{
			_isAttached = false;
		}
		
		public GameObject NewFlare => _newFlare;

	}
}