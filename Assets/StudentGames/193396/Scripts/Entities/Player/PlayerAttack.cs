using UnityEngine;

namespace _193396
{
	[RequireComponent(typeof(PlayerInfo))]
	[RequireComponent(typeof(PlayerInput))]
	[RequireComponent(typeof(PlayerMovement))]
	public class PlayerAttack : EntityBehavior
	{
		private PlayerInfo info;
		private PlayerInput input;
		private PlayerMovement movement;
		private PlayerData data => info.data;
		[field: Space(10)]
		[field: SerializeField, ReadOnly] public bool isAttackDownInput { get; private set; }
		[field: Space(10)]
		[field: SerializeField, ReadOnly] public float lastAttackInputTime { get; private set; }
		[field: SerializeField, ReadOnly] public float lastAttackTime { get; private set; }
		[field: SerializeField, ReadOnly] public float lastAttackDownTime { get; private set; }
		[field: Space(5)]
		[field: SerializeField, ReadOnly] public float attackAnyCooldown { get; private set; }
		[field: SerializeField, ReadOnly] public float attackForwardCooldown { get; private set; }
		[field: SerializeField, ReadOnly] public float attackDownCooldown { get; private set; }
		[field: Space(5)]
		[field: SerializeField, ReadOnly] public int attackForwardCombo { get; private set; }


		private Transform attackForwardTransform;
		private Transform attackForwardStrongTransform;
		private Transform attackForwardAirTransform;
		private Transform attackDownTransform;

		private string currentAttackName = null;
		private ProjectileBehavior currentAttackData = null;


		public override void onAwake()
		{
			info = controller.getBehavior<PlayerInfo>();
			input = controller.getBehavior<PlayerInput>();
			movement = controller.getBehavior<PlayerMovement>();

			attackForwardTransform = transform.Find("Attack/Forward");
			attackForwardStrongTransform = transform.Find("Attack/Forward Strong");
			attackForwardAirTransform = transform.Find("Attack/Forward Air");
			attackDownTransform = transform.Find("Attack/Down");

			lastAttackInputTime = float.PositiveInfinity;
			lastAttackTime = float.PositiveInfinity;
			lastAttackDownTime = float.PositiveInfinity;
		}

		public override string[] capturableEvents =>
			new string[] { "attackForward", "attackForwardReset", "attackForwardAir", "attackDown", "attackExit" };
		public override void onEvent(string eventName, object eventData)
		{
			switch (eventName)
			{
				case "attackForward": attackForwardInstantiate(); break;
				case "attackForwardReset": attackForwardReset(); break;
				case "attackForwardAir": attackForwardAirInstantiate(); break;
				case "attackDown": attackDownInstantiate(); break;
				case "attackExit":
					if (eventData != null && eventData as string == currentAttackName)
						attackFinish();
					break;
			}
		}

		public override void onUpdate()
		{
			updateTimers();

			updateInputs();

			updateAttack();
		}


		private void updateTimers()
		{
			lastAttackInputTime += Time.deltaTime;
			lastAttackTime += Time.deltaTime;
			lastAttackDownTime += Time.deltaTime;

			attackAnyCooldown -= Time.deltaTime;
			attackForwardCooldown -= Time.deltaTime;
			attackDownCooldown -= Time.deltaTime;
		}

		private void updateInputs()
		{
			if (input.isInputAttack)
				lastAttackInputTime = 0;

			isAttackDownInput = input.isInputAttackDown;
		}


		private void attackFinish()
		{
			movement.isAttacking = false;
		}

