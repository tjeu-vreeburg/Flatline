using System;
using Godot;

namespace FlatLine.Scripts
{
    public enum SpacecraftBulletMode
    {
        None,
        Directional,
        Mouse
    }

    public partial class SpacecraftBullet : RigidBody2D
    {
        [Export] private double speed = 800f;
        [Export] private double lifeTime = 2f;
        [Export] private Vector2 scale = Vector2.One;
        [Export] private Sprite2D sprite;

        private float timer = 0f;

        public override void _Ready()
        {
            sprite.Scale = scale;
        }

        public override void _PhysicsProcess(double delta)
        {
            timer += (float)delta;
            if (timer >= lifeTime)
            {
                QueueFree();
            }
        }

        public void Shoot(Vector2 direction)
        {
            LinearVelocity = direction.Normalized() * (float)speed;
            Rotation = direction.Angle();
        }

        public void SetSpeed(double speed)
        {
            this.speed = speed;
        }

        public void SetLifeTime(double lifeTime)
        {
            this.lifeTime = lifeTime;
        }

    }
}