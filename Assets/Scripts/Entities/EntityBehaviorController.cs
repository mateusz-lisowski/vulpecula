using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;

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
	public abstract void onAwake();
	public abstract void onEvent(string eventName, object eventData);
	public abstract void onUpdate();
	public abstract void onFixedUpdate();

	public void setController(EntityBehaviorController parent)
	{
		controller = parent;
	}
	protected EntityBehaviorController controller { get; private set; }
}

public class EntityBehaviorController : MonoBehaviour
{
	private List<EntityBehavior> behaviors;

	public Rigidbody2D rigidBody { get; private set; }
	public Animator animator { get; private set; }
	public SpriteRenderer spriteRenderer { get; private set; }
	public Transform hitbox { get; private set; }

	public int currentFrame { get; private set; }


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
		hitbox = transform.Find("Hitbox").GetComponent<Transform>();

		currentFrame = 0;

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
		foreach (var behavior in behaviors)
			behavior.onUpdate();
	}
	private void FixedUpdate()
	{
		currentFrame++;

		foreach (var behavior in behaviors)
			behavior.onFixedUpdate();
	}

	public void onEvent(string eventName, object eventData)
	{
		foreach (var behavior in behaviors)
			behavior.onEvent(eventName, eventData);
	}
	public void onMessage(EntityMessage msg)
	{
		onEvent(msg.name, msg.data);
	}
}
