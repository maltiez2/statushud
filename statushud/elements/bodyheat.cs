using System;
using Vintagestory.API.Client;
using Vintagestory.API.Datastructures;

namespace StatusHud {
	public class StatusHudBodyheatElement: StatusHudElement {
		public new const string name = "bodyheat";
		public new const string desc = "The 'bodyheat' element displays the player's body heat (in %). If at maximum, it is hidden.";
		protected const string textKey = "shud-bodyheat";

		// Hard-coded in EntityBehaviorBodyTemperature.
		public const float tempMin = 31;
		public const float tempMax = 45;
		public const float tempDivisor = tempMax - tempMin;

		public bool active;

		protected StatusHudBodyheatRenderer renderer;

		public StatusHudBodyheatElement(StatusHudSystem system, int slot, StatusHudTextConfig config): base(system, slot) {
			this.renderer = new StatusHudBodyheatRenderer(this.system, this.slot, this, config);
			this.system.capi.Event.RegisterRenderer(this.renderer, EnumRenderStage.Ortho);

			this.active = false;
		}

		protected override StatusHudRenderer getRenderer() {
			return this.renderer;
		}

		public virtual string getTextKey() {
			return textKey;
		}

		public override void Tick() {
			ITreeAttribute tempTree = this.system.capi.World.Player.Entity.WatchedAttributes.GetTreeAttribute("bodyTemp");

			if(tempTree == null) {
				return;
			}

			float temp = tempTree.GetFloat("bodytemp");

			if(temp < tempMax) {
				this.renderer.setText((int)Math.Round(((temp - tempMin) / tempDivisor) * 100, 0) + "%");
				this.active = true;
			} else {
				if(this.active) {
					this.renderer.setText("");
				}
				this.active = false;
			}
		}

		public override void Dispose() {
			this.renderer.Dispose();
			this.system.capi.Event.UnregisterRenderer(this.renderer, EnumRenderStage.Ortho);
		}
	}

	public class StatusHudBodyheatRenderer: StatusHudRenderer {
		protected StatusHudBodyheatElement element;

		protected StatusHudText text;

		public StatusHudBodyheatRenderer(StatusHudSystem system, int slot, StatusHudBodyheatElement element, StatusHudTextConfig config): base(system, slot) {
			this.element = element;

			this.text = new StatusHudText(this.system.capi, this.slot, this.element.getTextKey(), config, this.system.textures.size);
		}

		public void setText(string value) {
			this.text.Set(value);
		}

		protected override void update() {
			base.update();
			this.text.Pos(this.pos);
		}

		protected override void render() {
			if(!this.element.active) {
				if(this.system.showHidden) {
					this.renderHidden(this.system.textures.bodyheat.TextureId);
				}
				return;
			}

			this.system.capi.Render.RenderTexture(this.system.textures.bodyheat.TextureId, this.x, this.y, this.w, this.h);
		}

		public override void Dispose() {
			this.text.Dispose();
		}
	}
}