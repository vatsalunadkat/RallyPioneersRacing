Team Name: Prefab Pioneers 

Game Name: Rally Pioneers 

Team Members: 
    Saurav Yadav (raosaurav@gatech.edu) 
    Vatsal S Kansara (vats.kansara@gatech.edu) 
    Vatsal Paresh Unadkat (vunadkat6@gatech.edu) 
    Vivek Nereus Henriques (vivekh@gatech.edu) 

Start Scene of the project: MainMenu 

How to play and what parts of the level to observe technology requirements? 
- On opening the game, the main menu scene will be displayed. 
- The player can choose to start a new game with different scenes like Forest, Desert or training scene.
- Initially only the training and Desert level are unlocked. The player can unlock the Forest level by finishing first in the Desert level. 
- There is also an Achievements section which shows different achievements that the player can unlock by completing certain tasks.
- The Forest and Desert levels have AI bots that the player can fight against while the training scene is for the player to practice and get used to the controls. 
- The controls for moving the vehicle vary based on the platform. For PCs, the player uses the arrow keys:
- Up and down arrow keys are used to move the vehicle forward and backward respectively. 
- The left and right arrow keys are used to steer the vehicle left and right respectively. 
- The vehicle can be respawned by pressing the 'B' key on the keyboard, and the level can be reset by pressing the 'R' key on the keyboard. The ‘ESC’ key brings up the menu options.
- In the game, the player needs to complete 2 laps of the track in the shortest time possible. 
- There are power ups like speed boost, shields and portals that the player can collect to gain an advantage over the AI bots.
- There are also multiple obstacles like oil spills that the player needs to avoid, as hitting them will cause a temporary loss of vehicle control.

Each of the technological requirements met in the game are listed below: 

1. 3D Game Feel  

- The game has a 3D environment with 3D models for the vehicle, AI bots, power-ups and oil spills. The player can move the vehicle in 3D space and the camera follows the vehicle as it moves around the track. The two different 3D environments are Forest and Desert levels. The Forest level has trees, grass and a road track while the Desert level has sand, rocks and a sandy track. The track themselves have ups and downs as well as sharp turns to make the game more challenging and fun. 
- The game has a clear objective to finish 2 laps of the track in the shortest time possible. The player can see the lap count and time taken on the screen. At the end of the 2 laps, the player is shown the time taken to complete both the laps and the comparison with AI bots. 
- The player has the Start menu to start the chosen level, pause the game in between and respawn the vehicle if it goes off the track. The vehicle can be respawned by pressing the 'B' key on the keyboard and the level can be reset by pressing the 'R' key on the keyboard. 

2.Fun Gameplay 

- Players can choose from a variety of tracks, each offering unique challenges. Tracks include winding roads, elevated twists, and sharp turns to test driving skills. 
- Designed to provide a thrilling experience with dynamic and complex layouts, features such as narrow passages, steep inclines, and hairpin turns keep the race exciting. 
- AI opponents are designed to provide a challenging and competitive racing experience. 
- The tracks are populated with obstacles that players must navigate around or avoid. Elements like oil slicks and barriers add an extra layer of challenge. 
- Players can collect speed boosts to gain an advantage over opponents. Portals and other special power-ups provide unique gameplay twists and strategic options. 
- The game features realistic vehicle dynamics, including accurate acceleration, torque, and suspension behavior. Cars respond authentically to player inputs and environmental interactions, enhancing immersion. 
- Beautifully rendered environments enhance the racing experience. Detailed scenery, realistic lighting, and dynamic weather conditions make each race visually appealing and engaging. 
- The game tracks the time taken to complete each lap, adding an element of time management and efficiency. The game shows the player after-race highlights with the time taken and how the player stands in comparison to the AI NPCs.
- The shield and boost power-ups are placed side-by-side to give the player interesting choices to make. The player can only select one at a time. 
- The player can also choose a shortcut by going off-track. This will reduce the distance, but the player's speed will be reduced due to the sand and off-road terrain.
- The game HUD displays the current rank of the player and also has a mini-map that shows each player's position on the track.
- The Forest level is locked initially and can be unlocked by finishing first in the Desert level. This adds a competitive element to the game and encourages players to improve their skills to unlock new content.
- The Main Menu has an Achievements section that shows different achievements that the player can unlock by completing certain tasks. This adds replay value and encourages players to explore different aspects of the game.
- Added background music to make the player feel more immersed in the game. 

3. 3D Vehicle with Real-Time Control  

