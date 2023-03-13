﻿using UnityEngine;
using System.Collections;
using System.Linq;
using Werewolf.StatusIndicators.Services;
using System.Collections.Generic;

namespace Werewolf.StatusIndicators.Components {
	public class Cone : SpellIndicator {

        public List<GameObject> collectionList = new List<GameObject>();

        // Constants

        public const float CONE_ANIM_SPEED = 0.15f;

		// Fields

		public Projector LBorder, RBorder;

		// Properties

		public override ScalingType Scaling { get { return ScalingType.LengthAndHeight; } }

		[SerializeField]
		[Range(0, 360)]
		private float angle;

		public float Angle {
			get { return angle; }
			set {
				this.angle = value;
				SetAngle(value); 
			}
		}

        // Methods
        private void OnCollisionEnter(Collision collision)
        {
            Debug.Log("collision detected");
            collectionList.Add(collision.gameObject);
        }

        public override void Update() {
			if(Manager != null) {
                //this.transform.rotation.z = this.transform.parent.transform.parent.GetChild(0).rotation.y;
				Manager.transform.rotation = Quaternion.LookRotation(FlattenVector(Get3DMousePosition()) - Manager.transform.position);
			}
		}

		public override void OnValueChanged() {
			base.OnValueChanged();
			SetAngle(angle);
		}

		public override void OnShow() {
			base.OnShow();
			//StartCoroutine(FadeIn());
		}

		private void SetAngle(float angle) {
			SetShaderFloat("_Expand", Normalize.GetValue(angle - 1, 360));
			LBorder.transform.localEulerAngles = new Vector3(0, 0, (angle + 2) / 2);
			RBorder.transform.localEulerAngles = new Vector3(0, 0, (-angle + 2) / 2);
		}

		/// <summary>
		/// Optional animation when Cone is made visible.
		/// </summary>
		private IEnumerator FadeIn() {
			float final = angle;
			float current = 0;

			foreach(Projector p in Projectors)
				p.enabled = true;

			while(current < final) {
				SetAngle(current);
				current += final * CONE_ANIM_SPEED;
				yield return null;
			}
			SetAngle(final);
			yield return null;
		}
	}
}