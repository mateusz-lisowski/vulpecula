using System.Collections.Generic;
using UnityEngine;

namespace _193396
{
	public class PlayerMovement : EntityBehavior
	{
		const float minVerticalMovementVelocity = 0.01f;

		public PlayerData data;
		[field: Space(10)]
		[field: SerializeField, ReadOnly] public bool isFacingRight { get; private set; }
		[field: SerializeField, ReadOnly] public bool isMoving { get; private set; }
		[field: SerializeField, ReadOnly] public bool isJumping { get; private set; }
		[field: SerializeField, ReadOnly] public bool isDashing { get; private set; }
		[field: SerializeField, ReadOnly] public bool isAttacking { get; set; }
		[field: SerializeField, ReadOnly] public bool isInCombo { get; set; }
		[field: SerializeField, ReadOnly] public bool isFalling { get; private set; }
		[field: SerializeField, ReadOnly] public bool isGrounded { get; private set; }
		[field: SerializeField, ReadOnly] public bool isDistressed { get; private set; }
		[field: SerializeField, ReadOnly] public bool isInvulnerable { get; private set; }
		[field: SerializeField, ReadOnly] public bool isFacingWall { get; private set; }
		[field: SerializeField, ReadOnly] public bool isLastFacedWallRight { get; private set; }
		[field: SerializeField, ReadOnly] public bool isOnSlope { get; private set; }
		[field: SerializeField, ReadOnly] public bool isSlopeGrounded { get; private set; }
		[field: SerializeField, ReadOnly] public bool isPassing { get; private set; }
		[field: Space(5)]
		[field: SerializeField, ReadOnly] public bool canWallJump { get; private set; }
		[field: SerializeField, ReadOnly] public bool canGroundDrop { get; private set; }
		[field: SerializeField, ReadOnly] public bool canTakeGroundDamage { get; private set; }
		[field: Space(10)]
		[field: SerializeField, ReadOnly] public int jumpsLeft { get; private set; }
		[field: SerializeField, ReadOnly] public int dashesLeft { get; private set; }
		[field: Space(10)]
		[field: SerializeField, ReadOnly] public float lastTurnTime { get; private set; }
		[field: SerializeField, ReadOnly] public float lastGroundedTime { get; private set; }
		[field: SerializeField, ReadOnly] public float lastHurtTime { get; private set; }
		[field: SerializeField, ReadOnly] public float lastWallHoldingTime { get; private set; }
		[field: SerializeField, ReadOnly] public float lastJumpInputTime { get; private set; }
		[field: SerializeField, ReadOnly] public float lastWallJumpTime { get; private set; }
		[field: SerializeField, ReadOnly] public float lastDashInputTime { get; private set; }
		[field: SerializeField, ReadOnly] public float lastDashTime { get; private set; }
		[field: SerializeField, ReadOnly] public float lastPassInputTime { get; private set; }
		[field: Space(5)]
		[field: SerializeField, ReadOnly] public float hurtCooldown { get; private set; }
		[field: SerializeField, ReadOnly] public float jumpCooldown { get; private set; }
		[field: SerializeField, ReadOnly] public float dashCooldown { get; private set; }


		private Collider2D groundCheck;
		private Collider2D wallCheck;
		private Collider2D slopeCheck;
		private Collider2D withinCheck;
		private Collider2D passingCheck;

		private LayerMask currentGroundLayers;

		private HitData hitData = null;
		private Vector2 groundSlide;
		private Vector2 moveInput;
		private bool passingLayersDisabled = false;

		private int lastJumpFrame = -1;
		private int lastTurnFrame = -1;

		private bool currentJumpCuttable = false;
		private bool jumpCutInput = false;
		private bool dashCutInput = false;

		[HideInInspector] public bool registeredDownHitJump = false;
		[HideInInspector] public bool registeredDownHitHighJump = false;
		[HideInInspector] public bool lastAttackDown = false;


		public override void onAwake()
		{
			groundCheck = transform.Find("Detection/Ground").GetComponent<Collider2D>();
			wallCheck = transform.Find("Detection/Wall").GetComponent<Collider2D>();
			slopeCheck = transform.Find("Detection/Slope").GetComponent<Collider2D>();
			withinCheck = transform.Find("Detection/Within").GetComponent<Collider2D>();
			passingCheck = transform.Find("Detection/Passing").GetComponent<Collider2D>();

			currentGroundLayers = data.run.groundLayers;

			isFacingRight = true;

			lastTurnTime = float.PositiveInfinity;
			lastGroundedTime = float.PositiveInfinity;
			lastHurtTime = float.PositiveInfinity;
			lastWallHoldingTime = float.PositiveInfinity;
			lastJumpInputTime = float.PositiveInfinity;
			lastWallJumpTime = float.PositiveInfinity;
			lastDashInputTime = float.PositiveInfinity;
			lastDashTime = float.PositiveInfinity;
			lastPassInputTime = float.PositiveInfinity;
		}

