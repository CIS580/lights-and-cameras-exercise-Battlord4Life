using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace LightingAndCamerasExample
{
    /// <summary>
    /// An interface defining a camera
    /// </summary>
    public interface ICamera
    {
        /// <summary>
        /// The view matrix
        /// </summary>
        Matrix View { get; }

        /// <summary>
        /// The projection matrix
        /// </summary>
        Matrix Projection { get; }
    }
    /// <summary>
    /// A camera that circles the origin 
    /// </summary>
    public class CirclingCamera : ICamera
    {
        // The camera's angle 
        float angle;

        // The camera's position
        Vector3 position;

        // The camera's speed 
        float speed;

        // The game this camera belongs to 
        Game game;

        // The view matrix 
        Matrix view;

        // The projection matrix 
        Matrix projection;

        /// <summary>
        /// The camera's view matrix 
        /// </summary>
        public Matrix View => view;

        /// <summary>
        /// The camera's projection matrix 
        /// </summary>
        public Matrix Projection => projection;

        /// <summary>
        /// Constructs a new camera that circles the origin
        /// </summary>
        /// <param name="game">The game this camera belongs to</param>
        /// <param name="position">The initial position of the camera</param>
        /// <param name="speed">The speed of the camera</param>
        public CirclingCamera(Game game, Vector3 position, float speed)
        {
            this.game = game;
            this.position = position;
            this.speed = speed;
            this.projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.PiOver4,
                game.GraphicsDevice.Viewport.AspectRatio,
                1,
                1000
            );
            this.view = Matrix.CreateLookAt(
                position,
                Vector3.Zero,
                Vector3.Up
            );
        }

        /// <summary>
        /// Updates the camera's positon
        /// </summary>
        /// <param name="gameTime">The GameTime object</param>
        public void Update(GameTime gameTime)
        {
            // update the angle based on the elapsed time and speed
            angle += speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Calculate a new view matrix
            this.view =
                Matrix.CreateRotationY(angle) *
                Matrix.CreateLookAt(position, Vector3.Zero, Vector3.Up);
        }

        /// <summary>
        /// A camera controlled by WASD + Mouse
        /// </summary>
        public class FPSCamera : ICamera
        {
            // The angle of rotation about the Y-axis
            float horizontalAngle;

            // The angle of rotation about the X-axis
            float verticalAngle;

            // The camera's position in the world 
            Vector3 position;


            // The state of the mouse in the prior frame
            MouseState oldMouseState;


            // The Game this camera belongs to 
            Game game;

            /// <summary>
            /// The view matrix for this camera
            /// </summary>
            public Matrix View { get; protected set; }

            /// <summary>
            /// The projection matrix for this camera
            /// </summary>
            public Matrix Projection { get; protected set; }

            /// <summary>
            /// The sensitivity of the mouse when aiming
            /// </summary>
            public float Sensitivity { get; set; } = 0.0018f;

            /// <summary>
            /// The speed of the player while moving 
            /// </summary>
            public float Speed { get; set; } = 0.5f;

            /// <summary>
            /// Constructs a new FPS Camera
            /// </summary>
            /// <param name="game">The game this camera belongs to</param>
            /// <param name="position">The player's initial position</param>
            public FPSCamera(Game game, Vector3 position)
            {
                this.game = game;
                this.position = position;
                this.horizontalAngle = 0;
                this.verticalAngle = 0;
                this.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, game.GraphicsDevice.Viewport.AspectRatio, 1, 1000);
                Mouse.SetPosition(game.Window.ClientBounds.Width / 2, game.Window.ClientBounds.Height / 2);
                oldMouseState = Mouse.GetState();
            }

            /// <summary>
            /// Updates the camera
            /// </summary>
            /// <param name="gameTime">The current GameTime</param>
            public void Update(GameTime gameTime)
            {
                var keyboard = Keyboard.GetState();
                var newMouseState = Mouse.GetState();
                // Get the direction the player is currently facing
                var facing = Vector3.Transform(Vector3.Forward, Matrix.CreateRotationY(horizontalAngle));
                // Forward and backward movement
                if (keyboard.IsKeyDown(Keys.W)) position += facing * Speed;
                if (keyboard.IsKeyDown(Keys.S)) position -= facing * Speed;
                // Strafing movement
                if (keyboard.IsKeyDown(Keys.A)) position += Vector3.Cross(Vector3.Up, facing) * Speed;
                if (keyboard.IsKeyDown(Keys.D)) position -= Vector3.Cross(Vector3.Up, facing) * Speed;

                // Adjust horizontal angle
                horizontalAngle += Sensitivity * (oldMouseState.X - newMouseState.X);

                // Adjust vertical angle 
                verticalAngle += Sensitivity * (oldMouseState.Y - newMouseState.Y);

                Vector3 direction = Vector3.Transform(Vector3.Forward, Matrix.CreateRotationX(verticalAngle) * Matrix.CreateRotationY(horizontalAngle));

                // create the veiw matrix
                View = Matrix.CreateLookAt(position, position + direction, Vector3.Up);

                // Reset mouse state 
                Mouse.SetPosition(game.Window.ClientBounds.Width / 2, game.Window.ClientBounds.Height / 2);
                oldMouseState = Mouse.GetState();

            }

        }

    }
}
