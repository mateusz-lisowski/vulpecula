using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _193396
{
	public class PlayerInput : EntityBehavior
	{
		[field: Space(10)]
		[field: SerializeField, ReadOnly] public bool isEnabled { get; private set; } = true;
		[field: Space(10)]
		[field: SerializeField, ReadOnly] public bool isInputMoveUp { get; private set; }
		[field: SerializeField, ReadOnly] public bool isInputMoveDown { get; private set; }
		[field: SerializeField, ReadOnly] public bool isInputMoveLeft { get; private set; }
		[field: SerializeField, ReadOnly] public bool isInputMoveRight { get; private set; }
		[field: Space(5)]
		[field: SerializeField, ReadOnly] public bool isInputJump { get; private set; }
		[field: SerializeField, ReadOnly] public bool isInputJumpReleased { get; private set; }
		[field: SerializeField, ReadOnly] public bool isInputDash { get; private set; }
		[field: SerializeField, ReadOnly] public bool isInputDashReleased { get; private set; }
		[field: SerializeField, ReadOnly] public bool isInputPassthrough { get; private set; }
		[field: Space(5)]
		[field: SerializeField, ReadOnly] public bool isInputAttack { get; private set; }
		[field: SerializeField, ReadOnly] public bool isInputAttackDown { get; private set; }


		public override void onUpdate()
		{
			isInputMoveUp		= isEnabled && Input.GetKey(KeyCode.UpArrow);
			isInputMoveDown		= isEnabled && Input.GetKey(KeyCode.DownArrow);
			isInputMoveLeft		= isEnabled && Input.GetKey(KeyCode.LeftArrow);
			isInputMoveRight	= isEnabled && Input.GetKey(KeyCode.RightArrow);

			isInputJump			= isEnabled && (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Space));
			isInputJumpReleased	= !isEnabled || !(Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.Space));

			isInputDash			= isEnabled && Input.GetKeyDown(KeyCode.C);
			isInputDashReleased	= !isEnabled || !Input.GetKey(KeyCode.C);

			isInputPassthrough	= isEnabled && Input.GetKeyDown(KeyCode.DownArrow);

			isInputAttack		= isEnabled && Input.GetKeyDown(KeyCode.X);
			isInputAttackDown	= isEnabled && Input.GetKey(KeyCode.DownArrow);
		}

		public override string[] capturableEvents => new string[] { "inputEnable", "inputDisable" };
		public override void onEvent(string eventName, object eventData)
		{
			switch (eventName)
			{
				case "inputEnable": isEnabled = true; break;
				case "inputDisable": isEnabled = false; break;
			}
		}
	}
}