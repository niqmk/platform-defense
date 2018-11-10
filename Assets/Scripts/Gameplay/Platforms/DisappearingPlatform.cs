﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisappearingPlatform : Platform {
	public float duration;
	public float wait;

	public override PlatformType type {
		get {
			return PlatformType.Disappearing;
		}
	}

	private BoxCollider2D coll;
	private SpriteRenderer rend;

	private bool started, disappeared;
	private float startTime;
	private bool shouldAppear;

	private ContactFilter2D contactFilter;
	private Collider2D[] overlapResults;

	public void Start() {
		this.coll = this.gameObject.GetComponent<BoxCollider2D>();
		this.rend = this.gameObject.GetComponent<SpriteRenderer>();


		this.contactFilter = new ContactFilter2D();
		this.contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(this.gameObject.layer));
		this.contactFilter.useLayerMask = true;
		this.contactFilter.useTriggers = false;

		this.overlapResults = new Collider2D[16];
	}

	public override void Contact(Actor actor, RaycastHit2D hit, bool vertical) {
		Disappear();
	}

	public void Update() {
		if(!this.started) {
			return;
		}

		if(!this.shouldAppear) {
			if(Time.time - this.startTime < this.duration) {
				Color c = this.rend.color;
				c.a = 1f - (Time.time - this.startTime) / this.duration;
				this.rend.color = c;
			}
			
			if(!this.disappeared && Time.time - this.startTime > this.duration) {
				this.coll.enabled = false;
				this.rend.enabled = false;
				this.disappeared = true;
			} else if(this.disappeared && Time.time - this.startTime > this.duration + this.wait) {
				this.shouldAppear = true;
			}
		}

		if(this.shouldAppear) {
			int count = Physics2D.OverlapBoxNonAlloc(this.transform.position, this.coll.size, 0, this.overlapResults);
			bool doAppear = true;
			for(int i = 0; i < count; i++) {
				if(this.overlapResults[i].gameObject.tag == "Actor") {
					doAppear = false;
				}
			}

			if(doAppear) {
				this.coll.enabled = true;
				this.rend.enabled = true;
				this.disappeared = false;
				this.started = false;

				Color c = this.rend.color;
				c.a = 1f;
				this.rend.color = c;

				this.shouldAppear = false;
			}
		}
	}

	private void Disappear() {
		if(this.started) {
			return;
		}

		this.started = true;
		this.startTime = Time.time;
	}
}