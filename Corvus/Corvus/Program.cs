using System;
using Corvus.TestGames;

namespace Corvus {
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args) {
            using (var game = new CorvusGame()) {
                // TODO: Make the above in an overridden initialize method so we can actually call it inside Game.Run().
                game.Run();
            }
		}
    }
}

