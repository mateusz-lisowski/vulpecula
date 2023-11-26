using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct EntityMessage
{
	public EntityMessage(string name_, object data_)
	{
		name = name_;
		data = data_;
	}

	public string name;
	public object data;
}

public abstract class EntityBehavior : MonoBehaviour
{
	public void addSmoothForce(float targetSpeed, float accelerationCoefficient, Vector2 direction)
	{
		float speedDif = targetSpeed - Vector2.Dot(controller.rigidBody.velocity, direction);
		float movement = speedDif * accelerationCoefficient / Time.fixedDeltaTime;

		controller.rigidBody.AddForce(movement * direction, ForceMode2D.Force);
	}

	public virtual void onAwake() { }
	public virtual void onEvent(string eventName, object eventData) { }
	public virtual void onUpdate() { }
	public virtual bool onFixedUpdate() { return false; }

	public void setController(EntityBehaviorController parent)
	{
		controller = parent;
	}
	protected EntityBehaviorController controller { get; private set; }

	public void disableCurrentFixedUpdate()
	{
		lastFixedUpdateDisabled = controller.currentFixedUpdate;
	}
	public bool currentFixedUpdateDisabled()
	{
		return lastFixedUpdateDisabled == controller.currentFixedUpdate;
	}
	private int lastFixedUpdateDisabled = -1;
}

public class EntityBehaviorController : MonoBehaviour
{
	private List<EntityBehavior> behaviors;

	public Rigidbody2D rigidBody { get; private set; }
	public Animator animator { get; private set; }
	public SpriteRenderer spriteRenderer { get; private set; }
	public Transform hitbox { get; private set; }

	public int currentUpdate { get; private set; }
	public int currentFixedUpdate { get; private set; }


	public B getBehavior<B>() where B : EntityBehavior
	{
		return (B)behaviors.Find(b => b.GetType() == typeof(B));
	}
	public List<B> getBehaviors<B>() where B : EntityBehavior
	{
		return behaviors.FindAll(b => b.GetType() == typeof(B)).Cast<B>().ToList();
	}

	private void Awake()
	{
		rigidBody = transform.GetComponent<Rigidbody2D>();
		animator = transform.Find("Sprite").GetComponent<Animator>();
		spriteRenderer = transform.Find("Sprite").GetComponent<SpriteRenderer>();
		hitbox = transform.Find("Hitbox")?.GetComponent<Transform>();

		currentUpdate = 0;
		currentFixedUpdate = 0;

		behaviors = new List<EntityBehavior>();

		foreach (var behavior in transform.GetComponents<EntityBehavior>())
		{
			behavior.setController(this);

			behaviors.Add(behavior);
		}

		foreach (var behavior in behaviors)
			behavior.onAwake();
	}
	private void Update()
	{
		currentUpdate++;

		foreach (var behavior in behaviors)
			behavior.onUpdate();
	}
	private void FixedUpdate()
	{
		currentFixedUpdate++;

		foreach (var behavior in behaviors)
			if (!behavior.currentFixedUpdateDisabled())
				if (behavior.onFixedUpdate())
					break;
	}

	public void onEvent(string eventName, object eventData)
	{
		if (eventName == "destroy")
		{
			Destroy(gameObject);
			return;
		}

		foreach (var behavior in behaviors)
			behavior.onEvent(eventName, eventData);
	}
	public void onMessage(EntityMessage msg)
	{
		onEvent(msg.name, msg.data);
	}
}