- The car emulates real-world behavior with features like moving wheels and suspension movement. 
- Acceleration, vehicle torque, maximum speed, and friction are designed to replicate real-world values and behavior. 
- The car can be easily controlled using mapped keys, providing a smooth and intuitive driving experience. 
- Players can interact with various car components, including the car horn and brakes, while racing. 
- Players can engage in realistic races against AI opponents, enhancing the competitive aspect of the game. 
- AI-controlled cars exhibit distinct behaviors and strategies, providing a challenging and dynamic racing environment. 
- The player's car will interact dynamically with the environment, including obstacles and other scene elements. 
- Players can collect items like speed boosts and encounter hazards such as oil slicks. 
- The game tracks the time taken to complete each lap, adding an element of time management and efficiency. 
- A time counter displays the total time taken by the player and the NPC AI to complete the race. 
- Lap counters, current rank and time records provide detailed feedback on performance, encouraging players to improve their skills and beat their best times. 

4. 3D World with Physics and Spatial Simulation 

- Racing game set in various scenes such as desert and forest tracks. 
- Realistic lighting sources and environmental physics for immersion. 
- Dynamic environmental elements like hills, trees, and sand dunes. 
- Player-interactable elements with accurate physics behaviors. 
- Collisions with scene elements cause realistic stopping and physical reactions. 
- Real-world physics simulations for environmental interactions. 
- AI cars exhibit realistic physics-based behavior on the track, including elements like acceleration, braking, and collision responses. 
- AI cars interact with the environment and other vehicles according to physical laws, enhancing the realism of the racing experience. 
- Player's car with detailed physics including moving wheels, suspension, and vehicle bounce. 
- Accurate simulation of car behavior and collisions.
- The player car can collide with the trees and rocks on the track, it can go over small grass and even uproot medium-sized plants.
- As the player goes off the track on the sand, we can see sand particles flying off the tires of the car.
- NPC and player cars emit smoke from the engine as they try to accelerate or decelerate. 

5. Real-time NPC Steering Behaviors / Artificial Intelligence  

- NPCs are race cars competing against the player in real-time. 
- AI cars include different types such as trucks and race cars. 
- Each vehicle type has distinct acceleration speeds and racing behaviors. 
- AI cars aim to reach the finish line before the player. 
- AI cars are programmed to handle collisions and getting stuck. 
- Upon collision or becoming stuck: 
    The AI car will be re-spawned back onto the track. 
    Re-spawn points are strategically chosen to ensure smooth race continuation. 
- Vehicle-Specific Behaviors: 
    Trucks: 
        Slower acceleration but more stable handling. 
        Favor straight paths and wider turns. 
        Exhibit cautious overtaking maneuvers. 
    Race Cars: 
        Faster acceleration and higher top speeds. 
        Aggressive driving with sharp turns. 
        Focus on overtaking and maintaining the lead. 
- AI Logic: 
  - The AI uses a waypoint system, similar to Milestone 4, to navigate the racetrack. This system directs the agent to follow a set sequence of checkpoints, ensuring lap validity and guiding the agent back on track if they deviate from the racing line. To navigate the racetrack, the agent receives directions to the next waypoint and applies a set torque to the wheel collider motors. 
  - Each agent vehicle is equipped with eleven “sensors” that use physics raycasting for obstacle and vehicle avoidance. Five front sensors detect collisions with objects (excluding terrain) and adjust the steering angle based on their positions. For example, if the front-right sensor detects an obstacle, a negative float value is added to the steering angle to turn the vehicle left. The same logic applies to the front-left sensors. As a failsafe, if the total angle value is zero and the center sensor detects a collision, the agent uses the collision surface’s normal to set an extreme steering angle to avoid a head-on collision. The remaining six sensors, placed on each side of the vehicle, ensure a safe gap between vehicles. 
  - The agent also includes logic to avoid potential deadlocks. If the agent remains stationary for an extended period, it will attempt to free itself by briefly reversing and then moving forward. If reversing does not resolve the issue, the vehicle will be respawned to its last visited waypoint. To handle edge cases where the agent might get stuck in a continuous loop of reversing and moving forward, the agent is automatically respawned to its last visited waypoint if it fails to cover a certain distance within a set timeframe. 
  - Sensory feedback of the AI: Currently, the only sensory feedback the AI provides is through its obstacle avoidance and reversal mechanisms. 
  - The NPC cars have a state machine implementation that controls their behavior. The states include Idle, FollowPath, AvoidObstacles and ReverseCar.

