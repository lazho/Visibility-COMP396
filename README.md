# COMP396
This project aims to create a 2D visibility shadow effort and apply it on real scenario.

## Introduction:
Guards are very common in game, therefore it is important to implement the visibility of the guard in a map with obstacles. This project aims to create a 2D visibility shadow effort and apply it on real scenario.
## Objectives
* Build a formalized 2D visibility effects on game with robust C# code. 
  * The map can show the visible region and invisible region with different colors.
  * The map state can be changed by different position of the viewpoint.
  * The algorithm can handle some extreme circumstances.(e.g. three points are on the same line)
* Application: (If time permitted)
  * Create a smallest region where a guard can view the whole polygon map with obstacles.
  * Compute the cover and attacking spot based on the result of the visibility.
## Methodology
* The basic idea to compute the region of a guard is to use triangularization. We draw many rays starting from the viewpoint of the guard, connecting with the endpoint of each line segments. 
* The visible region is consisted by many slim triangles.
* Use basic algebra knowledge to compute the line and ray segments.
* A good cover spot is always outside the visible region of the guard.
* A good attacking spot must has all properties of a cover spot and should has the property that the player can attack “suddenly”. (Not well defined)
## Reference 
[Reference link doing with Javascript](https://ncase.me/sight-and-light/). 
