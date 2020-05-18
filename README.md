# Mixed Initiative Procedural Dungeon Designer
A unity3d tool which designs dungeon maps based on a level designer's preferences. This was developed for the publication *Mixed-Initiative Procedural Content Generation using Level Design Patterns and Interactive Evolutionary Optimisation* which is currently under review, but is available as a [pre-print](https://arxiv.org/abs/2005.07478). It is a tool to assist level designers to design levels which would be shipped with a game. Our two design goals were:
1. Allow level designers to interact and effect the results of the algorithm through the act of level design and
2. Support designers to explore the design space.

As part of our research we conducted a user study and found that the tool was effective in inspiring new ideas and design directions. We have published this tool for several reasons (a) to allow the community to recreate our user study (b) to allow the community to extend this work and (c) to let people have a play with it!

## How the Tool is Used
The level designer is first asked to design a level from scratch. When happy with the level she submits it and the system generates several levels which have similar qualities to the one designed by the designer. The designer may then edit these suggested levels and indicate which of those she likes and which she wants to keep (to ship with the game). The system then takes that information into account and suggests more levels. This then repeats.

## How the Tool Works
When a level is initially provided to the system it analyses it and identifies patterns in the level, and calculates metrics based on these patterns. For example, average corridor length or rotational symmetry. An evolutionary optimisation algorithm then tries to generate a set of levels which match these metrics. As the designer specifies more levels which have desirable qualities the system will try to match metrics with any of these levels. For example, generating levels with the average corridor length from the first design and the rotational symmetry of the third design.

## Acknowledgments

### Research Team
* Sean P. Walton (Swansea University)
* Alma A. M. Rahat (Swansea University)
* James Stovold (University of York)

### Key Publications which informed our Approach

Baldwin, A., S. Dahlskog, J. M. Font, and J. Holmberg. 2017. “Mixed-Initiative Procedural Generation of Dungeons Using Game Design Patterns.” In 2017 IEEE Conference on Computational Intelligence and Games (CIG), 25–32.

Craveirinha, Rui, and Licinio Roque. 2015. “Studying an Author-Oriented Approach to Procedural Content Generation through Participatory Design.” In Entertainment Computing - ICEC 2015, 383–90. Springer International Publishing.

Liapis, Antonios, Gillian Smith, and Noor Shaker. 2016. “Mixed-Initiative Content Creation.” In Procedural Content Generation in Games, edited by Noor Shaker, Julian Togelius, and Mark J. Nelson, 195–214. Cham: Springer International Publishing.

Preuss, M., A. Liapis, and J. Togelius. 2014. “Searching for Good and Diverse Game Levels.” In 2014 IEEE Conference on Computational Intelligence and Games, 1–8.