		public override string[] capturableEvents => new string[] { "hit", "slide" };
		public override void onEvent(string eventName, object eventData)
		{
			switch (eventName)
			{
				case "hit":
					hitData = eventData as HitData;
					break;
				case "slide":
					groundSlide += (Vector2)eventData;
					break;
			}
		}

		public override void onUpdate()
		{
			updateTimers();

			updateInputs();
			updateCollisions();
			updateHurt();

			updateWallFacing();

			updateDash();
			updateJump();

			updatePass();
			updateGroundBreak();

			controller.animator.SetBool("isGrounded", isGrounded);
			controller.animator.SetBool("isFalling", isFalling);
			controller.animator.SetBool("isMoving", isMoving);
			controller.animator.SetBool("isWallHolding", lastWallHoldingTime == 0);
		}

		public override bool onFixedUpdate()
		{
			if (!isDashing)
			{
				updateRun();
				updateFall();

				if (lastWallHoldingTime == 0)
					updateWallSlide();
			}

			return true;
		}


		private void updateTimers()
		{
			lastTurnTime += Time.deltaTime;
			lastGroundedTime += Time.deltaTime;
			lastHurtTime += Time.deltaTime;
			lastWallHoldingTime += Time.deltaTime;
			lastJumpInputTime += Time.deltaTime;
			lastWallJumpTime += Time.deltaTime;
			lastDashInputTime += Time.deltaTime;
			lastDashTime += Time.deltaTime;
			lastPassInputTime += Time.deltaTime;

			hurtCooldown -= Time.deltaTime;
			jumpCooldown -= Time.deltaTime;
			dashCooldown -= Time.deltaTime;
		}

		private void flip()
		{
			lastTurnTime = 0;
			lastTurnFrame = controller.currentFixedUpdate;

			isFacingRight = !isFacingRight;
			transform.Rotate(0, 180, 0);
		}
		private void updateInputs()
		{
			moveInput = new Vector2();

			if (Input.GetKey(KeyCode.RightArrow)) moveInput.x += 1;
			if (Input.GetKey(KeyCode.LeftArrow)) moveInput.x -= 1;
			if (Input.GetKey(KeyCode.UpArrow)) moveInput.y += 1;
			if (Input.GetKey(KeyCode.DownArrow)) moveInput.y -= 1;

			isMoving = moveInput.x != 0;

			if (!isDashing && !isDistressed && !isAttacking && !isInCombo)
				if ((moveInput.x > 0 && !isFacingRight) || (moveInput.x < 0 && isFacingRight))
					flip();

			if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Z))
				lastJumpInputTime = 0;

			jumpCutInput = !Input.GetKey(KeyCode.Space) && !Input.GetKey(KeyCode.Z);

			if (Input.GetKeyDown(KeyCode.C))
				lastDashInputTime = 0;

			dashCutInput = !Input.GetKey(KeyCode.C);

