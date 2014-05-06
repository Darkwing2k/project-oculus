using UnityEngine;
using System.Collections;

public class LookWhereYoureGoing : AngularSteering {

	public override void Update(Character self) {
		if (self.Velocity != Vector3.zero) {
			self.LookAt(self.Position + self.Velocity);
		}
	}
}
