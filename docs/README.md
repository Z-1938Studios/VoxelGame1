# Voxel Game (C# OpenTK)

## Overview
This project is a voxel-based game currently under development. It uses OpenTK, a set of bindings to OpenGL, to handle graphics rendering and windowing. The game features a camera system for navigating the voxel world, shader management for rendering, and a basic world generation system that creates chunks of blocks.

## Features
- **Voxel-based World**: The game generates a world made up of blocky voxels.
- **Camera System**: A free-moving camera for exploring the world.
- **Shader Management**: Uses GLSL shaders for rendering the world.
- **Chunk-based World Generation**: Efficiently manages and renders chunks of blocks.
- **OpenGL Rendering**: Uses OpenTK to interface with OpenGL.

## Project Structure
```
Assets/
├── Scripts/
│   ├── Camera.cs         # Manages the camera's position, view, and projection matrices.
│   ├── Game.cs           # Main game class handling initialization, rendering, and input.
│   ├── Meshing.cs        # Defines mesh structure for chunks and block faces.
│   ├── Shaders.cs        # Manages shader programs and their attributes/uniforms.
│   ├── Utilities.cs      # Utility functions for setting shader uniforms.
│   ├── World.cs          # Manages world generation and block data.
│
├── Shaders/
│   ├── test.frag         # Fragment shader for coloring.
│   ├── test.vert         # Vertex shader for transforming vertices.
│
├── Program.cs            # Entry point, initializes and runs the game.
```

## Requirements
- .NET 8.0
- OpenTK

## Installation & Running
1. Clone the repository:
   ```sh
   git clone <repository-url>
   cd voxel-game
   ```
2. Build the project:
   ```sh
   dotnet build
   ```
3. Run the game:
   ```sh
   dotnet run
   ```

## Future Plans
- Implement physics and collision detection.
- Add texture support for blocks.
- Optimize world generation and chunk management.
- Introduce more block types and biomes.

## License
This project is licensed under the MIT License.
See the [License File](LICENSE) for further details.
