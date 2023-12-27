using UnityEngine;

namespace _193396
{
	[RequireComponent(typeof(BossInfo))]
	[RequireComponent(typeof(FlipBehavior))]
	[RequireComponent(typeof(GroundedBehavior))]
	[RequireComponent(typeof(RunBehavior))]
	[RequireComponent(typeof(JumpBehavior))]
	public class BossMovement : EntityBehavior
	{
		public enum State
		{
			Idle, Return, Smash, RunAttack, JumpAttack
		}

		public Collider2D bossRoomCheck;
		public Collider2D bossRoomLowerCheck;

		private BossInfo info;
		private BossData data => info.data;
		[field: Space(5)]
		[field: SerializeField, ReadOnly] public bool isPlayerAtGroundRange { get; private set; }
		[field: SerializeField, ReadOnly] public bool isPlayerSmashable { get; private set; }
		[field: Space(5)]
		[field: SerializeField, ReadOnly] public State currentState { get; private set; }
		[field: Space(10)]
		[field: SerializeField, ReadOnly] public float restCooldown { get; private set; }


		private FlipBehavior direction;
		private GroundedBehavior ground;
		private RunBehavior run;
		private JumpBehavior jump;
		private MeleeAtackBehavior smashAttack;
		private MeleeAtackBehavior runAttack;
		private MeleeAtackBehavior jumpAttack;

		private Collider2D smashCheck;


		public override void onAwake()
		{
			info = controller.getBehavior<BossInfo>();

			direction = controller.getBehavior<FlipBehavior>();
			ground = controller.getBehavior<GroundedBehavior>();
			run = controller.getBehavior<RunBehavior>();
			jump = controller.getBehavior<JumpBehavior>();

			var attacks = controller.getBehaviors<MeleeAtackBehavior>();
			smashAttack = attacks[0];
			runAttack = attacks[1];
			jumpAttack = attacks[2];

			smashCheck = transform.Find("Detection/Smash").GetComponent<Collider2D>();

			changeState(State.Idle);
		}

		public override string[] capturableEvents => new string[] { "attackExit" };
		public override void onEvent(string eventName, object eventData)
		{
			switch (eventName)
			{
				case "attackExit":
					string name = eventData == null ? "attack" : eventData as string;
					if (name == smashAttack.data.attackInstantiateEventName)
						changeState(State.Idle);
					break;
			}
		}

		public override void onUpdate()
		{
			restCooldown -= Time.deltaTime;

			updateCollisions();

			updateState();

			tryStartNewState();
		}

		public override bool onFixedUpdate()
		{
			addSmoothForce(0f, run.data.accelerationCoefficient, transform.right);

			return true;
		}


		private void updateCollisions()
		{
			bool wasPlayerAtGroundRange = isPlayerAtGroundRange;
			isPlayerAtGroundRange = bossRoomLowerCheck.IsTouchingLayers(data.targetLayers);

			bool wasPlayerSmashable = isPlayerSmashable;
			isPlayerSmashable = smashCheck.IsTouchingLayers(data.targetLayers);


			if (isPlayerAtGroundRange != wasPlayerAtGroundRange)
				controller.onEvent("playerOnGround", isPlayerAtGroundRange);

			if (isPlayerSmashable != wasPlayerSmashable)
				controller.onEvent("playerSmashable", isPlayerSmashable);
		}

		private void updateState()
		{
			if (currentState == State.JumpAttack)
			{
				jumpAttack.enabled = !ground.isGrounded;
				runAttack.enabled = ground.isGrounded;
			}

			if ((currentState == State.Return || currentState == State.RunAttack || currentState == State.JumpAttack) && run.isFacingWall)
			{
				direction.flip();
				changeState(State.Idle);
			}
			else if (currentState != State.Return && !info.isActive)
				changeState(State.Return);
		}

		private void tryStartNewState()
		{
			if (currentState == State.Return && info.isActive)
			{
				direction.flip();
				changeState(State.Idle);
			}

			if (currentState != State.Idle)
				return;

			if (restCooldown > 0f)
				return;

			if (!isPlayerAtGroundRange)
				changeState(State.JumpAttack);
			else if (isPlayerSmashable)
				changeState(State.Smash);
			else
				changeState(State.RunAttack);
		}


		private void changeState(State newState)
		{
			if (currentState == newState)
				return;

			run.enabled = false;
			jump.enabled = false;

			smashAttack.enabled = false;
			runAttack.enabled = false;
			jumpAttack.enabled = false;

			switch (newState)
			{
				case State.Idle:
					restCooldown = data.restTime;
					break;
				case State.Return:
					if (!direction.isFacingRight)
						direction.flip();
					run.enabled = true;
					break;
				case State.Smash:
					smashAttack.enabled = true;
					break;
				case State.RunAttack:
					run.enabled = true;
					runAttack.enabled = true;
					break;
				case State.JumpAttack:
					run.enabled = true;
					jump.enabled = true;
					runAttack.enabled = true;
					break;
			}

			currentState = newState;
		}
	}
}