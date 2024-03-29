#region [Copyright (c) 2021 ZeoFlow S.R.L.]
//	Distributed under the terms of an MIT-style license:
//
//	The MIT License
//
//	Copyright (c) 2021 ZeoFlow S.R.L.
//
//	Permission is hereby granted, free of charge, to any person obtaining a copy of this software 
//	and associated documentation files (the "Software"), to deal in the Software without restriction, 
//	including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
//	and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, 
//	subject to the following conditions:
//
//	The above copyright notice and this permission notice shall be included in all copies or substantial 
//	portions of the Software.
//
//	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
//	INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR 
//	PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
//	FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, 
//	ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
#endregion
using UnityEngine;
using UnityEngine.UI;

namespace ZeoFlow.Examples
{
	public class GamepadToggle : MonoBehaviour 
	{
		[SerializeField]
		private string m_keyboardScheme = null;
		[SerializeField]
		private string m_gamepadScheme = null;
		[SerializeField]
		private Text m_status = null;

		private bool m_gamepadOn;

		private void Awake()
		{
			if(InputManager.PlayerOneControlScheme.Name == m_keyboardScheme)
			{
				m_gamepadOn = false;
				m_status.text = "Gamepad: Off";
			}
			else
			{
				m_gamepadOn = true;
				m_status.text = "Gamepad: On";
			}

			InputManager.Loaded += HandleInputLoaded;
		}

		private void OnDestroy()
		{
			InputManager.Loaded -= HandleInputLoaded;
		}

		private void HandleInputLoaded()
		{
			if(m_gamepadOn)
			{
				InputManager.SetControlScheme(m_gamepadScheme, PlayerID.One);
				m_status.text = "Gamepad: On";
			}
			else
			{
				InputManager.SetControlScheme(m_keyboardScheme, PlayerID.One);
				m_status.text = "Gamepad: Off";
			}
		}

		public void Toggle()
		{
			if(m_gamepadOn)
			{
				InputManager.SetControlScheme(m_keyboardScheme, PlayerID.One);
				m_status.text = "Gamepad: Off";
				m_gamepadOn = false;
			}
			else
			{
				InputManager.SetControlScheme(m_gamepadScheme, PlayerID.One);
				m_status.text = "Gamepad: On";
				m_gamepadOn = true;
			}
		}
	}
}