			if (Input.GetKeyDown(KeyCode.DownArrow))
				lastPassInputTime = 0;
		}

		private bool isGroundedFalsePositive()
		{
			// just jumped
			if (isJumping && !isFalling)
			{
				// wait for hitbox position update
				if (!isOnSlope || lastJumpFrame >= controller.currentFixedUpdate - 1)
					return true;

				// is not running up a slope after jumping
				if (!controller.rigidBody.IsTouchingLayers(currentGroundLayers & ~data.platformPassing.layers))
					return true;
			}

			// is within passing platform
			if (!passingLayersDisabled && isPassing)
			{
				// was wall holding last frame
				if (lastWallHoldingTime == Time.deltaTime)
					return false;

				// is falling
				if (controller.rigidBody.velocity.y <= -minVerticalMovementVelocity && isJumping && !isSlopeGrounded)
					return true;
			}

			return false;
		}
		private void updateCollisions()
		{
			isGrounded = groundCheck.IsTouchingLayers(currentGroundLayers);
			isSlopeGrounded = slopeCheck.IsTouchingLayers(currentGroundLayers & ~data.platformPassing.layers);
			isOnSlope = slopeCheck.IsTouchingLayers(data.run.slopeLayer);
			isFacingWall = wallCheck.IsTouchingLayers(data.wall.layers);
			isPassing = passingCheck.IsTouchingLayers(data.platformPassing.layers);
			canWallJump = withinCheck.IsTouchingLayers(data.wall.canJumpLayer);
			canGroundDrop = withinCheck.IsTouchingLayers(data.detection.canDropLayer);
			canTakeGroundDamage = groundCheck.IsTouchingLayers(data.detection.canDamageLayer);

			// disable registering wall collision immediately after turning because wallCheck's hitbox
			// needs time to get updated
			if (lastTurnFrame >= controller.currentFixedUpdate - 1)
				isFacingWall = false;

			if ((isGrounded || isSlopeGrounded) && isGroundedFalsePositive())
			{
				isGrounded = false;
				isSlopeGrounded = false;
			}

			if (isFacingWall)
			{
				moveInput.x = 0;
				isMoving = false;
			}
		}

		private bool canTriggerGroundDamage()
		{
			return canTakeGroundDamage && isGrounded && !isInvulnerable;
		}
		private void takeGroundDamage()
		{
			hitData = new HitData();
			hitData.isVertical = true;
			hitData.right = transform.right;
		}
		private bool canHurt()
		{
			return hitData != null && hurtCooldown <= 0;
		}
		private void setDistressDirection()
		{
			if (hitData.isVertical)
				return;

			// if attack faces the same direction
			if (Vector2.Dot(transform.right, hitData.right) > 0)
				flip();
		}
		private void setInvulnerability(bool val)
		{
			isInvulnerable = val;

			int layer = (int)(val ? RuntimeSettings.Layer.PlayerInvulnerable : RuntimeSettings.Layer.Player);

			foreach (Transform child in controller.hitbox)
				child.gameObject.layer = layer;

			controller.hitbox.gameObject.layer = layer;
		}
		private void hurt()
		{
			isDistressed = true;
			lastHurtTime = 0;
			hurtCooldown = data.hurt.invulnerabilityTime;

			setDistressDirection();
			setInvulnerability(true);
			StartCoroutine(Effects.instance.flashing.run(
				controller.spriteRenderer, data.hurt.invulnerabilityTime, burst: true));

			float force = data.hurt.knockbackForce;
			if (force > controller.rigidBody.velocity.y)
			{
				force -= controller.rigidBody.velocity.y;
				controller.rigidBody.AddForce(force * Vector2.up, ForceMode2D.Impulse);
			}

			flip();
			controller.onEvent("hurt", null);
		}
		private void updateHurt()
		{
			if (canTriggerGroundDamage())
				takeGroundDamage();

			if (canHurt())
				hurt();
			hitData = null;

			if (hurtCooldown <= 0)
				setInvulnerability(false);

			if (isDistressed && lastHurtTime >= data.hurt.distressTime)
			{
				flip();
				isDistressed = false;
				controller.onEvent("recover", null);
			}

			if (isDistressed)
			{
				if (isDashing)
					controller.onEvent("dashFinish", null);
				isDashing = false;
				isJumping = false;
				isGrounded = false;

				if (isFacingWall)
					flip();

				moveInput.y = 0;
				moveInput.x = isFacingRight ? 1 : -1;
			}
		}

		private void updateWallFacing()
		{
			if (!isGrounded && !isDistressed && isFacingWall && canWallJump)
				lastWallHoldingTime = 0;

			if (isFacingWall)
				isLastFacedWallRight = isFacingRight;
		}

		private bool canDash()
		{
			return dashCooldown <= 0 && lastDashInputTime <= data.dash.inputBufferTime && dashesLeft > 0
				&& !isDistressed && !isAttacking && !isFacingWall;
		}
		private void dash()
		{
			isDashing = true;
			lastDashTime = 0;
			lastDashInputTime = float.PositiveInfinity;
			dashCooldown = data.dash.time + data.dash.cooldown;

			dashesLeft--;

			float forceDirection = isFacingRight ? 1.0f : -1.0f;

			float force = data.dash.force;
			if (Mathf.Sign(forceDirection) == Mathf.Sign(controller.rigidBody.velocity.x))
				force -= Mathf.Abs(controller.rigidBody.velocity.x);

			if (force > 0)
			{
				controller.rigidBody.AddForce(force * forceDirection * Vector2.right, ForceMode2D.Impulse);
			}

			controller.rigidBody.AddForce(-controller.rigidBody.velocity.y * Vector2.up, ForceMode2D.Impulse);
		}
		private void updateDash()
		{
			if (canDash())
			{
				dash();
				controller.onEvent("dashBegin", null);
			}

			if (isDashing)
				if (dashCutInput || lastDashTime >= data.dash.time
					|| isFacingWall || Mathf.Abs(controller.rigidBody.velocity.y) > minVerticalMovementVelocity)
				{
					isDashing = false;
					controller.onEvent("dashFinish", null);
				}

			if (isDashing)
			{
				isFalling = false;
				isGrounded = false;
			}

			if (isGrounded || lastWallHoldingTime == 0)
			{
				dashesLeft = data.dash.dashesCount;
			}
		}

		private bool canJump()
		{
			return jumpCooldown <= 0 && lastJumpInputTime <= data.jump.inputBufferTime
				&& jumpsLeft > 0 && lastWallJumpTime >= data.wall.jumpTime
				&& !isDistressed;
		}
		private bool canJumpCut()
		{
			return jumpCutInput && currentJumpCuttable && !isFalling && !isDistressed;
		}
		private void jump()
		{
			isJumping = true;
			isFalling = false;
			isGrounded = false;
			lastJumpInputTime = float.PositiveInfinity;
			jumpCooldown = data.jump.cooldown;

			currentJumpCuttable = !registeredDownHitJump;
			lastJumpFrame = controller.currentFixedUpdate;

			float force = data.jump.force;
			if (registeredDownHitJump)
				if (registeredDownHitHighJump)
					force = data.attack.attackDownHighBounceForce;
				else
					force = data.attack.attackDownBounceForce;

			if (force > controller.rigidBody.velocity.y)
			{
				force -= controller.rigidBody.velocity.y;
				controller.rigidBody.AddForce(force * Vector2.up, ForceMode2D.Impulse);
			}

			if (lastWallHoldingTime < data.wall.jumpCoyoteTime)
			{
				lastWallHoldingTime = float.PositiveInfinity;
				lastWallJumpTime = 0;

				if (isLastFacedWallRight == isFacingRight)
					flip();

				float forceDirection = isFacingRight ? 1.0f : -1.0f;

				float wallJumpForce = data.wall.jumpForce;
				controller.rigidBody.AddForce(wallJumpForce * forceDirection * Vector2.right, ForceMode2D.Impulse);
			}

			controller.onEvent("jumped", null);
		}
		private void updateGravityScale()
		{
			if (isDashing)
			{
				controller.rigidBody.gravityScale = 0;
			}
			else if (lastWallHoldingTime == 0)
			{
				controller.rigidBody.gravityScale = 0;
			}
			else if (isJumping && canJumpCut())
			{
				controller.rigidBody.gravityScale = data.gravity.scale * data.jump.jumpCutGravityMultiplier;
			}
			else if (isJumping && Mathf.Abs(controller.rigidBody.velocity.y) < data.jump.hangingVelocityThreshold)
			{
				controller.rigidBody.gravityScale = data.gravity.scale * data.jump.hangingGravityMultiplier;
			}
			else if (isOnSlope && isSlopeGrounded)
			{
				controller.rigidBody.gravityScale = 0;
			}
			else if (isFalling)
			{
				controller.rigidBody.gravityScale = data.gravity.scale * data.gravity.fallMultiplier;
			}
			else
				controller.rigidBody.gravityScale = data.gravity.scale;
		}
		private void updateJump()
		{
			if (!isGrounded && lastWallHoldingTime != 0
				&& (controller.rigidBody.velocity.y <= -minVerticalMovementVelocity || isSlopeGrounded))
			{
				isFalling = true;
			}
			if (isFalling && !isJumping && jumpsLeft == data.jump.jumpsCount
				&& lastGroundedTime >= data.jump.coyoteTime)
			{
				jumpsLeft--;
			}

			if (canJump() || registeredDownHitJump)
			{
				jump();
				if (!registeredDownHitJump)
					jumpsLeft--;
				registeredDownHitHighJump = false;
				registeredDownHitJump = false;
			}

			updateGravityScale();

			if ((isGrounded || lastWallHoldingTime == 0) && lastWallJumpTime > data.wall.jumpMinTime)
			{
				if (isFalling && lastWallHoldingTime != 0)
					controller.onEvent("fell", null);

				jumpsLeft = data.jump.jumpsCount;
				isJumping = false;
				isFalling = false;
				lastGroundedTime = 0;
				lastWallJumpTime = float.PositiveInfinity;
			}

			if (controller.rigidBody.velocity.y < -data.gravity.maxFallSpeed)
			{
				controller.rigidBody.AddForce(
					(data.gravity.maxFallSpeed + controller.rigidBody.velocity.y) * Vector2.down,
					ForceMode2D.Impulse);
			}
		}

		private bool canPass()
		{
			return lastPassInputTime <= data.platformPassing.inputBufferTime && isPassing
				&& isGrounded && !isDistressed;
		}
		private void setPassable(bool val)
		{
			passingLayersDisabled = val;
			int mask = Physics2D.GetLayerCollisionMask((int)RuntimeSettings.Layer.Player);
			int maskInv = Physics2D.GetLayerCollisionMask((int)RuntimeSettings.Layer.PlayerInvulnerable);

			if (!passingLayersDisabled)
			{
				mask |= data.platformPassing.layers;
				maskInv |= data.platformPassing.layers;
				currentGroundLayers |= (data.run.groundLayers & data.platformPassing.layers);
			}
			else
			{
				mask &= ~data.platformPassing.layers;
				maskInv &= ~data.platformPassing.layers;
				currentGroundLayers &= ~data.platformPassing.layers;
			}

			Physics2D.SetLayerCollisionMask((int)RuntimeSettings.Layer.Player, mask);
			Physics2D.SetLayerCollisionMask((int)RuntimeSettings.Layer.PlayerInvulnerable, maskInv);
		}
		private void updatePass()
		{
			if (canPass())
			{
				isGrounded = false;

				setPassable(true);
			}

			if (passingLayersDisabled && !isPassing)
				setPassable(false);
		}

		private bool canTriggerGroundDrop()
		{
			return canGroundDrop && isGrounded;
		}
		private void triggerGroundBreak()
		{
			ContactFilter2D filter = new ContactFilter2D().NoFilter();
			filter.SetLayerMask(data.detection.canDropLayer);
			filter.useLayerMask = true;

			List<Collider2D> contacts = new List<Collider2D>();
			if (controller.rigidBody.OverlapCollider(filter, contacts) == 0)
				return;

			foreach (Collider2D contact in contacts)
			{
				GroundDroppingController controller = contact.GetComponent<GroundDroppingController>();

				controller.triggerDrop(groundCheck.bounds);
			}
		}
		private void updateGroundBreak()
		{
			if (canTriggerGroundDrop())
			{
				triggerGroundBreak();
			}
		}

		private void updateRun()
		{
			float targetSpeed = moveInput.x * data.run.maxSpeed;

			if (lastWallJumpTime < data.wall.jumpTime)
				targetSpeed = Mathf.Lerp(targetSpeed, controller.rigidBody.velocity.x, data.wall.jumpInputReduction);

			if (isDistressed)
				targetSpeed = moveInput.x * data.hurt.knockbackMaxSpeed;

			float accelRate = targetSpeed == 0 ? data.run.decelerationForce : data.run.accelerationForce;

			if (isJumping && Mathf.Abs(controller.rigidBody.velocity.y) < data.jump.hangingVelocityThreshold)
			{
				targetSpeed *= data.jump.hangingSpeedMultiplier;
				accelRate *= data.jump.hangingSpeedMultiplier;
			}

			if (isAttacking && !lastAttackDown && isGrounded && !isDashing)
			{
				targetSpeed = data.attack.forwardSpeed * (isFacingRight ? 1 : -1);
				accelRate = data.attack.forwardAcceleration;
			}

			if (!isGrounded)
				groundSlide = Vector2.zero;

			float speedDif = targetSpeed - (controller.rigidBody.velocity.x - groundSlide.x / Time.fixedDeltaTime);
			float movement = speedDif * accelRate;

			if (isGrounded && targetSpeed != 0f)
				controller.onEvent("walked", null);

			controller.rigidBody.AddForce(movement * Vector2.right, ForceMode2D.Force);

			groundSlide = Vector2.zero;
		}

		private void updateFall()
		{
			if (isGrounded && controller.rigidBody.velocity.y > 0)
				controller.rigidBody.AddForce(
					data.run.groundStickiness * -controller.rigidBody.velocity.y * Vector2.up,
					ForceMode2D.Force);

			if (isFalling && isAttacking && lastAttackDown)
				controller.rigidBody.AddForce(
					data.attack.attackDownSlowdown * -controller.rigidBody.velocity.y * Vector2.up,
					ForceMode2D.Force);

		}

		private void updateWallSlide()
		{
			float targetSpeed = Mathf.Min(moveInput.y, 0f) * data.wall.slideMaxSpeed;
			float accelRate = targetSpeed == 0 ? data.wall.slideDecelerationForce
				: data.wall.slideAccelerationForce;

			float speedDif = targetSpeed - controller.rigidBody.velocity.y;
			float movement = speedDif * accelRate;

			controller.rigidBody.AddForce(movement * Vector2.up, ForceMode2D.Force);
		}
	}
}