Unity Excavator Vehicle Demo

## Project Summary

This project is a 1-2 player, split-screen, physics-based game demo developed as a technical case study. Players choose between two types of heavy machinery—a Wheel Loader and a Telehandler—and compete to deliver the most loads to a designated drop zone.

The project was built from the ground up, focusing on creating a clean, scalable, and professional code architecture that meets all the requirements outlined in the provided project document.

## Features

-   **Single & Two-Player Modes:** The game can be played solo or as a competitive split-screen experience.
-   **Dynamic Game Setup:** Features a main menu scene where players can select their vehicles. A second player can join on this screen by pressing a dedicated key. The game scene is then dynamically configured based on these selections.
-   **Two Distinct Vehicle Types:**
    -   **Wheel Loader (WL):** Features a two-part arm controlled by two separate radial joints (`HingeJoint`).
    -   **Telehandler (TH):** Features a main rotating arm (`HingeJoint`) and a telescoping secondary arm (`ConfigurableJoint`).
-   **Physics-Based Mechanics:** All vehicle and arm movements are driven by the Unity physics engine, using `Rigidbody` forces and `Joints` for realistic interactions.
-   **Competitive Gameplay Loop:** Players pick up loads and deliver them to a drop zone to score points. The game ends when all loads are delivered, and the player with the highest score wins.
-   **Advanced Event-Driven UI:** The user interface updates efficiently based on C# `event`s, avoiding unnecessary polling in the `Update` loop. It displays shared information (time, total progress) and player-specific information (vehicle, fuel, score) in their respective screen areas during split-screen play. 
-   **Data-Driven Design with ScriptableObjects:** Vehicle statistics (motor force, brake force, fuel capacity, etc.) are managed using `ScriptableObject` assets. This decouples game balance from code, allowing for easy tweaking and scaling (e.g., adding new vehicles without changing code).
-   **Decoupled Gameplay Logic:** An event-driven architecture is used for key gameplay events, such as notifying `DropZone`s when a load is dropped anywhere in the world. This creates modular and maintainable code.
-   **Advanced Physics Stabilization:** Implements manual **Center of Mass** adjustment and a unified **Physics Layer** for each vehicle to prevent common physics instabilities like tipping over and internal collider "fighting".

## Controls

The game supports two players on a single keyboard. 

| Control | Player 1 (Left side of Keyboard) | Player 2 (Right side of Keyboard) |
| :--- | :--- | :--- |
| Move / Steer | Arrow Keys | WASD Keys |
| Arm 1 Control | R / F | I / K |
| Arm 2 Control | T / G | O / L |
| Pick Up / Drop Load | V | Numpad 0 |
| P2 Join Game (in Menu) | N/A | Numpad Enter

*Note: The project document specifies P1 on the LEFT and P2 on the right. The `Input Actions` can be easily swapped if needed.*

## Architectural Highlights (For Reviewers)

This project was built with professional, scalable practices in mind. When reviewing the code, please consider paying special attention to the following architectural patterns:

1.  **ScriptableObjects for Vehicle Stats (`VehicleStats.cs`):** Instead of hard-coding values like `motorForce` in the `BaseVehicleController`, all vehicle-specific data is stored in `ScriptableObject` assets (`WL_Stats`, `TH_Stats`). This demonstrates a data-driven approach that allows designers to balance the game without touching any code.

2.  **Event-Driven Architecture:**
    * **UI System (`UIManager.cs`):** The UI does not poll for data every frame. Instead, it subscribes to events like `GameManager.OnScoreChanged` and `BaseVehicleController.OnFuelChanged`. This is a highly efficient pattern that ensures the UI only updates when necessary.
    * **Gameplay Logic (`AttachmentController.cs`, `DropZone.cs`):** The `AttachmentController` broadcasts a global `static event` when a load is dropped. All `DropZone`s listen for this event and then check if the dropped load is within their bounds. This decouples the "action" from the "reaction," creating highly modular and reusable components.

3.  **Dynamic Scene Management (`GameManager.cs`, `GameSettings.cs`):** Player choices from the `MainMenuScene` are stored in a persistent `DontDestroyOnLoad` object (`GameSettings`). The `GameManager` in the `GameScene` reads from this object on `Start()` to dynamically instantiate the correct vehicle prefabs, configure player inputs, and set up the cameras for either single-player or split-screen mode.

4.  **Physics Stability:** The `BaseVehicleController` demonstrates two key techniques for solving common vehicle physics problems:
    * It uses an empty `GameObject` to manually set the `Rigidbody.centerOfMass`, preventing the vehicles from being top-heavy and tipping over.
    * It uses a dedicated "Vehicle" Physics Layer and modifies the Layer Collision Matrix to prevent a vehicle's own colliders from interacting with each other, which solves a wide range of physics instability issues.

## How to Run

1.  Open the project in a compatible version of the Unity Editor
2.  Open the `MainMenuScene` located in the `Scenes` folder.
3.  Press the Play button in the editor.
4.  Or you can run the executable in the build folder