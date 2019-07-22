using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public static class PathFinder {
  #region Static Properties

    /*
      We define a delegate so that we can reuse the PathEdge generation
        method to find a MapSpot according to any defined condition.
        (e.g. locating the MapSpot where a given GamePlayer is located)
    */
    public delegate bool TruthChecker(MapSpot test);
    public static TruthChecker checkSpot;

  #endregion

  #region External Methods

    /*
      MovementDirection MovementDirection WhichWayToTarget(
        MapSpot WhereIAm,
        SpotType targetType
      )

      Returns MovementDirection that begins the shortest path to a spot of type targetType.
    */
    public static MovementDirection WhichWayToTargetType(
      MapSpot WhereIAm,
      SpotType targetType
    ) {
      // Defines and assigns delegate function we will use when checking
      //  the spots we've found.
      bool spotCheck(MapSpot test) {
        return (test.myType == targetType);
      }

      PathFinder.checkSpot = spotCheck;

      // We can take advantage of the pathfinding algorithm to find
      //  our target
      Dictionary<MapSpot, MapSpot> pathEdges = PathFinder.CreatePathEdges(WhereIAm);
      // Use the same function we used for the delgate to get the right MapSpot
      MapSpot targetSpot = pathEdges.Keys.ToList().Find(spotCheck);
      if (targetSpot == null) targetSpot = WhereIAm;

      return PathFinder.FindPathInitialDirection(targetSpot, WhereIAm, pathEdges);
    }
  
  #endregion

  #region Internal Navigation Methods

    /*
      MovementDirection FindPathInitialDirection(
        MapSpot goal,
        MapSpot WhereIAm,
        Dictionary<MapSpot, MapSpot> pathEdges
      )

      Returns MovementDirection to begin shortest path to target MapSpot
        by navigating through pathEdges Dictionary.
    */
    private static MovementDirection FindPathInitialDirection(
      MapSpot goal,
      MapSpot WhereIAm,
      Dictionary<MapSpot, MapSpot> pathEdges
    ) {
      // Go backwards through pathEdges starting from the goal until we find
      //  the spot we first visited
      MapSpot current = goal;
      while (current != WhereIAm) {
        MapSpot origin = pathEdges[current];

        if (origin == WhereIAm) {
          // We have made it back to where the player is and can decide which
          //  direction to go
          MovementDirection direction = WhereIAm.nextSpotsReverseLookup[current];

          Debug.Log(string.Format("I should go, {0}", direction));

          return direction;
        }

        current = origin;
      }

      // If all else fails, choose a random direction
      Debug.LogWarning("Pathfinding Error: Choosing random direction");
      List<MovementDirection> possibleDirections = WhereIAm.nextSpots.Keys.ToList();
      int randomDirectionIndex = (int)UnityEngine.Random.Range(0,possibleDirections.Count);
      MovementDirection randomDirection = possibleDirections[randomDirectionIndex];

      return randomDirection;
    }

    /*
      Dictionary<MapSpot Destination, MapSpot Origin> CreatePathEdges(
        MapSpot WhereIAm
      )

      Returns pathEdge Dictionary that goes the shortest path from WhereIAm to
        target MapSpot as identified via checkSpot() delegate function.
    */
    private static Dictionary<MapSpot, MapSpot> CreatePathEdges(
      MapSpot WhereIAm
    ) {
      Queue<MapSpot> toVisit = new Queue<MapSpot>();
      Dictionary<MapSpot, MapSpot> cameFrom = new Dictionary<MapSpot, MapSpot>();
      toVisit.Enqueue(WhereIAm);

      Debug.Log("Beginning Search for target");

      // Navigate down node tree starting from current position until
      //  we find the target spot in list of navigable neighbors.
      while (toVisit.Count > 0) {
        MapSpot thisSpot = toVisit.Dequeue();
        List<MapSpot> neighbors = thisSpot.nextSpots.Values.ToList();

        foreach (MapSpot nextSpot in neighbors) {
          // If we've gotten to this spot from somewhere already,
          //  it's not worth navigating down this road anymore
          if (cameFrom.ContainsKey(nextSpot)) {
            PathFinder.CleanPathEdges(ref cameFrom, nextSpot, WhereIAm);
            continue;
          }

          // Leave a breadcrumb so we know where we've been
          cameFrom.Add(nextSpot, thisSpot);
          
          // Call Delegate to perform check.
          if (PathFinder.checkSpot(nextSpot)) {
            // We have located the targetMapSpot and can quit searching.
            //  we clear Queue toVisit to exit the while() loop.
            Debug.Log("Found target");
            toVisit.Clear();
          } else {
            // Keep looking.
            toVisit.Enqueue(nextSpot);
          }
        }
      }

      return cameFrom;
    }

    /*
      void CleanPathEdges(
        ref Dictionary<MapSpot, MapSpot> edgePaths,
        MapSpot endOfLine,
        MapSpot pathOrigin
      )

      Accepts edgePaths Dictionary as ref and removes edges for paths that are
        not as optimal as some other one.
    */
    private static void CleanPathEdges(
      ref Dictionary<MapSpot, MapSpot> edgePaths,
      MapSpot endOfLine,
      MapSpot pathOrigin
    ) {
      Debug.Log("end of less-than-optimal path. cleaning up.");
      MapSpot current = edgePaths[endOfLine];
      edgePaths.Remove(endOfLine);

      // Follows the path backwards to the origin and removes entries for 
      //  the shittier path.
      while (current != pathOrigin) {
        var origin = edgePaths[current];
        edgePaths.Remove(current);

        current = origin;
      }
    }
  #endregion
}
