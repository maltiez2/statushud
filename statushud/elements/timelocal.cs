using System;

namespace StatusHud {
	public class StatusHudTimeLocalElement: StatusHudTimeElement {
		public new const string name = "time-local";
		public new const string desc = "The 'time-local' element displays the system's local time.";
		protected new const string textKey = "shud-timelocal";

		public StatusHudTimeLocalElement(StatusHudSystem system, int slot, StatusHudTextConfig config) : base(system, slot, config) {
			this.textureId = this.system.textures.timeLocal.TextureId;
		}

		public override string getTextKey() {
			return textKey;
		}

		public override void Tick() {
			this.renderer.setText(DateTime.Now.ToString("HH:mm"));
		}
	}
}