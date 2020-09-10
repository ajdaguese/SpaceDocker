using Asteroids;
using BEPUphysics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace SpaceDocker
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class SpDoc : Game
    {
        GraphicsDeviceManager graphics;
        //spriteBatch for displaying text
        SpriteBatch spriteBatch;
        //The object for the ship
        Ship ship;
        //a count for spawning asteroids. Every 50 frames, a new asteroid is spawned
        private int countDown = 0;
        //this is to keep track of the number of frames since a torpedo has been fired, otherwise holding 'F' too long accidentally would fire all of your torpedos instantly
        private int torpCount = 0;
        private Random rnd = new Random();
        private Sky box;
        public static Vector3 CameraPosition
        {
            get;
            private set;
        }
        public static Vector3 CameraDirection
        {
            get;
            private set;
        }
        //A flag that is set to true when the goal object detects collision
        //the font and color for win text
        SpriteFont dfont;
        Color textColor = Color.White;
        public SpDoc()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }
        /// <summary>
        /// Allows the game to perform any initialization it needs to before 
      //  starting to run.
/// This is where it can query for any required services and load any non-
//graphic
/// related content.  Calling base.Initialize will enumerate through any 
//components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // Make our BEPU Physics space a service
            Services.AddService<Space>(new Space());
            // Create two asteroids.  Note that asteroids automatically add 
         //   themselves to
            // as a DrawableGameComponent as well as add an object into Bepu 
            //physics
            // that represents the asteroid.
            new Asteroid(this, new Vector3(-2, 0, -5), "Asteroid", 2, new Vector3(0f, 0,0), new Vector3(.3f, .5f, .5f));
          //  new Asteroid(this, new Vector3(2, 0, -5), "Asteroid", 3, new Vector3(0f, 0, 0), new Vector3(.3f, .5f, .5f));
            //places the goal past the asteroids. Hit this and you win
            new Goals.Goal(this, new Vector3(6, 20, -16), "Goal", 500, new Vector3(0, 0, 0), new Vector3(.3f, .5f, .5f));
            //The instantiation of the ship
            ship = new Ship(this, new Vector3(0, 0, 0), "Cam", 1);
           // ship.getWin() = false;
            //starting camera
            CameraPosition = new Vector3(0, 0, 0);
            CameraDirection = Vector3.Forward;
            box = new Sky(this.GraphicsDevice, Content);
            base.Initialize();
        }
        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            dfont = Content.Load<SpriteFont>("posdisplayfont");
            box.LoadContent();
        }
        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
        }
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            Services.GetService<Space>().Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            if (Keyboard.GetState().IsKeyDown(Keys.S) && !ship.getWin())
            {
                if (ship.getFuel() > 0)
                {
                    Matrix O = ConversionHelper.MathConverter.Convert(ship.physicsObject.OrientationMatrix);
                    //set the ships linear momentum based on the transform between Vector3.Backward and the ship's physicObject's OrientationMatrix.
                    //causes the ship to go backward based on it's own orientation. If the game is won, the controls will not work
                    ship.physicsObject.LinearMomentum = ConversionHelper.MathConverter.Convert(Vector3.Transform(Vector3.Backward, O));
                    ship.subtractFuel(.05f);
                }
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.W) && !ship.getWin())
            {
                if (ship.getFuel() > 0)
                {
                    Matrix O = ConversionHelper.MathConverter.Convert(ship.physicsObject.OrientationMatrix);
                    ship.physicsObject.LinearMomentum = ConversionHelper.MathConverter.Convert(Vector3.Transform(Vector3.Forward, O));
                    ship.subtractFuel(.05f);
                }
            }
            else
            {
                //if neither forward nor backward momentum is supplied, set momentum to 0
                ship.physicsObject.LinearMomentum = new BEPUutilities.Vector3(0, 0, 0);
            }
            Matrix Orientation = ConversionHelper.MathConverter.Convert(ship.physicsObject.OrientationMatrix);
            //sets the camera position based on the ships physicsObject position
            CameraPosition = new Vector3(ship.physicsObject.Position.X, ship.physicsObject.Position.Y, ship.physicsObject.Position.Z);
            //sets camera direction to the forward orientation plus the cameraPosition
            CameraDirection = Vector3.Transform(Vector3.Forward, Orientation) + CameraPosition;
            //These ifs determine the rotation, a and d to rotate around the Y axis i.e. turn, Up and Down to rotate along the X access to look up and down
            //and left and right to rotate along the Z access. If none of these buttons are pressed, AngularMomentum will be 0. If the game is won the controls
            //will not work
            if (Keyboard.GetState().IsKeyDown(Keys.A) && !ship.getWin())
            {
                ship.physicsObject.AngularMomentum = new BEPUutilities.Vector3(0, .7f, 0);
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.D) && !ship.getWin())
            {
                ship.physicsObject.AngularMomentum = new BEPUutilities.Vector3(0, -.7f, 0);
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.Up) && !ship.getWin())
            {
                ship.physicsObject.AngularMomentum = new BEPUutilities.Vector3(.7f, 0, 0);
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.Down) && !ship.getWin())
            {
                ship.physicsObject.AngularMomentum = new BEPUutilities.Vector3(-.7f, 0, 0);
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.Left) && !ship.getWin())
            {
                ship.physicsObject.AngularMomentum += new BEPUutilities.Vector3(0f, 0, .7f);
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.Right) && !ship.getWin())
            {
                ship.physicsObject.AngularMomentum += new BEPUutilities.Vector3(0f, 0, -.7f);
            }
            else
            {
               ship.physicsObject.AngularMomentum = new BEPUutilities.Vector3(0, 0, 0);
            }
            if(Keyboard.GetState().IsKeyDown(Keys.F) && torpCount < countDown && ship.torpedoLeft())
            {
                //The position of the torpedo is the camera position + 1.5 units in front of the position
                Vector3 torpedoPos = CameraPosition + Vector3.Transform(Vector3.Forward, Orientation * 1.5f);
                Vector3 normalPos = ConversionHelper.MathConverter.Convert(ship.physicsObject.OrientationMatrix.Forward);
                normalPos.Normalize();
                //the speed is 9 units/s, achieved by getting the unit vector for the direction thd torpedo is facing (which is the same direction that you are facing)
                Vector3 torpedoVel = normalPos * 9;
                Torpedo torp =  new Torpedo(this, torpedoPos, "Torpedo", 2, torpedoVel , new Vector3(0, 0, 0));
                torp.physicsObject.Orientation = -ship.physicsObject.Orientation;
                ship.useTorpedo();
                //only allow a torpedo every 50 frames, otherwise one press of 'F' could spawn multiple torpedos if you don't let up quickly
                torpCount = countDown+50;
            }
            if(Keyboard.GetState().IsKeyDown(Keys.Space) && !ship.getWin() && !ship.getLose())
            {
                ship.setShielding(true);
                //shielding drains fuel twice as fast as moving. If you're shielding and moving you'll be burning 3 times the fuel
                ship.subtractFuel(.1f);
            }
            else
            {
                ship.setShielding(false);
            }
            countDown++;
            if(countDown%50 == 0)
            {
                //Randomly spawns a new asteroid every frame with a random position between 25 and 50 away from target in every direction
                //and a random velocity between 0 and 20 in every direction. This happens once every 50 frames
                int x = rnd.Next(25, 50);
                if(rnd.Next(0, 1) == 1)
                {
                    x *= -1;
                }
                int y = rnd.Next(25, 50);
                if (rnd.Next(0, 1) == 1)
                {
                    y *= -1;
                }
                int z = rnd.Next(25, 50);
                if (rnd.Next(0, 1) == 1)
                {
                    z *= -1;
                }

                new Asteroid(this, new Vector3(CameraPosition.X + x, CameraPosition.Y + y, CameraPosition.Z + z), "Asteroid", 2, new Vector3(-rnd.Next(0, 20), -rnd.Next(0, 20), -rnd.Next(0, 20)), new Vector3(.3f, .5f, .5f));
            }

            base.Update(gameTime);
        }
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            
            GraphicsDevice.Clear(Color.Black);
            box.Draw(CameraPosition, CameraDirection);
            spriteBatch.Begin();
            //the truncate exists for the purpose of displaying only a whole number instead of a long decimal
            spriteBatch.DrawString(dfont, "Fuel: " + System.Math.Truncate(ship.getFuel()), new Vector2(0, 0), textColor);
            spriteBatch.DrawString(dfont, "Torpedos: " + ship.getTorpedos(), new Vector2(0, 400), textColor);
            spriteBatch.DrawString(dfont, "Shields: " + ship.getShields(), new Vector2(550, 0), textColor);
            if (ship.getWin())
            {
                
                spriteBatch.DrawString(dfont, "YOU WIN!", new Vector2(200, 200), textColor);
                
            }
            else if(ship.getLose())
            {
                spriteBatch.DrawString(dfont, "YOU LOST :(", new Vector2(200, 200), textColor);
            }
            else if(ship.getShielding())
            {
                spriteBatch.DrawString(dfont, "SHIELDS ACTIVE", new Vector2(200, 50), textColor);
            }
            spriteBatch.End();

            base.Draw(gameTime);

        }
    }
}