//Lab 7 - Mining Teddies
//Steven Duong
//816623412
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace MiningTeddies
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        List<Mine> mineList = new List<Mine>();
        List<TeddyBear> teddyBearList = new List<TeddyBear>();
        List<Explosion> explosionList = new List<Explosion>();

        Texture2D teddySprite;
        Texture2D mineSprite;
        Texture2D explosionSprite;

        int timer;
        int spawnTime;
        bool newSpawn;
        Random rand = new Random();
        int randX;
        int randY;
        Vector2 randomVelocity;
        int velocityXtemp;
        float velocityX;
        int velocityYtemp;
        float velocityY;
        
        public const int WindowWidth = 604;
        public const int WindowHeight = 453;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = WindowHeight;
            graphics.PreferredBackBufferWidth = WindowWidth;
            this.IsMouseVisible = true;
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            teddySprite = Content.Load<Texture2D>(@"graphics\teddybear");
            mineSprite = Content.Load<Texture2D>(@"graphics\mine");
            explosionSprite = Content.Load<Texture2D>(@"graphics\explosion");
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// Add a teddy bear to the teddy bear list with random speed and position
        /// </summary>
        public void addTeddyBear()
        {
            randX = rand.Next(604);
            randY = rand.Next(453);

            velocityXtemp = rand.Next(-5, 5); //-0.5 to 0.4
            velocityX = (float)velocityXtemp / 10;
            velocityYtemp = rand.Next(-5, 5); //-0.5 to 0.4
            velocityY = (float)velocityYtemp / 10;

            randomVelocity.X = velocityX;
            randomVelocity.Y = velocityY;
            TeddyBear temp = new TeddyBear(teddySprite, randomVelocity, randX, randY);
            temp.Active = true;
            teddyBearList.Add(temp);
        }

        /// <summary>
        /// When user left clicks anywhere on the program, create a mine
        /// </summary>
        public void createMineWhenClicked()
        {
            //add a new mine to the list whenever user clicks 
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                Mine mineTemp = new Mine(mineSprite, Mouse.GetState().X, Mouse.GetState().Y);
                mineTemp.Active = true;
                mineList.Add(mineTemp);
            }
        }
    
        /// <summary>
        /// Generate a new spawn time for each teddy bear
        /// </summary>
        public void generateBearSpawnTime()
        {
            if (newSpawn)               //get a new spawn time for each bear
            {
                spawnTime = rand.Next(1000, 3001);  //1 to 3 seconds spawn time
                newSpawn = false;
            }
        }

        /// <summary>
        /// Start the explosion animation
        /// </summary>
        /// <param name="gameTime"></param>
        public void explosionAnimation(GameTime gameTime)
        {
            foreach (Explosion explosion in explosionList)
                explosion.Update(gameTime);
        }

        /// <summary>
        /// Generate a teddy bear to the screen when timer exceeds spawnTime
        /// </summary>
        public void generateBear()
        {
            //generate a new bear to the screen if newSpawn time is passed
            if (timer > spawnTime)
            {
                addTeddyBear();
                spawnTime = rand.Next(1000, 3001);  //get new spawn time
                timer = 0;
                newSpawn = true;
            }
        }

        /// <summary>
        /// Remove all the inactive mines in the mine list
        /// </summary>
        public void removeAllMines()
        {
            for (int i = mineList.Count -1; i >= 0; i--)
            {
                if (mineList[i].Active == false)
                    mineList.Remove(mineList[i]);
            }
        }

        /// <summary>
        /// Remove all the inactive bears in the teddy bear list
        /// </summary>
        public void removeAllBears()
        {
            for (int i = teddyBearList.Count - 1; i >= 0; i--)
            {
                if (teddyBearList[i].Active == false)
                    teddyBearList.Remove(teddyBearList[i]);
            }
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
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

            createMineWhenClicked();

            timer += gameTime.ElapsedGameTime.Milliseconds;
            generateBearSpawnTime();
            generateBear();
        
            foreach (TeddyBear bear in teddyBearList)
            {
                foreach (Mine mine in mineList)
                {
                    if ((bear.CollisionRectangle.Intersects(mine.CollisionRectangle)))
                    {
                        mine.Active = false;
                        bear.Active = false;

                        Explosion explosionTemp = new Explosion(explosionSprite, mine.Location.X, mine.Location.Y);
                        explosionList.Add(explosionTemp);
                        explosionTemp.Play(mine.Location.X, mine.Location.Y);  
                    }
                }
            }

            removeAllBears();
            removeAllMines();

            foreach (TeddyBear bear in teddyBearList)
                bear.Update(gameTime);

            explosionAnimation(gameTime);
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            foreach (TeddyBear bear in teddyBearList)
                bear.Draw(spriteBatch);
            
            foreach (Mine mine in mineList)
                mine.Draw(spriteBatch);
                
            foreach (Explosion explosion in explosionList)
                explosion.Draw(spriteBatch);

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
