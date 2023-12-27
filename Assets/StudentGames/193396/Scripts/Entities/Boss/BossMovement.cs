using UnityEngine;

namespace _193396
{
	[RequireComponent(typeof(BossInfo))]
	public class BossMovement : EntityBehavior
	{
		public Collider2D bossRoomCheck;
		public Collider2D bossRoomLowerCheck;

		private BossInfo info;
		private BossData data => info.data;
		[field: Space(10)]
		[field: SerializeField, ReadOnly] public bool isFacingRight { get; private set; }

		public override void onAwake()
		{
			info = controller.getBehavior<BossInfo>();
		}
		public override void onStart()
		{
			isFacingRight = controller.transform.right.x > 0;
		}

		public override string[] capturableEvents => new string[] {  };
		public override void onEvent(string eventName, object eventData)
		{
			
		}

		public override void onUpdate()
		{
			
		}

		public override bool onFixedUpdate()
		{
			return true;
		}

	}
}