using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using CorvEngine.Scenes;
using Microsoft.Xna.Framework;

namespace CorvEngine.Geometry {

	/// <summary>
	/// Provides information about the geometry of a Platformer game, allowing path-finding to take place.
	/// </summary>
	public class TiledPlatformerGeometry : SceneGeometry {
		/*
		 * So, approach will be as follows:
		 * Go through each tile in the level, and find all groups of solid blocks.
		 * These are the largest rectangles that can be made of entirely solid blocks.
		 * So in an example like
		 *
		 *   . . + + + . .
		 *   + + + + + + +
		 *
		 * Three rectangles would result:
		 *	 The 3x2 middle rectangle
		 *	 The 2x1 bottom-left rectangle
		 *	 The 2x1 bottom-right rectangle.
		 * We'll need to know if we can jump through a rectangle as well in the future.
		 * For example, if we can jump through the bottom of the middle 3x2 rectangle to reach one of the 2x1s.
		 * For now, we assume that you can jump through the bottom of anything. Later we could use a curve and determine if any point on it hits something.
		 * 
		 * After generating those levels, we'll add a TiledPlatformerGeometryObject for each of the rectangles.
		 *	It should derive from GeometryObject.
		 *  LevelGeometry should probably have an abstract method to create Geometry for an Entity as well if needed. Maybe.
		 *  Not for now though, we can just use the level itself until we get in to solid Entities.
		 * 
		 * Eventually, we'll have a set of TiledPlatformerGeometryObjects. These should be put into a QuadTree.
		 * Then, we'll need a way to compute the maximum distance we can travel from a jump going straight up, left, or right.
		 * Create a BoundingBox expanded by this Y, X, and Width this from each edge of the object.
		 * For each of the objects within this, we could possibly go in to.
		 * So, determine with a more complex calculation if we can actually reach them by checking what our Y value would be at the minimum X value we require.
		 * Also check if we'll overshoot it, so we can issue a Stop in an appropriate location.
		 * 
		 * Alternatively, can likely just use a brute force solution with the Curve class.
		 *	But using the QuadTree would likely make this more efficient by only getting the values we know will work.
		 *	
		 * Also, remember that the ideal path could actually require a node that's below. Take into consideration the amount we can move for a jump is different then.
		 *	Aka, using the above approach with the BoundingBox would not work for going up.
		 *	
		 * Also take into consideration Drag and Acceleration, and GravityCoefficients and such.
		 * 
		 * This won't work once dynamic objects come in to play; what if you reach a platform but it moves away?
		 * We could perhaps say that if it moves away simply wait for it to come back. This would work for basic stuff.
		 * Of course, if it never comes back the NPC will just remain frozen.
		 * 
		 * Once we determine which platforms we can get to from one, we can determine the time it would take to reach that platform.
		 *	We should already know this. However we can use distance instead.
		 * If we find a platform we've already checked, just add it's distance to this one, no need to recalculate.
		 *	So we'll be using a depth-first approach.
		 * Recursively check all possible platforms from this one.
		 *	Or use some other approach like A*. Will it work here? Probably.
		 * This will still be somewhat expensive I'd assume.
		 *	Naturally though, all of this is done read-only so we can multithread the path calculation for every component.
		 *	Should then be a system managing updating the path.
		 *	We could also just cache the path and only update it if the ideal path has changed or the NPC goes away from the path.
		 *		If they go only slightly away, we can just get a path back to a node on the old path. If distance is small, use it.
		 *		Otherwise if they go further away, we can recalculate the entire path.
		 *	Overall, this should make it fast enough. Especially since path won't change for now.
		 *	
		 * Also, what do we do if no path is available?
		 *	That's up to the game.
		 *	
		 */


		// Right now, path finding is done in two steps.
		// First, there's local pathfinding.
		// Local pathfinding is done when you're on the same geometry object as your target.
		// In this situation you just go left if left, right if right, or jump if higher.
		// Otherwise, if you can't reach, just have the game do whatever.
		// We can return null and game can handle what it wants to do in that situation.
		// Probably go left/right in order with a flag that resets every time a path is available.
		// We can determine the platform of an entity by checking the highest platform within the X/Y position that's below the entity.

		/// <summary>
		/// Returns all of the geometry objects that this instance contains.
		/// </summary>
		public IEnumerable<ISceneGeometryObject> GeometryObjects {
			get { return _Geometry; }
		}

		/// <summary>
		/// Creates a new TiledPlatformerGeometry instance for the given Scene.
		/// </summary>
		public TiledPlatformerGeometry(Scene Scene) : base(Scene) {
			GenerateTileGeometry();
		}

		private void GenerateTileGeometry() {
			// We only care about layers that are solid. Cache which are to save some cycles.
			Tile[][,] SolidTiles = Scene.Layers.Where(c => c.IsSolid).Select(c => c.Tiles).ToArray();
			// We'll check if tiles are checked already, as it could get a bit complicated to keep track.
			// To check if a tile is checked, we'll simply store if it's X and Y coordinate was checked.
			// Using a bool array of hundreds of thousands of elements seems a bit wasteful however.
			BitArray CheckedTiles = new BitArray((int)(Scene.TilesInMap.Y * Scene.TilesInMap.X), false);
			for(int y = 0; y < Scene.TilesInMap.Y; y++) {
				for(int x = 0; x < Scene.TilesInMap.X; x++) {
					if(!Exists(SolidTiles, CheckedTiles, x, y))
						continue;
					int BitIndex = y * (int)Scene.TilesInMap.X + x;
					bool AlreadyChecked = CheckedTiles.Get(BitIndex);
					if(AlreadyChecked)
						continue;
					//CheckedTiles.Set(BitIndex, true);
					// When we have a tile we need to check, find the largest rectangle that encompasses it.
					Rectangle TileBoundaries = GetLargestSolidTileRectangle(SolidTiles, CheckedTiles, x, y);
					Rectangle WorldBoundaries = new Rectangle(TileBoundaries.X * (int)Scene.TileSize.X, TileBoundaries.Y * (int)Scene.TileSize.Y,
						TileBoundaries.Width * (int)Scene.TileSize.X, TileBoundaries.Height * (int)Scene.TileSize.Y);
					TiledPlatformerGeometryObject Obj = new TiledPlatformerGeometryObject(WorldBoundaries);
					AddGeometry(Obj);
				}
			}
		}

