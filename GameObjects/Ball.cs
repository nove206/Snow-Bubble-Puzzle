using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bubblepuz.GameObjects
{
    class Ball : GameObject
    {

        private int BallID;
        private int rowID, columnID;
        public Boolean matched;

        public enum BallColor
        {
            BLUE,
            GREEN,
            PINK,
            YELLOW,

            COUNT

        }
        public BallColor ballColor;

        public enum BallStates
        {
            RANDOM,
            IN_GRID,
            FIRED,
            CHECK,
            MATCHED
        }
        public BallStates ballStates;

        public Ball(Texture2D texture, int color) : base(texture)
        {
            switch (color)
            {
                case 0:
                    {
                        ballColor = BallColor.BLUE;
                        break;
                    }
                case 1:
                    {
                        ballColor = BallColor.GREEN;
                        break;
                    }
                case 2:
                    {
                        ballColor = BallColor.PINK;
                        break;
                    }
                case 3:
                    {
                        ballColor = BallColor.YELLOW;
                        break;
                    }
            }
        }

        public Ball(Texture2D texture, int BallID, String type, int color) : base(texture)
        {
            this.BallID = BallID;
            this.columnID = BallID / 100;
            this.rowID = BallID % 100;
            switch (type)
            {
                case "INIT":
                    {
                        ballStates = BallStates.IN_GRID;
                        break;
                    }

            }
            switch (color)
            {
                case 0:
                    {
                        ballColor = BallColor.BLUE;
                        break;
                    }
                case 1:
                    {
                        ballColor = BallColor.GREEN;
                        break;
                    }
                case 2:
                    {
                        ballColor = BallColor.PINK;
                        break;
                    }
                case 3:
                    {
                        ballColor = BallColor.YELLOW;
                        break;
                    }


            }


        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture,
                            Position,
                            Viewport,
                            Color.White);
            base.Draw(spriteBatch);
        }

        public override void Reset()
        {
            base.Reset();
        }

        public override void Update(GameTime gameTime, List<GameObject> gameObjects)
        {

            switch (ballStates)
            {
                case BallStates.RANDOM:
                    {
                        break;
                    }
                case BallStates.IN_GRID:
                    {
                        this.setVelocityINIT();
                        Position += (Velocity * gameTime.ElapsedGameTime.Ticks / TimeSpan.TicksPerSecond)*(Singleton.Instance.Level*1.15f);
                        checkWin();
                        break;
                    }
                case BallStates.FIRED:
                    {
                        if ((Position.Y <= 0 || Position.Y >= Singleton.SCREENHEIGHT))
                        {
                            IsActive = false;
                            Singleton.Instance.BallLeft--;
                            Singleton.Instance.Score -= 20 * Singleton.Instance.Level;
                        }


                        //left wall
                        if (Position.X <=Singleton.POLEWIDTH || Position.X >= Singleton.GAMEWIDTH)
                        {

                            Direction.X *= -1;
                        }

                        Position -= Direction * Velocity * gameTime.ElapsedGameTime.Ticks / TimeSpan.TicksPerSecond;

                        checkCollision();

                        break;
                    }
                case BallStates.CHECK:
                    {
                        checkNeighbors();
                        break;
                    }
                case BallStates.MATCHED:
                    {
                        Singleton.Instance.CooldownCombo = 0;
                        Singleton.Instance.BallLeft--;
                        Singleton.Instance.Score += (100 * Singleton.Instance.Level+(Singleton.Instance.Combo*10));
                        Singleton.Instance.Combo++;
                        this.IsActive = false;
                        Singleton.Instance.ballMap.Remove(BallID);
                        checkWin();
                        break;
                    }

            }
            base.Update(gameTime, gameObjects);
        }

        public void checkWin()
        {
            if (Position.Y >= (Singleton.SCREENHEIGHT - Singleton.CANNONHEIGHT-(Singleton.BALLHITBOX*2)) && Name.Equals("Candy"))
            {
                Singleton.Instance.CurrentGameResult = Singleton.GameResult.Lose;
                Singleton.Instance.CurrentGameState = Singleton.GameState.GameEnded;
            }
            else if (Singleton.Instance.BallLeft <= 0)
            {
                Singleton.Instance.CurrentGameResult = Singleton.GameResult.Win;
                Singleton.Instance.CurrentGameState = Singleton.GameState.GameEnded;
            }
        }

        public void checkNeighbors()
        {
            int neighbor1 = (this.columnID + 1) * 100 + this.rowID;
            int neighbor2 = (this.columnID - 1) * 100 + this.rowID;
            int neighbor3 = (this.columnID) * 100 + this.rowID - 1;
            int neighbor4;
            int neighbor5;
            int neighbor6;

            if ((this.rowID) % 2 == 0)
            {
                neighbor4 = (this.columnID + 1) * 100 + this.rowID - 1;
                neighbor5 = (this.columnID) * 100 + this.rowID + 1;
                neighbor6 = (this.columnID + 1) * 100 + this.rowID + 1;
            }
            else
            {
                neighbor4 = (this.columnID - 1) * 100 + this.rowID - 1;
                neighbor5 = (this.columnID) * 100 + this.rowID + 1;
                neighbor6 = (this.columnID - 1) * 100 + this.rowID + 1;
            }
            //ขวา
            if (Singleton.Instance.ballMap.ContainsKey(neighbor1) && (Singleton.Instance.ballMap[neighbor1].ballColor.Equals(ballColor))
                && (Singleton.Instance.ballMap[neighbor1].matched.Equals(false)))
            {
                this.matched = true;
                Singleton.Instance.ballMap[neighbor1].matched = true;
                Singleton.Instance.ballMap[neighbor1].ballStates = BallStates.CHECK;

            }
            //ซ้าย
            if (Singleton.Instance.ballMap.ContainsKey(neighbor2) && (Singleton.Instance.ballMap[neighbor2].ballColor.Equals(ballColor))
                && (Singleton.Instance.ballMap[neighbor2].matched.Equals(false)))
            {
                this.matched = true;
                Singleton.Instance.ballMap[neighbor2].matched = true;
                Singleton.Instance.ballMap[neighbor2].ballStates = BallStates.CHECK;

            }
            //บนซ้าย
            if (Singleton.Instance.ballMap.ContainsKey(neighbor3) && (Singleton.Instance.ballMap[neighbor3].ballColor.Equals(ballColor))
                && (Singleton.Instance.ballMap[neighbor3].matched.Equals(false)))
            {
                this.matched = true;
                Singleton.Instance.ballMap[neighbor3].matched = true;
                Singleton.Instance.ballMap[neighbor3].ballStates = BallStates.CHECK;

            }
            //บนขวา
            if (Singleton.Instance.ballMap.ContainsKey(neighbor4) && (Singleton.Instance.ballMap[neighbor4].ballColor.Equals(ballColor))
                && (Singleton.Instance.ballMap[neighbor4].matched.Equals(false)))
            {
                matched = true;
                Singleton.Instance.ballMap[neighbor4].matched = true;
                Singleton.Instance.ballMap[neighbor4].ballStates = BallStates.CHECK;

            }
            //ล่างซ้าย
            if (Singleton.Instance.ballMap.ContainsKey(neighbor5) && (Singleton.Instance.ballMap[neighbor5].ballColor.Equals(ballColor))
                && (Singleton.Instance.ballMap[neighbor5].matched.Equals(false)))
            {
                this.matched = true;
                Singleton.Instance.ballMap[neighbor5].matched = true;
                Singleton.Instance.ballMap[neighbor5].ballStates = BallStates.CHECK;

            }
            //ล่างขวา
            if (Singleton.Instance.ballMap.ContainsKey(neighbor6) && (Singleton.Instance.ballMap[neighbor6].ballColor.Equals(ballColor))
                && (Singleton.Instance.ballMap[neighbor6].matched.Equals(false)))
            {
                this.matched = true;
                Singleton.Instance.ballMap[neighbor6].matched = true;
                Singleton.Instance.ballMap[neighbor6].ballStates = BallStates.CHECK;

            }

            if (matched)
            {
                
                ballStates = BallStates.MATCHED;
            }
            else
            {
                ballStates = BallStates.IN_GRID;
            }

        }

        public void checkCollision()
        {
            foreach (KeyValuePair<int, Ball> entry in Singleton.Instance.ballMap)
            {
                int otherBallID = entry.Key;
                Ball otherball = entry.Value;
                if (IsTouching(otherball) && otherball.Name.Equals("Candy"))
                {
                    int otherBallRowID = otherBallID % 100;
                    int otherBallColumnID = otherBallID / 100;

                    if ((Position.Y >= otherball.Position.Y + (Singleton.BALLHITBOX / 2)))
                    {
                        Position.Y = otherball.Position.Y + Singleton.BALLHITBOX;
                        rowID = otherBallRowID + 1;
                        if ((rowID) % 2 == 0)
                        {
                            if (otherBallColumnID == 8)
                            {
                                Position.X = (otherBallColumnID - 1) * Singleton.BALLHITBOX +  (Singleton.BALLHITBOX / 2);

                            }
                            else
                            {
                                if (Position.X >= otherball.Position.X)
                                {
                                    Position.X = otherBallColumnID * Singleton.BALLHITBOX + (Singleton.BALLHITBOX / 2);
                                }
                                else
                                {
                                    Position.X = (otherBallColumnID - 1) * Singleton.BALLHITBOX + (Singleton.BALLHITBOX / 2);
                                }
                            }
                            columnID = (int)(Position.X - (Singleton.BALLHITBOX / 2)) / Singleton.BALLHITBOX;
                        }
                        else
                        {
                            if (Position.X <= otherball.Position.X)
                            {
                                Position.X = otherBallColumnID * Singleton.BALLHITBOX;
                            }
                            else
                            {
                                Position.X = (otherBallColumnID + 1) * Singleton.BALLHITBOX;
                            }
                            columnID = (int)(Position.X) / Singleton.BALLHITBOX;
                        }

                    }
                    else
                    {
                        Position.Y = otherball.Position.Y;
                        rowID = otherBallRowID;
                        if ((rowID) % 2 == 0)
                        {
                            if (Position.X >= otherball.Position.X)
                            {
                                Position.X = (otherBallColumnID + 1) * Singleton.BALLHITBOX + (Singleton.BALLHITBOX / 2);
                            }
                            else
                            {
                                Position.X = (otherBallColumnID - 1) * Singleton.BALLHITBOX + (Singleton.BALLHITBOX / 2);
                            }
                            columnID = (int)(Position.X - (Singleton.BALLHITBOX / 2)) / Singleton.BALLHITBOX;

                        }
                        else
                        {
                            if (Position.X >= otherball.Position.X)
                            {
                                Position.X = (otherBallColumnID + 1) * Singleton.BALLHITBOX;
                            }
                            else
                            {
                                Position.X = (otherBallColumnID - 1) * Singleton.BALLHITBOX;
                            }
                            columnID = (int)(Position.X) / Singleton.BALLHITBOX;
                        }


                    }

                    BallID = columnID * 100 + rowID;
                    Singleton.Instance.ballMap.Add(BallID, this);
                    ballStates = BallStates.CHECK;
                    break;
                }

            }

        }
        public void setVelocityINIT()
        {
            this.Velocity.X = 0;
            this.Velocity.Y = (float)(((Singleton.Instance.Level * 1.4) / 10 + 1) * 5);
            this.Direction.X = 0;
            this.Direction.Y = 0;
        }
    }
}
