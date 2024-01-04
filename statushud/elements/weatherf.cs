using System;
using Vintagestory.API.Common;

namespace StatusHud {
	public class StatusHudWeatherfElement: StatusHudWeatherElement {
		public new const string name = "weather-f";
		public new const string desc = "The 'weather-f' element displays the current temperature (in °F) and an icon for the current condition.";
		protected new const string textKey = "shud-weatherf";

		protected const float cfratio = (9f / 5f);
		protected const float cfdiff = 32;

		public StatusHudWeatherfElement(StatusHudSystem system, int slot, StatusHudTextConfig config) : base(system, slot, config) {}

		public override string getTextKey() {
			return textKey;
		}

		public override void Tick() {
			ClimateCondition cc = this.system.capi.World.BlockAccessor.GetClimateAt(this.system.capi.World.Player.Entity.Pos.AsBlockPos, EnumGetClimateMode.NowValues);
			this.renderer.setText((int)Math.Round((cc.Temperature * cfratio) + cfdiff, 0) + "°F");

			this.updateTexture(cc);
		}
	}
}