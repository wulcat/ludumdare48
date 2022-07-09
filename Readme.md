#### Procedural System
    - Maintain the dungeons for construction and destruction
    - Transtion from level to level
    - Maintain dungeon for proceding to next level or go deep within a dungeon for more dungeons

#### Dungeons - will hold the creation of a floor
    - Create Layout
        - Create pillars (blocking walls)
        - Create entry and exit points
        - Sub Dungeons entry points
    
#### Cofiguration for incremenatal levels
    - Will have the complexity config for dungeons (interpolated with spike in every 5 levels with a cap)
    - Enemy count and level 
    - Bosses
    - Any pickups
    - Hidden rooms for chests

#### Global Settings
    - Overall main generation values (like random seed, min-max dungeon sizes)

#### Class Diagram
![System](ProceduralSystemClassDiagram.png?raw=true "Title")
