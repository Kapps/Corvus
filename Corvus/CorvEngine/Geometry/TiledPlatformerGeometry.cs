using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CorvEngine.Geometry {

	/// <summary>
	/// Provides information about the geometry of a Platformer game, allowing path-finding to take place.
	/// </summary>
	public class TiledPlatformerGeometry {
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
	}
}