		private Rectangle GetLargestSolidTileRectangle(Tile[][,] SolidTiles, BitArray CheckedTiles, int StartX, int StartY) {
			// Basically, we know we start at the top-level coordinate.
			// So we increase X until we reach a gap.
			// At that point, we increase Y and check each tile for that row until EndX.
			// If we encounter a gap, we set EndX to be where that gap was.
			// We know that every row above it was valid until that location so no need to start over.
			// Also, we treat each checked tile as a gap. Otherwise we'd have duplicates.
			// There's a pretty significant issue here though.
			// Imagine a scenario such as
			/**
			*  . . . . . . . 
			*  . . .   . . .
			*  . . .   . . .
			*  . . .   . . .
			*/ 
			// How would this be handled?
			// The result would end up being something like:
			/**
			* 1 1 1 2 2 2 2 
			* 1 1 1   3 3 3
			* 1 1 1   3 3 3
			* 1 1 1   3 3 3
			*/
			// Which probably doesn't make much sense..
			// You can't even reach 3 ever.

			// So for now we'll just assume there are no gaps.
			// This is the way that probably makes most sense for our game...
			// Though this is in the engine which shouldn't make that assumption, but oh well.
			// TODO: Handle the above.

			int EndX, EndY;
			/*for(EndX = StartX; EndX < Scene.TilesInMap.X; EndX++) {
				if(!Exists(SolidTiles, CheckedTiles, EndX, StartY))
					break;
			}
			//EndX--;
			for(EndY = StartY; EndY < Scene.TilesInMap.Y; EndY++) {
				if(!Exists(SolidTiles, CheckedTiles, StartX, EndY))
					break;
			}
			//EndY--;
			for(int x = StartX; x < EndX; x++) {
				for(int y = StartY; y < EndY; y++) {
					int Index = (y * (int)Scene.TilesInMap.X + x);
					Debug.Assert(!CheckedTiles.Get(Index), "Expected no overlapping tiles when using the rectangle tile approach.");
					
					CheckedTiles.Set(Index, true);
				}
			}*/

			// For now, scrapped above and switched to original one. I think it works better.
			// Can play with it and determine which works better in practice.
			// Also should probably be able to declare a layer unreachable.
			// So, first find the largest possible EndX and EndY. This is taken from above.
			for(EndX = StartX; EndX < Scene.TilesInMap.X; EndX++) {
				if(!Exists(SolidTiles, CheckedTiles, EndX, StartY))
					break;
			}
			for(EndY = StartY; EndY < Scene.TilesInMap.Y; EndY++) {
				if(!Exists(SolidTiles, CheckedTiles, StartX, EndY))
					break;
			}
			// Now verify no gaps in each row. if there is a gap, lower the EndX.
			for(int y = StartY; y < EndY; y++) {
				for(int x = StartX; x < EndX; x++) {
					if(!Exists(SolidTiles, CheckedTiles, x, y)) {
						// Found a gap, reduce EndX to the spot we found a gap at.
						// Then continue on to the next row.
						EndX = x;
						break;
					}
				}
			}
			// Lastly, indicate we checked this rectangle.
			for(int x = StartX; x < EndX; x++) {
				for(int y = StartY; y < EndY; y++) {
					int Index = GetIndex(x, y);
					Debug.Assert(!CheckedTiles.Get(Index), "Expected no overlapping tiles when using the rectangle tile approach.");
					
					CheckedTiles.Set(Index, true);
				}
			}
			return new Rectangle(StartX, StartY, EndX - StartX, EndY - StartY);
		}

		private bool Exists(Tile[][,] Tiles, BitArray CheckedTiles, int X, int Y) {
			foreach(var TileSet in Tiles) {
				if(TileSet[X, Y] != null) {
					if(CheckedTiles.Get(GetIndex(X, Y)))
						continue;
					return true;
				}
			}
			return false;
		}

		private int GetIndex(int X, int Y) {
			return Y * (int)Scene.TilesInMap.X + X;
		}

		protected override ISceneGeometryObject CreateGeometryForEntity(Components.Entity Entity) {
			// At the moment nothing is ever solid.
			// We should probably change that at some point.
			return null;
		}

		protected override void RemoveGeometry(ISceneGeometryObject Object) {
			bool Removed = _Geometry.Remove(Object);
			if(!Removed)
				throw new KeyNotFoundException();
		}

		protected override void AddGeometry(ISceneGeometryObject Object) {
			Debug.Assert(Object is TiledPlatformerGeometryObject);
			// Of course we should use something like a QuadTree,
			// but we're not actually doing solid entities so too few to matter.
			_Geometry.Add(Object);
		}

		private List<ISceneGeometryObject> _Geometry = new List<ISceneGeometryObject>();
	}
}
