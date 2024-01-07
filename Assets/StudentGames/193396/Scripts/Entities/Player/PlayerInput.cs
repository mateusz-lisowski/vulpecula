using UnityEngine;

namespace _193396
{
	public class PlayerInput : EntityBehavior
	{
		[field: Space(10)]
		[field: SerializeField, ReadOnly] public bool isEnabled { get; protected set; } = true;
		[field: Space(10)]
		[field: SerializeField, ReadOnly] public bool isInputMoveUp { get; protected set; }
		[field: SerializeField, ReadOnly] public bool isInputMoveDown { get; protected set; }
		[field: SerializeField, ReadOnly] public bool isInputMoveLeft { get; protected set; }
		[field: SerializeField, ReadOnly] public bool isInputMoveRight { get; protected set; }
		[field: Space(5)]
		[field: SerializeField, ReadOnly] public bool isInputJump { get; protected set; }
		[field: SerializeField, ReadOnly] public bool isInputJumpReleased { get; protected set; }
		[field: SerializeField, ReadOnly] public bool isInputDash { get; protected set; }
		[field: SerializeField, ReadOnly] public bool isInputDashReleased { get; protected set; }
		[field: SerializeField, ReadOnly] public bool isInputPassthrough { get; protected set; }
		[field: Space(5)]
		[field: SerializeField, ReadOnly] public bool isInputAttack { get; protected set; }
		[field: SerializeField, ReadOnly] public bool isInputAttackDown { get; protected set; }


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