using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace SpaceDocker
{
    /// <summary>
    /// Handles all of the aspects of working with a skybox.
    /// </summary>
    public class Sky
    {
        private GraphicsDevice graphic;
        private ContentManager con;
        private Model model;
        private Texture2D texture;

        public Sky(GraphicsDevice g, ContentManager c)
        {
            graphic = g;
            con = c;
            
        }

        public void LoadContent()
        {
            model = con.Load<Model>("ssphere");
        }

        /// <summary>
        /// Helper for drawing the skydome mesh.
        /// </summary>
        public void Draw(Vector3 camPos, Vector3 camDir)
        {
            graphic.BlendState = BlendState.Opaque;
            graphic.RasterizerState = RasterizerState.CullNone;
            graphic.DepthStencilState = DepthStencilState.None;
            graphic.SamplerStates[0] = SamplerState.LinearWrap;
            Matrix view = Matrix.CreateLookAt(camPos, camDir, Vector3.Up);
            view.Translation = Vector3.Zero;

            foreach(ModelMesh mesh in model.Meshes)
            {
                foreach(BasicEffect effect in mesh.Effects)
                {
                    effect.View = view;
                    effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, graphic.Viewport.AspectRatio, 0.1f, 2000);
                    effect.TextureEnabled = true;
                }
                mesh.Draw();
            }
            graphic.RasterizerState = RasterizerState.CullCounterClockwise;
        }
    }
}