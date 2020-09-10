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
namespace Asteroids
{
    //ship is just a modded version of Asteroids where the physicsObject is public
    internal class Ship : DrawableGameComponent
    {
        private int remainingShields;
        private Model model;
        private Texture2D moonTexture;
        private bool winFlag = false;
        private double remainingFuel;
        private int remainingTorpedos;
        private bool loseFlag = false;
        private bool shielding = false;
        public BEPUphysics.Entities.Prefabs.Sphere physicsObject;
        private Vector3 CurrentPosition
        {
            get
            {
                return MathConverter.Convert(physicsObject.Position);
            }
        }
        public Ship(Game game) : base(game)
        {
            game.Components.Add(this);
            remainingShields = 100;
            remainingFuel = 100;
            remainingTorpedos = 8;
        }
        public Ship(Game game, Vector3 pos, string id, int mass) : this(game)
        {
            physicsObject = new BEPUphysics.Entities.Prefabs.Sphere(ConversionHelper.MathConverter.Convert(pos), 1);
            physicsObject.Mass = mass;
            physicsObject.AngularDamping = 0f;
            physicsObject.LinearDamping = 0f;
            physicsObject.CollisionInformation.Events.InitialCollisionDetected += Events_InitialCollisionDetected;
            physicsObject.Tag = id;
            Game.Services.GetService<Space>().Add(physicsObject);
        }
        private void Events_InitialCollisionDetected(BEPUphysics.BroadPhaseEntries.MobileCollidables.EntityCollidable sender, BEPUphysics.BroadPhaseEntries.Collidable other,
        BEPUphysics.NarrowPhaseSystems.Pairs.CollidablePairHandler pair)
        {
            //if the ship collides with an asteroid, remove 20 shields
            if (pair.EntityA.Tag.ToString() == "Asteroid" || pair.EntityB.Tag.ToString() == "Asteroid")
            {
                if (!shielding)
                {
                    remainingShields -= 20;
                    if (remainingShields <= 0)
                    {
                        loseFlag = true;
                    }
                }
            }
            else if(pair.EntityA.Tag.ToString() == "Goal" || pair.EntityB.Tag.ToString() == "Goal")
            {
                winFlag = true;
            }
        }
        public override void Initialize()
        {
            base.Initialize();
        }
        protected override void LoadContent()
        {
            //since the ship is in first person, I am just reusing the moon model
            moonTexture = Game.Content.Load<Texture2D>("moonsurface");
            model = Game.Content.Load<Model>("moon");
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
                    effect.Alpha = 0.7f;
                    //effect.EnableDefaultLighting();
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
        public int getShields()
        {
            return remainingShields;
        }

        public double getFuel()
        {
            return remainingFuel;
        }

        public int getTorpedos()
        {
            return remainingTorpedos;
        }

        public bool getWin()
        {
            return winFlag;
        }

        public bool getLose()
        {
            return loseFlag;
        }

        public void subtractFuel(double usedFuel)
        {
            remainingFuel -= usedFuel;
            if(remainingFuel <= 0)
            {
                loseFlag = true;
            }
        }

        public void setShielding(bool s)
        {
            shielding = s;
        }

        public bool getShielding()
        {
            return shielding;
        }

        public void useTorpedo()
        {
            remainingTorpedos--;
        }

        public bool torpedoLeft()
        {
            bool left = false;
            if(remainingTorpedos > 0)
            {
                left = true;
            }
            return left;
        }
    }
}