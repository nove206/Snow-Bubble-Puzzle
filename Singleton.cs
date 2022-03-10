using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bubblepuz.GameObjects;

namespace Bubblepuz
{
    class Singleton
    {
        public const int SCREENWIDTH = 800; //ชนาดจอ
        public const int SCREENHEIGHT = 800;

        public const int GAMEWIDTH = 480; //ขนาดอจเกม
        public const int GAMEHEIGHT = 800;

        public const int STATUSWIDTH = 200; //bar
        public const int STATUSHEIGHT = 800;

        public const int POLEWIDTH = 60; //เสา
        public const int POLEHEIGHT = 800;

        public const int CANNONWIDTH = 120; //ปืนarea
        public const int CANNONHEIGHT = 120;

        public const int BALLHITBOX = 60; //ขนาดบอล
        public const int INITROW = 16; //จำนวน gen
        public const int INITPOSITION_Y = -900; //โผล่

        public float MasterBGMVolume;
        public float MasterSFXVolume;
        public float CooldownCombo = 0;

        public Dictionary<int, Ball> ballMap;
        public Queue<int> randomColor = new Queue<int>();

        public int Score;
        public int BallLeft;
        public int Level;
        public int Combo;

        //TODO: Game State Machine
        public enum GameState
        {
            GameMain,
            GameStart,
            GamePlaying,
            GameWin,
            GameLose,
            GameEnded,
        }
 
        public GameState CurrentGameState;
        

        public enum GameResult
        {
            Win,
            Lose
        }

        public GameResult CurrentGameResult;

        public KeyboardState PreviousKey, CurrentKey;
        public MouseState CurrentMouse;

        private static Singleton instance;

        private Singleton()
        {

        }
        public static Singleton Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Singleton();
                }
                return instance;
            }
        }

    }
}

