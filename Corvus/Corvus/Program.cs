using System;
using Corvus.TestGames;

namespace Corvus {
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args) {
			/*
            using (var game = new CorvusGame()) {
				game.Run();
			}*/
			using(var game2 = new Game2())
				game2.Run();
		}
    }
}

