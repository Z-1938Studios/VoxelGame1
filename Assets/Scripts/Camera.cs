using OpenTK.Mathematics;

namespace VoxelGame.Visual
{
    public class Camera
    {

        float speed = 1.5f;
        float sensitivity = 0.1f;

        public float pitch;
        public float yaw;
        float near = 0.01f;
        float far = 100.0f;
        float FOV = 45.0f;

        public Vector3i chunkLoadDistance = (5, 5, 5);

        Vector3 cameraTarget = Vector3.Zero;

        #region Positioning
        private Vector3 _position
        = new(0.0f, 0.0f, 3.0f);

        public Vector3 position
        {
            get { return _position; }
            set
            {
                _position = value;
            }
        }

        #endregion
        Vector3 front = new(0.0f, 0.0f, -1.0f);
        Vector3 up = Vector3.UnitY;

        Vector2 lastPos = new(0.0f, 0.0f);

        private Matrix4 _view;
        public Matrix4 View { get { return _view; } set {_view = value; OnCameraViewChanged(); } }

        public class CameraEventArgs : EventArgs
        {
            public Matrix4 View { get; set; }
        }
        public delegate void CameraEventHandler(object source, CameraEventArgs e);
        public event CameraEventHandler? CameraViewChanged;
        protected virtual void OnCameraViewChanged()
        {
            //_view = GetViewMatrix();
            CameraViewChanged?.Invoke(this, new CameraEventArgs { View = _view });
        }
        Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45f), 100 / 100, .01f, 100f);

        public Camera(Vector3? position = null, Matrix4? projection = null)
        {
            if (position.HasValue)
            {
                this.position = position.Value;
            }
            if (projection.HasValue)
            {
                this.projection = projection.Value;
            }
            View = Matrix4.LookAt(this.position, this.position + front, up);
        }

        public void SetPosition(Vector3 position) => this.position = position;
        public void SetSpeed(float speed) => this.speed = speed;
        public void SetNear(float near) => this.near = near;
        public void SetFar(float far) => this.far = far;
        public void SetNearFar(float near, float far) { SetNear(near); SetFar(far); }
        public void SetSensitivity(float sensitivity) => this.sensitivity = sensitivity;
        /// <summary>
        /// Sets the projection matrix of the camera.
        /// </summary>
        /// <param name="fov"></param>
        /// <param name="aspect"></param>
        /// <param name="near"></param>
        /// <param name="far"></param>
        public void SetProjection(float fov, float aspect, float? near = null, float? far = null)
        {
            if (near.HasValue)
            {
                this.near = near.Value;
            }
            if (far.HasValue)
            {
                this.far = far.Value;
            }
            projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(fov), aspect, this.near, this.far);
            Console.WriteLine($"Camera Projection Updated:\n\tFOV: {fov}\n\tASPECT: {aspect}\n\tNEAR/FAR: {this.near}/{this.far}");
        }

        public void Forward(float delta) => position += front * speed * delta;
        public void Backward(float delta) => position -= front * speed * delta;
        public void Right(float delta) => position += Vector3.Normalize(Vector3.Cross(front, up)) * speed * delta;
        public void Left(float delta) => position -= Vector3.Normalize(Vector3.Cross(front, up)) * speed * delta;
        public void Up(float delta) => position += up * speed * delta;
        public void Down(float delta) => position -= up * speed * delta;

        public void Rotate()
        {
            front.X = (float)Math.Cos(MathHelper.DegreesToRadians(pitch)) * (float)Math.Cos(MathHelper.DegreesToRadians(yaw));
            front.Y = (float)Math.Sin(MathHelper.DegreesToRadians(pitch));
            front.Z = (float)Math.Cos(MathHelper.DegreesToRadians(pitch)) * (float)Math.Sin(MathHelper.DegreesToRadians(yaw));
            front = Vector3.Normalize(front);
        }

        public void UpdateLastPos(Vector2 currentPos) => lastPos = currentPos;
        public void UpdateLastPos(float X, float Y) => lastPos = new(X, Y);

        /// <summary>
        /// Updates the rotation of the camera.
        /// `currentPos` is the current position of the mouse, this is used to calculate the change in rotation.
        /// </summary>
        /// <param name="currentPos"></param>
        public void UpdateRotation(Vector2 currentPos)
        {
            float deltaX = currentPos.X - lastPos.X;
            float deltaY = currentPos.Y - lastPos.Y;
            lastPos = currentPos;

            yaw += deltaX * sensitivity;
            if (pitch > 89f)
            {
                pitch = 89f;
            }
            else if (pitch < -89f)
            {
                pitch = -89f;
            }
            else
            {
                pitch -= deltaY * sensitivity;
            }
        }

        public Matrix4 GetViewMatrix() { View = Matrix4.LookAt(position, position + front, up); return View; }
        public Matrix4 GetProjectionMatrix() => projection;
        public Vector3 GetPosition() => position;
        public Vector3 GetFront() => front;
        public float GetSpeed() => speed;

        public bool IsBlockInFOV(Vector3 point)
        {
            Vector3 dir = (point - position).Normalized();
            float angle = (float)Math.Acos(Vector3.Dot(front, dir));
            return angle < ((FOV + 5f) / 2f);
        }
    }
}