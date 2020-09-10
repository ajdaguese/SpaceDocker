using BEPUphysics;
using SpaceDocker;
using ConversionHelper;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SpaceDocker
{
    internal class Torpedo : DrawableGameComponent
    {
        private Model model;
        private Texture2D TorpedoTexture;
        public BEPUphysics.Entities.Prefabs.Sphere physicsObject;
        private Vector3 CurrentPosition
        {
            get
            {
                return MathConverter.Convert(physicsObject.Position);
            }
        }
        public Torpedo(Game game) : base(game)
        {
            //game.Components.Add(this);
        }
        public Torpedo(Game game, Vector3 pos, string id) : this(game)
        {
            physicsObject = new BEPUphysics.Entities.Prefabs.Sphere(ConversionHelper.MathConverter.Convert(pos), 5);
            physicsObject.AngularDamping = 0f;
            physicsObject.LinearDamping = 0f;
            physicsObject.CollisionInformation.Events.InitialCollisionDetected += Events_InitialCollisionDetected;
            physicsObject.Tag = id;
            game.Components.Add(this);
            Game.Services.GetService<Space>().Add(physicsObject);
        }
        private void Events_InitialCollisionDetected(BEPUphysics.BroadPhaseEntries.MobileCollidables.EntityCollidable sender, BEPUphysics.BroadPhaseEntries.Collidable other,
        BEPUphysics.NarrowPhaseSystems.Pairs.CollidablePairHandler pair)
        {
            //colliding with your own ship is no big deal, but colliding with anything else destroys the torpedo
            if (pair.EntityA.Tag.ToString() == "Ship" || pair.EntityB.Tag.ToString() == "Ship")
            {

            }
            else
            {
                Game.Services.GetService<Space>().Remove(physicsObject);
                Game.Components.Remove(this);
            }
            
        }
        public Torpedo(Game game, Vector3 pos, string id, float mass) : this(game, pos, id)
        {
            physicsObject.Mass = mass;
        }
        public Torpedo(Game game, Vector3 pos, string id, float mass, Vector3 linMomentum) : this(game, pos, id, mass)
        {
            physicsObject.LinearMomentum = ConversionHelper.MathConverter.Convert(linMomentum);
        }
        public Torpedo(Game game, Vector3 pos, string id, float mass, Vector3 linMomentum, Vector3 angMomentum) : this(game, pos, id, mass, linMomentum)
        {
            physicsObject.AngularMomentum = ConversionHelper.MathConverter.Convert(angMomentum);
        }
        public override void Initialize()
        {
            base.Initialize();
        }
        protected override void LoadContent()
        {
            TorpedoTexture = Game.Content.Load<Texture2D>("torpedoSurface");
            model = Game.Content.Load<Model>("torpedo");
            physicsObject.Radius = model.Meshes[0].BoundingSphere.Radius;
            base.LoadContent();
        }
        protected override void UnloadContent()
        {
            base.UnloadContent();
        }
        public override void Draw(GameTime gameTime)
        {
            foreach (var mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.Alpha = 1f;
                    //effect.EnableDefaultLighting();
                    effect.Texture = TorpedoTexture;
                    effect.TextureEnabled = true;
                    effect.PreferPerPixelLighting = true;
                    effect.World = ConversionHelper.MathConverter.Convert(physicsObject.WorldTransform);
                    effect.View = Matrix.CreateLookAt(SpDoc.CameraPosition, SpDoc.CameraDirection, Vector3.Up);
                    float aspectRatio = Game.GraphicsDevice.Viewport.AspectRatio;
                    float fieldOfView = Microsoft.Xna.Framework.MathHelper.PiOver4;
                    float nearClipPlane = 1;
                    float farClipPlane = 200;
                    effect.Projection = Matrix.CreatePerspectiveFieldOfView(fieldOfView, aspectRatio, nearClipPlane, farClipPlane);
                }
                mesh.Draw();
            }
            base.Draw(gameTime);
        }
    }
}