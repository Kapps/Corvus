using System;
using Corvus.TestGames;

namespace Corvus {
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args) {
            //using (var game2 = new Game2())
              //  game2.Run();
            using (var Game = new CorvusGame())
                Game.Run();
			//using(var game1 = new Game1())
				//game1.Run();
		}
    }
}

