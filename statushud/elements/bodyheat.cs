using System;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace StatusHud
{
    public class StatusHudBodyheatElement : StatusHudElement
    {
        public new const string name = "bodyheat";
        public new const string desc = "The 'bodyheat' element displays the player's body heat (in %). If at maximum, it is hidden.";
        protected const string textKey = "shud-bodyheat";

        public override string Name => name;

        protected const float cfratio = (9f / 5f);

        public const float tempIdeal = 37;

        public bool active;
        public int textureId;

        protected StatusHudBodyheatRenderer renderer;
        protected StatusHudConfig config;


        public StatusHudBodyheatElement(StatusHudSystem system, int slot, StatusHudConfig config) : base(system, slot)
        {
            this.renderer = new StatusHudBodyheatRenderer(this.system, this.slot, this, config.text);
            this.system.capi.Event.RegisterRenderer(this.renderer, EnumRenderStage.Ortho);

            this.textureId = this.system.textures.empty.TextureId;
            this.config = config;

            this.active = false;
        }

        protected override StatusHudRenderer getRenderer()
        {
            return this.renderer;
        }

        public virtual string getTextKey()
        {
            return textKey;
        }

        public override void Tick()
        {
            ITreeAttribute tempTree = this.system.capi.World.Player.Entity.WatchedAttributes.GetTreeAttribute("bodyTemp");

            if (tempTree == null)
            {
                return;
            }

            float temp = tempTree.GetFloat("bodytemp");
            float tempDiff = temp - tempIdeal;

            // Heatstroke doesn't exists yet, only consider cold tempatures
            if (tempDiff <= -0.5f)
            {
                string textRender;
                switch (config.options.temperatureScale)
                {
                    case 'F':
                        textRender = string.Format("{0:N1}", tempDiff * cfratio) + "�F";
                        break;
                    case 'K':
                        textRender = string.Format("{0:N1}", tempDiff) + "�K";
                        break;
                    case 'C':
                    default:
                        textRender = string.Format("{0:N1}", tempDiff) + "�C";
                        break;
                }

                this.active = true;
                this.renderer.setText(textRender);
            }
            else
            {
                if (this.active)
                {
                    this.renderer.setText("");
                }

                this.active = false;
            }
            this.updateTexture(tempDiff);
        }

        public override void Dispose()
        {
            this.renderer.Dispose();
            this.system.capi.Event.UnregisterRenderer(this.renderer, EnumRenderStage.Ortho);
        }

        protected void updateTexture(float tempDiff)
        {
            // If body temp ~33C, the player will start freezing
            if (tempDiff > -4)
            {
                this.textureId = this.system.textures.bodyheat.TextureId;
            }
            else
            {
                this.textureId = this.system.textures.bodyheatCold.TextureId;
            }
        }
    }

    public class StatusHudBodyheatRenderer : StatusHudRenderer
    {
        protected StatusHudBodyheatElement element;

        protected StatusHudText text;

        public StatusHudBodyheatRenderer(StatusHudSystem system, int slot, StatusHudBodyheatElement element, StatusHudTextConfig config) : base(system, slot)
        {
            this.element = element;

            this.text = new StatusHudText(this.system.capi, this.slot, this.element.getTextKey(), config, this.system.textures.size);
        }

        public void setText(string value)
        {
            this.text.Set(value);
        }

        protected override void update()
        {
            base.update();
            this.text.Pos(this.pos);
        }

        protected override void render()
        {
            if (!this.element.active)
            {
                if (this.system.showHidden)
                {
                    this.renderHidden(this.system.textures.bodyheat.TextureId);
                }
                return;
            }

            this.system.capi.Render.RenderTexture(this.element.textureId, this.x, this.y, this.w, this.h);
        }

        public override void Dispose()
        {
            this.text.Dispose();
        }
    }
}