6. Polish 
Main Menu and Navigation
- The game has a main menu with options to start the game, choose levels, and exit.
- Players can select from Forest, Desert, and Training levels. The levels are ranked and renamed based on their difficulty.
- A pause menu, accessible with the 'Esc' key, includes buttons to restart the level, return to the main menu, and exit the game.
Visual and Audio Enhancements
- Smooth transition effects for collectibles like power-ups enhance the visual appeal.
- The vehicle engine sound dynamically adjusts according to the speed of the vehicle, providing a more immersive experience.
Gameplay Interaction
- Players can interact dynamically with the environment, including obstacles and other scene elements.
- Collecting items like speed boosts and encountering hazards such as oil slicks adds strategic depth to the gameplay.
Time Tracking and Performance
- The game tracks the time taken to complete each lap, displaying lap and race times, and providing post-race highlights comparing player performance to AI opponents.
Track Variety and Challenges
- Players can choose from a variety of tracks, each offering unique challenges with dynamic and complex layouts, including winding roads, elevated twists, and sharp turns.
AI and Competitive Experience
- AI opponents provide a challenging and competitive racing experience with distinct behaviors and strategies.
Player Convenience
- Players can respawn their vehicle if it goes off track by pressing the 'B' key, ensuring they never get stuck and can easily continue racing.
- As the player goes off the track on the sand, we can see sand particles flying off the tires of the car.
- NPC and player cars emit smoke from the engine as they try to accelerate or decelerate. 
- The game HUD displays the current rank of the player and also has a mini-map that shows each player's position on the track.

7. Known Problem Areas  
Issue 1: Gamepad disconnections requires level reload
Description: The game does not handle gamepad disconnections gracefully during gameplay. Removing the gamepad while the game is running causes unexpected results and loss of control from the gamepad.
Workaround: To restore normal behaviour, the player must reload the level or restart the game.
 
Issue 2: Keyboard input with Gamepad Connected
 
Description: The game currently prioritizes gamepad input over keyboard input. As a result, when a gamepad is connected, keyboard controls for player movement become unresponsive.
Workaround: The only workaround is to disconnect the gamepad to enable keyboard input.

8. Manifest 

Team responsibility break down: 

Name: Saurav Yadav (raosaurav@gatech.edu) 
Responsibility: Complete implementation of AI Logic and Player Controller. Level designing for the Desert level, 
Achievements and credits menu with UI/UX, Improve game polish with smoke exhaust and mud/dust effects for all Vehicles, Vegetation shaders, 
Minimap (minimap, vehicle trackers, checkpoint markers on map)
Assets Implemented: Desert.unity
C# Scripts: CarSounds.cs, ModifiedCarController.cs, OpponentEngine.cs, OpponentLapCounter.cs (minor contributions), PathViz.cs, PortalScript.cs, WheelControl.cs, OpponentStateMachine.cs, TreeCollision.cs, TutorialScript.cs, TutorialTrigger.cs 

Name: Vatsal S Kansara (vats.kansara@gatech.edu) 
Responsibility: Checkpoints objects & Laps System, Level designing for the Forest level. Vehicle HUD: Complete speedometer, lap countdown and player position implementation.  
Data Logging for playtesting, Game Music and Sound Effects, Level Unlocking System
Assets Implemented: Forest.unity
C# Scripts: AudioManager.cs, ButtonSounds.cs, DataLogger.cs, LapCounter.cs, LockForestLevel.cs, OpponentLapCounter.cs, PlayerPosition.cs, Sound.cs, Speedometer.cs, CollisionController.cs

Name: Vatsal Paresh Unadkat (vunadkat6@gatech.edu) 
Responsibility: Menu implementation that includes design, display and behavior for Main and the Pause menu, race countdown timer, camera logic and animations, completion display, logic and total lap time. 
Checkpoint Map Markers, Player Progress and Achievements, Camera Controls, Car respawn and reset.
Assets Implemented: MainMenu.unity. 
C# Scripts: CameraRotateAround.cs, GameQuitter.cs, GameStarter.cs, ModifiedCarController.cs, PauseMenu.cs, RaceCountdown.cs, LapCounter.cs, ScoreboardManager.cs, CheckpointMapMarkers.cs, PlayerProgressScript.cs, RespawnDirectionAdjustments.cs  

Name: Vivek Nereus Henriques (vivekh@gatech.edu) 
Responsibility: Complete consumables, collectables & obstacles implementations – logic, vehicle behaviors, and placements. Level designing for the Training level. 
Sound and Audio system, Tutorial Controls and Hints UX/UI
Assets Implemented: Training_Desert.unity. 
C# Scripts: OilSlickScript.cs, SpeedPowerUp.cs, ModifiedCarController.cs, TrackVehicle.cs, CarImpacts.cs, GrassCollison.cs, MenuSounds.cs, SpeedControl.cs, ShieldScript.cs, MusicSelector.cs
 