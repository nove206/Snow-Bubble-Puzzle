using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Bubblepuz.GameObjects;
using System;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;

namespace Bubblepuz
{
    public class MainGame : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Cannon cannon;
        List<GameObject> _gameObjects;
        int _numObject;
        private SoundEffectInstance BoomSound, _bgw, _bgl,ChangeBall;
        SpriteFont _spriteFont;
        Texture2D background;
        Texture2D PoleLeft,PoleRight,shooter,bearball,EndGameLoss,EndGameWin,Line,MainMenu,LogoPlay,ScoreBoard,Troby;
        Texture2D rectTexture;
        Texture2D spaceBallTexture;
        Song _bgm,_bgp;
        Boolean PlaySong = false, PlayEffect = false;
        Ball _currentBall, _nextBall,_TempBall;
        Random rnd;
        Vector2 fontLength;
        long time;
        float tick,Cooldown;
        Vector2 mousePosition;

        public MainGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            this.IsMouseVisible = true;
            graphics.PreferredBackBufferWidth = Singleton.SCREENWIDTH;
            graphics.PreferredBackBufferHeight = Singleton.SCREENHEIGHT;
            graphics.ApplyChanges();

            _gameObjects = new List<GameObject>();
            Singleton.Instance.ballMap = new Dictionary<int, Ball>();
            Singleton.Instance.CurrentKey = Keyboard.GetState();
            Singleton.Instance.CurrentMouse = Mouse.GetState();
            Singleton.Instance.CurrentGameResult = Singleton.GameResult.Lose;
            Singleton.Instance.MasterBGMVolume = 0.15f;
            Singleton.Instance.MasterSFXVolume = 0.05f;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            _spriteFont = Content.Load<SpriteFont>("Fonts/GameText");
            PoleLeft = Content.Load<Texture2D>("Images/PoleLeft");
            PoleRight = Content.Load<Texture2D>("Images/PoleRigth");
            background = Content.Load<Texture2D>("Images/Background");
            shooter = Content.Load<Texture2D>("Images/Cannon");
            bearball = Content.Load<Texture2D>("Images/PBEAR");
            EndGameLoss = Content.Load<Texture2D>("Images/EndGameLoss");
            EndGameWin = Content.Load<Texture2D>("Images/EndGameWin");
            Line = Content.Load<Texture2D>("Images/Line");
            MainMenu = Content.Load<Texture2D>("Images/MainMenu");
            LogoPlay = Content.Load<Texture2D>("Images/LogoPlay");
            ScoreBoard = Content.Load<Texture2D>("Images/ScoreBoard");
            Troby = Content.Load<Texture2D>("Images/Troby");
            spaceBallTexture = this.Content.Load<Texture2D>("Images/Sprite");
            BoomSound = Content.Load<SoundEffect>("Song/CannonSound").CreateInstance();
            _bgm = Content.Load<Song>("Song/MainMenu");
            _bgp = Content.Load<Song>("Song/Porntera");
            _bgw = Content.Load<SoundEffect>("Song/GameWin").CreateInstance();
            _bgl = Content.Load<SoundEffect>("Song/GameOver").CreateInstance();
            ChangeBall = Content.Load<SoundEffect>("Song/BreakBall").CreateInstance();

            rectTexture = new Texture2D(graphics.GraphicsDevice, 250, 800);
            Color[] data = new Color[250 * 800];
            for (int i = 0; i < data.Length; i++) data[i] = Color.AliceBlue;
            rectTexture.SetData(data);
            rnd = new Random();
          
