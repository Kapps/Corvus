using System;
using Corvus.TestGames;

namespace Corvus {
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            /*
            using (CorvEngine.CorvEngine engine = new CorvEngine.CorvEngine()) {
				engine.Run();
            }
            */
            //using (Game1 game = new Game1())
            //{
            //    game.Run();
            //}

            using (Game2 game = new Game2())
                game.Run();
        }
    }
#endif
}

