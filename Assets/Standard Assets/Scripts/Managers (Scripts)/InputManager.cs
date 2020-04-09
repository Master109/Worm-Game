using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using Extensions;
using System;

namespace Worms
{
	public class InputManager : SingletonMonoBehaviour<InputManager>
	{
		public Transform trs;
		public static Rewired.Player inputter;
		public InputDevice[] defaultInputDevices = new InputDevice[0];
		public static InputDevice[] inputDevices = new InputDevice[0];
		public static bool UsingGamepad
		{
			get
			{
				return inputDevices.Contains(InputDevice.Gamepad);
			}
		}
		public float joystickDeadzone;
		Vector2 previousMousePosition;
		public bool PauseWhileUnfocused
		{
			get
			{
				return true;
			}
		}
		
		public override void Awake ()
		{
			base.Awake ();
			trs.SetParent(null);
			inputter = ReInput.players.GetPlayer("Player");
			inputDevices = defaultInputDevices;
			if (ReInput.controllers.joystickCount > 0 && !inputDevices.Contains(InputDevice.Gamepad))
				inputDevices = inputDevices.Add(InputDevice.Gamepad);
			ReInput.ControllerConnectedEvent += OnControllerConnected;
			ReInput.ControllerPreDisconnectEvent += OnControllerPreDisconnect;
		}
		
		public virtual void OnControllerConnected (ControllerStatusChangedEventArgs args)
		{
			if (!inputDevices.Contains(InputDevice.Gamepad))
			{
				inputDevices = inputDevices.Add(InputDevice.Gamepad);
				GameManager.activeCursorEntry.rectTrs.gameObject.SetActive(false);
			}
		}

		public virtual void OnControllerPreDisconnect (ControllerStatusChangedEventArgs args)
		{
			if (!inputDevices.Contains(InputDevice.Gamepad) && ReInput.controllers.joystickCount > 0)
			{
				inputDevices = inputDevices.Add(InputDevice.Gamepad);
				GameManager.activeCursorEntry.rectTrs.gameObject.SetActive(false);
			}
			else if (inputDevices.Contains(InputDevice.Gamepad) && ReInput.controllers.joystickCount == 0)
			{
				inputDevices = inputDevices.Remove(InputDevice.Gamepad);
				GameManager.activeCursorEntry.rectTrs.gameObject.SetActive(true);
			}
		}
		
		public virtual void OnDestroy ()
		{
			ReInput.ControllerConnectedEvent -= OnControllerConnected;
			ReInput.ControllerPreDisconnectEvent -= OnControllerPreDisconnect;
		}

		public static Vector2 GetAxis2D (string xAxisName, string yAxisName)
		{
			return Vector2.ClampMagnitude(inputter.GetAxis2D(xAxisName, yAxisName), 1);
		}

		public static Vector2 GetWorldMousePosition ()
		{
			Rect gameViewRect = GameManager.GetSingleton<GameManager>().gameViewRectTrs.GetWorldRect();
			return GameManager.GetSingleton<GameCamera>().camera.ViewportToWorldPoint(gameViewRect.ToNormalizedPosition(Input.mousePosition));
		}
	}

	[Serializable]
	public class InputButton
	{
		public string[] buttonNames;
		public KeyCode[] keyCodes;

		public virtual bool GetDown ()
		{
			bool output = false;
			foreach (KeyCode keyCode in keyCodes)
				output |= Input.GetKeyDown(keyCode);
			foreach (string buttonName in buttonNames)
				output |= InputManager.inputter.GetButtonDown(buttonName);
			return output;
		}

		public virtual bool Get ()
		{
			bool output = false;
			foreach (KeyCode keyCode in keyCodes)
				output |= Input.GetKey(keyCode);
			foreach (string buttonName in buttonNames)
				output |= InputManager.inputter.GetButton(buttonName);
			return output;
		}

		public virtual bool GetUp ()
		{
			bool output = false;
			foreach (KeyCode keyCode in keyCodes)
				output |= Input.GetKeyUp(keyCode);
			foreach (string buttonName in buttonNames)
				output |= InputManager.inputter.GetButtonUp(buttonName);
			return output;
		}
	}

	[Serializable]
	public class InputAxis
	{
		public InputButton positiveButton;
		public InputButton negativeButton;

		public virtual int Get ()
		{
			int output = 0;
			if (positiveButton.Get())
				output ++;
			if (negativeButton.Get())
				output --;
			return output;
		}
	}

	public enum InputDevice
	{
		Keyboard,
		Gamepad,
		Touchscreen
	}
}