		private void attackForwardReset()
		{
			attackFinish();

			attackAnyCooldown = 0;
			attackForwardCooldown = 0;
		}
		private void attackForwardInstantiate()
		{
			bool isStrong = attackForwardCombo >= 3;
			Transform currentAttackTransform =
				isStrong ? attackForwardStrongTransform : attackForwardTransform;

			GameObject currentAttack = QolUtility.Instantiate(
				isStrong ? data.attack.attackForward3Prefab
				: attackForwardCombo == 2 ? data.attack.attackForward2Prefab
				: data.attack.attackForward1Prefab,
				currentAttackTransform.position, currentAttackTransform.rotation, transform);

			currentAttackData = currentAttack.GetComponent<ProjectileBehavior>();

			currentAttackData.initialize(controller, data);
			currentAttackData.setFrameVelocity(new Vector2(controller.rigidBody.velocity.x, 0));
			currentAttackData.setStrength(isStrong ? data.attack.forwardStrongStrength
				: data.attack.forwardStrength);
			currentAttackData.setHitboxSize(currentAttackTransform.localScale);
		}
		private void attackForward()
		{
			movement.isAttacking = true;
			movement.lastAttackDown = false;

			lastAttackTime = 0;
			lastAttackInputTime = float.PositiveInfinity;
			attackAnyCooldown = attackForwardCooldown = data.attack.forwardCooldown;

			attackForwardCombo++;

			if (attackForwardCombo == 1)
				currentAttackName = "attack1";
			else if (attackForwardCombo == 2)
				currentAttackName = "attack2";
			else
				currentAttackName = "attack3";
		}

		private void attackForwardAirInstantiate()
		{
			GameObject currentAttack = QolUtility.Instantiate(data.attack.attackForwardAirPrefab,
				attackForwardAirTransform.position, attackForwardAirTransform.rotation, transform);

			currentAttackData = currentAttack.GetComponent<ProjectileBehavior>();

			currentAttackData.initialize(controller, data);
			currentAttackData.setFrameVelocity(new Vector2(controller.rigidBody.velocity.x, 0));
			currentAttackData.setStrength(data.attack.forwardAirStrength);
			currentAttackData.setHitboxSize(attackForwardAirTransform.localScale);
		}
		private void attackForwardAir()
		{
			movement.isAttacking = true;
			movement.lastAttackDown = false;

			lastAttackTime = 0;
			lastAttackInputTime = float.PositiveInfinity;
			attackAnyCooldown = attackForwardCooldown = data.attack.forwardCooldown;

			currentAttackName = "attackAir";
		}

		private void attackDownHitCallback(HitData hitData)
		{
			if (hitData.bounce)
				movement.registeredDownHitHighJump = true;

			movement.registeredDownHitJump = true;
			hitData.source.setHitCallback(null);
		}
		private void attackDownInstantiate()
		{
			GameObject currentAttack = QolUtility.Instantiate(data.attack.attackDownPrefab,
				attackDownTransform.position, attackDownTransform.rotation, transform);

			currentAttackData = currentAttack.GetComponent<ProjectileBehavior>();

			currentAttackData.initialize(controller, data);
			currentAttackData.setStrength(data.attack.downStrength);
			currentAttackData.setHitboxSize(attackDownTransform.localScale);
			currentAttackData.setHitCallback(attackDownHitCallback);
			currentAttackData.setVertical();
		}
		private void attackDown()
		{
			movement.isAttacking = true;
			movement.lastAttackDown = true;

			lastAttackTime = 0;
			lastAttackDownTime = 0;
			lastAttackInputTime = float.PositiveInfinity;
			attackAnyCooldown = attackDownCooldown = data.attack.downCooldown;

			currentAttackName = "attackDown";
		}

		private bool canAttackAny()
		{
			return attackAnyCooldown <= 0 && lastAttackInputTime <= data.attack.inputBufferTime
				&& !movement.isDistressed && !movement.isAttacking;
		}
		private bool canAttackDown()
		{
			return attackDownCooldown <= 0;
		}
		private bool canAttackForward()
		{
			return attackForwardCooldown <= 0;
		}
		private bool canAttackForwardAir()
		{
			return movement.lastGroundedTime >= data.attack.airMinTime;
		}
		private void updateAttack()
		{
			if (!input.isEnabled || movement.isDistressed)
			{
				movement.isAttacking = false;
				if (currentAttackData != null)
					currentAttackData.halt();
			}

			if (movement.isDistressed || lastAttackTime > data.attack.comboResetTime)
				attackForwardCombo = 0;

			if (canAttackAny())
			{
				if (isAttackDownInput)
				{
					if (canAttackDown())
						attackDown();
				}
				else
				{
					if (canAttackForward())
						if (canAttackForwardAir())
							attackForwardAir();
						else
							attackForward();
				}

				controller.onEvent("attackBegin", currentAttackName);
			}
		}
	}
}