            Singleton.Instance.CurrentGameState = Singleton.GameState.GameMain;

        }

        protected override void Update(GameTime gameTime)
        {
            Singleton.Instance.CurrentKey = Keyboard.GetState();
            _numObject = _gameObjects.Count;
            mousePosition = new Vector2(Singleton.Instance.CurrentMouse.X, Singleton.Instance.CurrentMouse.Y);



            switch (Singleton.Instance.CurrentGameState)
            {
                case Singleton.GameState.GameMain:
                    
                    if (!Singleton.Instance.CurrentKey.Equals(Singleton.Instance.PreviousKey) && Singleton.Instance.CurrentKey.IsKeyDown(Keys.Space))
                    {
                        //Space keys pressed to start
                        Singleton.Instance.CurrentGameState = Singleton.GameState.GameStart;
                    }
                    if (!Singleton.Instance.CurrentKey.Equals(Singleton.Instance.PreviousKey) && Singleton.Instance.CurrentKey.IsKeyDown(Keys.Escape))
                    {
                        //Space keys pressed to start
                        this.Exit();
                    }
                    break;

                case Singleton.GameState.GameStart:
                    Singleton.Instance.Combo = 0;
                    if (Singleton.Instance.CurrentGameResult == Singleton.GameResult.Lose)
                    {
                        Singleton.Instance.Level = 1;
                        Singleton.Instance.Score = 0;
                        Reset();
                    }
                    else
                    {
                        Singleton.Instance.Level++;
                        Singleton.Instance.Score = 0;
                        Reset();
                    }
                    Singleton.Instance.CurrentGameState = Singleton.GameState.GamePlaying;
                    MediaPlayer.Stop();
                    PlaySong = false;
                    break;

                case Singleton.GameState.GamePlaying:
                    Cooldown += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    Singleton.Instance.CooldownCombo += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    tick += gameTime.ElapsedGameTime.Ticks / (float)TimeSpan.TicksPerSecond;
                    time += gameTime.ElapsedGameTime.Ticks;
                    Vector2 Difference = Vector2.Subtract(cannon.Position, new Vector2(Singleton.Instance.CurrentMouse.X, Singleton.Instance.CurrentMouse.Y));
                    Vector2 Direction = Vector2.Normalize(Difference);
                    if (Singleton.Instance.CooldownCombo > 4f)
                    {
                        Singleton.Instance.Combo = 0;
                    }

                    if ((Singleton.Instance.CurrentMouse.LeftButton == ButtonState.Pressed) && mousePosition.Y < 600 )
                    {
                        
                            ChangeBall.Volume = 0.7f;
                            BoomSound.Play();
                        

                        if (Cooldown > 0.7f)
                        {

                            Cooldown = 0;
                            Ball ball = _currentBall;
                            ball.Position = new Vector2(((Singleton.POLEWIDTH + Singleton.GAMEWIDTH - Singleton.CANNONWIDTH) / 2) + Singleton.POLEWIDTH, Singleton.SCREENHEIGHT - Singleton.CANNONHEIGHT - 10);
                            ball.Name = "Candy";
                            ball.ballStates = Ball.BallStates.FIRED;
                            ball.Velocity = new Vector2(1000, 1000);
                            ball.Direction = Direction;

                            //after shot
                            Singleton.Instance.BallLeft++;
                            _currentBall = _nextBall;
                            _currentBall.Position = new Vector2(270, (Singleton.SCREENHEIGHT - Singleton.BALLHITBOX * 2));

                            int indexColor = rnd.Next(4);
                            _nextBall = new Ball(spaceBallTexture, indexColor)
                            {
                                Viewport = new Rectangle(0, indexColor * Singleton.BALLHITBOX, Singleton.BALLHITBOX, Singleton.BALLHITBOX),
                                Position = new Vector2(Singleton.SCREENWIDTH - (Singleton.STATUSWIDTH) - (Singleton.BALLHITBOX) - Singleton.GAMEWIDTH, (Singleton.SCREENHEIGHT - Singleton.BALLHITBOX * 2) + 30)
                            };
                            _gameObjects.Add(_nextBall);
                        }

                    }

                    if (Singleton.Instance.CurrentMouse.RightButton == ButtonState.Pressed)
                    {
                        if (Cooldown > 0.7f)
                        {
                            
                            ChangeBall.Volume = 0.75f;
                            ChangeBall.Play();
                            
                            Cooldown = 0;
                            _TempBall = _currentBall;
                            _TempBall.Position = _currentBall.Position;

                            _currentBall = _nextBall;
                            _currentBall.Position = _nextBall.Position;

                            _nextBall = _TempBall;
                            _nextBall.Position = _TempBall.Position;
                            _currentBall.Position = new Vector2(270, (Singleton.SCREENHEIGHT - Singleton.BALLHITBOX * 2));
                            _nextBall.Position = new Vector2(Singleton.SCREENWIDTH - (Singleton.STATUSWIDTH) - (Singleton.BALLHITBOX) - Singleton.GAMEWIDTH, (Singleton.SCREENHEIGHT - Singleton.BALLHITBOX * 2) + 30);

                        }
                    }


                    for (int i = 0; i < _numObject; i++)
                    {
                        if (_gameObjects[i].IsActive)
                        {
                            _gameObjects[i].Update(gameTime, _gameObjects);
                        }
                    }
                    for (int i = 0; i < _numObject; i++)
                    {
                        if (!_gameObjects[i].IsActive)
                        {
                            _gameObjects.RemoveAt(i);
                            i--;
                            _numObject--;
                        }
                    }
                    break;

                case Singleton.GameState.GameEnded:
                    _gameObjects.Clear();

                    if (Singleton.Instance.CurrentGameResult == Singleton.GameResult.Win)
                        {
                            Singleton.Instance.CurrentGameState = Singleton.GameState.GameWin;
                        }
                        else if (Singleton.Instance.CurrentGameResult == Singleton.GameResult.Lose)
                        {
                            Singleton.Instance.CurrentGameState = Singleton.GameState.GameLose;
                        }

                    break;

                case Singleton.GameState.GameWin:
                    MediaPlayer.Stop();
                    PlaySong = false;
                    if (!PlayEffect)
                    {
                        _bgw.Volume = 0.6f;
                        _bgw.Play();
                        PlayEffect = true;
                    }
                    if (!Singleton.Instance.CurrentKey.Equals(Singleton.Instance.PreviousKey) && Singleton.Instance.CurrentKey.IsKeyDown(Keys.Space))
                    {
                        //Space keys pressed to start
                        Singleton.Instance.CurrentGameState = Singleton.GameState.GameStart;
                    }
                    break;

                case Singleton.GameState.GameLose:
                    MediaPlayer.Stop();
                    PlaySong = false;
                    if (!PlayEffect)
                    {
                        _bgl.Volume = 0.6f;
                        _bgl.Play();
                        PlayEffect = true;
                    }
                    if (!Singleton.Instance.CurrentKey.Equals(Singleton.Instance.PreviousKey) && Singleton.Instance.CurrentKey.IsKeyDown(Keys.Space))
                    {
                        //Space keys pressed to start
                        Singleton.Instance.CurrentGameState = Singleton.GameState.GameMain;
                    }
                    break;
            }

            Singleton.Instance.PreviousKey = Singleton.Instance.CurrentKey;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.LightBlue);
            spriteBatch.Begin();
            switch (Singleton.Instance.CurrentGameState)
            {
                
                case Singleton.GameState.GameMain:
                    if(!PlaySong)
                    {
                        MediaPlayer.Volume = 0.05f;
                        MediaPlayer.Play(_bgm);
                        PlaySong = true;
                    }
                    spriteBatch.Draw(MainMenu, new Vector2(0, 0), Color.White);
                    
                    break;

                case Singleton.GameState.GamePlaying:
                    
                    if (!PlaySong)
                    {
                        MediaPlayer.Volume = 0.15f;
                        MediaPlayer.Play(_bgp);
                        PlaySong = true;
                    }
                    
                    spriteBatch.Draw(background, new Vector2(0, 0), Color.White);
                    spriteBatch.Draw(PoleLeft, new Rectangle(0, 0, 60, 800), Color.White);
                    spriteBatch.Draw(PoleRight, new Rectangle(540, 0, 60, 800), Color.White);
                    
                    spriteBatch.Draw(Line, new Vector2(Singleton.POLEWIDTH, 630), Color.White);
                    spriteBatch.Draw(bearball, new Rectangle(0, 620, 180, 180), Color.White);
                    spriteBatch.Draw(LogoPlay, new Rectangle((int)Singleton.GAMEWIDTH+ (Singleton.POLEWIDTH*2)+25, 20, 150, 176), Color.White);
                    spriteBatch.Draw(ScoreBoard, new Rectangle((int)Singleton.GAMEWIDTH + (Singleton.POLEWIDTH * 2) + 5, Singleton.STATUSHEIGHT / 4 + 10, 190, 380), Color.White);
                    spriteBatch.Draw(Troby, new Rectangle((int)Singleton.GAMEWIDTH + (Singleton.POLEWIDTH * 2) + 30, 580, 120, 190), Color.White);

                    fontLength = _spriteFont.MeasureString(Singleton.Instance.Level.ToString());
                    spriteBatch.DrawString(_spriteFont, Singleton.Instance.Level.ToString(), new Vector2((Singleton.STATUSWIDTH - fontLength.X) / 2 + Singleton.GAMEWIDTH + (Singleton.POLEWIDTH * 2) + (fontLength.X / 2) + 15, (Singleton.STATUSHEIGHT - fontLength.Y) / 3 +10), Color.RoyalBlue);

                    fontLength = _spriteFont.MeasureString(Singleton.Instance.BallLeft.ToString());
                    spriteBatch.DrawString(_spriteFont, Singleton.Instance.BallLeft.ToString(), new Vector2((Singleton.STATUSWIDTH - fontLength.X) / 2 + Singleton.GAMEWIDTH + (Singleton.POLEWIDTH * 2), (Singleton.STATUSHEIGHT - fontLength.Y) / 3 + (fontLength.Y * 5)+21), Color.RoyalBlue);

                    fontLength = _spriteFont.MeasureString(Singleton.Instance.Score.ToString());
                    spriteBatch.DrawString(_spriteFont, Singleton.Instance.Score.ToString(), new Vector2((Singleton.STATUSWIDTH - fontLength.X) / 2 + Singleton.GAMEWIDTH + (Singleton.POLEWIDTH * 2), (Singleton.STATUSHEIGHT - fontLength.Y) / 3 + (fontLength.Y * 3)), Color.RoyalBlue);

                    fontLength = _spriteFont.MeasureString(Singleton.Instance.Combo.ToString());
                    spriteBatch.DrawString(_spriteFont, Singleton.Instance.Combo.ToString(), new Vector2((Singleton.STATUSWIDTH - fontLength.X) / 2 + Singleton.GAMEWIDTH + (Singleton.POLEWIDTH * 2)+ 35, (Singleton.STATUSHEIGHT - fontLength.Y) / 3 + (fontLength.Y * 4)+16), Color.RoyalBlue);


                    fontLength = _spriteFont.MeasureString(String.Format("{0}:{1:00}", time / 600000000, (time / 10000000) % 60));
                    spriteBatch.DrawString(_spriteFont, String.Format("{0}:{1:00}", time / 600000000, (time / 10000000) % 60), new Vector2((Singleton.STATUSWIDTH - fontLength.X) / 2 + Singleton.GAMEWIDTH + (Singleton.POLEWIDTH * 2)+ (fontLength.X/2)+8, (Singleton.STATUSHEIGHT - fontLength.Y) / 2 + fontLength.Y *3 -2), Color.RoyalBlue);

                    break;

                case Singleton.GameState.GameWin:

                    spriteBatch.Draw(EndGameWin, new Vector2(0, 0), Color.White);
                    fontLength = _spriteFont.MeasureString(Singleton.Instance.Score.ToString());
                    spriteBatch.DrawString(_spriteFont, Singleton.Instance.Score.ToString(), new Vector2(385, 390), Color.MediumAquamarine);


                    break;

                case Singleton.GameState.GameLose:

                    spriteBatch.Draw(EndGameLoss, new Vector2(0, 0), Color.White);
                    fontLength = _spriteFont.MeasureString(Singleton.Instance.Score.ToString());
                    spriteBatch.DrawString(_spriteFont, Singleton.Instance.Score.ToString(), new Vector2(385,390), Color.MediumAquamarine);

                    break;

            }


            for (int i = 0; i < _gameObjects.Count; i++)
            {

                _gameObjects[i].Draw(spriteBatch);
            }

            spriteBatch.End();
            graphics.BeginDraw();
            base.Draw(gameTime);
        }
        public void resetball(float x, float y, int yPicture, int ballID, int color)
        {
            Texture2D spaceBallTexture = this.Content.Load<Texture2D>("Images/Sprite");
            Ball ball = new Ball(spaceBallTexture, ballID, "INIT", color)
            {
                Name = "Candy",
                Viewport = new Rectangle(0, yPicture, Singleton.BALLHITBOX, Singleton.BALLHITBOX),
                Position = new Vector2(x, y)
            };
            Singleton.Instance.ballMap.Add(ballID, ball);
            _gameObjects.Add(ball);
            Singleton.Instance.BallLeft++;
        }

        protected void Reset()
        {
            
            time = 0;
            tick = 0;
            Singleton.Instance.ballMap.Clear();
            


            for (int i = 0; i < (int)Ball.BallColor.COUNT; i++)
            {
                Singleton.Instance.randomColor.Enqueue(rnd.Next(4));
            }

            _gameObjects.Clear();


            //DONE
            cannon = new Cannon(shooter)
            {
                Position = new Vector2(300, (Singleton.SCREENHEIGHT - 90)),

                
            };
        _gameObjects.Add(cannon);
            //candy random before shot
            int indexColor = rnd.Next(4);

            BoomSound.Volume = 0.5f;
            //DONE
            _currentBall = new Ball(spaceBallTexture, indexColor)
            {
                Name = "CandyRandom",
                Viewport = new Rectangle(0, indexColor * Singleton.BALLHITBOX, Singleton.BALLHITBOX, Singleton.BALLHITBOX),
                Position = new Vector2(270, (Singleton.SCREENHEIGHT - Singleton.BALLHITBOX * 2)),
            };


            _gameObjects.Add(_currentBall);
            //DONE
            //candy random next before shot
            indexColor = rnd.Next(4);
            _nextBall = new Ball(spaceBallTexture, indexColor)
            {
                Name = "CandyRandom",
                Viewport = new Rectangle(0, indexColor * Singleton.BALLHITBOX, Singleton.BALLHITBOX, Singleton.BALLHITBOX),
                Position = new Vector2(Singleton.SCREENWIDTH - (Singleton.STATUSWIDTH) - (Singleton.BALLHITBOX) - Singleton.GAMEWIDTH, (Singleton.SCREENHEIGHT - Singleton.BALLHITBOX * 2)+30)
            };
            _gameObjects.Add(_nextBall);

            int bubbleColums = (Singleton.GAMEWIDTH / Singleton.BALLHITBOX); //8 colums
            for (int i = 1; i <= Singleton.INITROW; i++) //1-16 row ตอนเริ่มเกม
            {
                for (int j = 1; j <= bubbleColums; j++) //1-8 col
                {
                    int yPicture = 0;
                    float x;
                    float y;

                    if (i % 2 == 0) //7 col
                    {
                        x = (Singleton.GAMEWIDTH / 8 * j)   + (Singleton.GAMEWIDTH / 8) + (Singleton.BALLHITBOX / 2) - Singleton.BALLHITBOX;
                        if (j == (bubbleColums)) continue;
                    }
                    else //8 col
                    {
                        x = (Singleton.GAMEWIDTH / 8 * j) + (Singleton.GAMEWIDTH / 8) - Singleton.BALLHITBOX;
                    }
                    y = Singleton.INITPOSITION_Y + (i * Singleton.BALLHITBOX);
                    int color = rnd.Next(4);
                    yPicture = color * Singleton.BALLHITBOX;
                    int ballID = j * 100 + i;
                    resetball(x, y, yPicture, ballID, color);
                }
            }

            foreach (GameObject s in _gameObjects)
            {
                s.Reset();
            }
        }
    }
}
