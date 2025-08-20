using FlatLine.Scripts.Environment;
using Godot;
using System;
using System.Collections.Generic;

namespace FlatLine.Scripts
{
    public partial class Starfield : Node2D
    {
        [Export] private Vector2 StarSpeedRange = new(0.001f, 0.05f);
        [Export] private Vector2 StarSizeRange = new(0.3f, 1.5f); 
        [Export] private int StarCount = 300;
        [Export] private float ParallaxStrength = 0.1f; 
        [Export] private float MaxBrightness = 0.8f;
        [Export] private bool EnableAtmosphericPerspective = true;

        private Camera2D camera;
        private Spacecraft spaceCraft;
        private readonly List<Star> stars = [];
        private Vector2 lastCameraPosition;
        private Vector2 viewportSize;
        private float spawnMargin = 300f;

        public void Intialise(Spacecraft spacecraft)
        {
            this.spaceCraft = spacecraft;
        }

        public override void _Ready()
        {
            camera = spaceCraft.GetCamera();
            lastCameraPosition = camera.GlobalPosition;
            viewportSize = GetViewportRect().Size;

            GenerateInitialStars();

            ZIndex = -100;
        }
        
        private void GenerateInitialStars()
        {
            if (camera == null) return;

            stars.Clear();
            var rand = new Random();

            var cameraPos = camera?.GlobalPosition ?? Vector2.Zero;
            var areaWidth = viewportSize.X + spawnMargin * 2;
            var areaHeight = viewportSize.Y + spawnMargin * 2;

            for (int i = 0; i < StarCount; i++)
            {
                var distance = 1.0f - (float)Math.Pow(rand.NextDouble(), 3.0);

                var star = new Star
                {
                    Position = new Vector2(
                        cameraPos.X + ((float)rand.NextDouble() - 0.5f) * areaWidth,
                        cameraPos.Y + ((float)rand.NextDouble() - 0.5f) * areaHeight
                    ),
                    Size = Mathf.Lerp(StarSizeRange.X, StarSizeRange.Y, distance * distance),
                    Speed = Mathf.Lerp(StarSpeedRange.X, StarSpeedRange.Y, distance),
                    Color = GetDeepSpaceStarColor(rand, distance),
                    Distance = distance,
                    Twinkle = (float)rand.NextDouble() * Mathf.Pi * 2
                };

                stars.Add(star);
            }
        }
        
        private Color GetDeepSpaceStarColor(Random rand, float distance)
        {
            var colorVariation = (float)rand.NextDouble();
            Color baseColor;

            if (colorVariation < 0.6f)
                baseColor = Colors.White;
            else if (colorVariation < 0.75f)
                baseColor = new Color(0.9f, 0.95f, 1.0f);
            else if (colorVariation < 0.9f)
                baseColor = new Color(1.0f, 0.95f, 0.8f);
            else
                baseColor = new Color(1.0f, 0.9f, 0.85f);

            if (EnableAtmosphericPerspective)
            {
                var brightness = 0.1f + (distance * MaxBrightness);
                var blueShift = 1.0f + (1.0f - distance) * 0.2f;
                
                var redShift = 1.0f - (1.0f - distance) * 0.05f;
                var greenShift = 1.0f - (1.0f - distance) * 0.02f;
                
                baseColor = new Color(
                    baseColor.R * brightness * redShift,
                    baseColor.G * brightness * greenShift,
                    baseColor.B * brightness * blueShift,
                    baseColor.A * brightness
                );
            }
            
            return baseColor;
        }
        
        public override void _Process(double delta)
        {
            if (camera == null) return;

            var cameraPos = camera.GlobalPosition;
            var cameraDelta = cameraPos - lastCameraPosition;
            
            if (cameraDelta.LengthSquared() > 0.01f)
            {
                for (int i = 0; i < stars.Count; i++)
                {
                    var star = stars[i];
                    
                    var parallaxMovement = cameraDelta * star.Speed * ParallaxStrength;
                    star.Position -= parallaxMovement;
                    star.Twinkle += (float)delta * 0.5f;
                    stars[i] = star;
                }
                
                HandleStarRespawning(cameraPos);
                lastCameraPosition = cameraPos;
                QueueRedraw();
            }
        }
        
        private void HandleStarRespawning(Vector2 cameraPos)
        {
            var rand = new Random();
            var halfViewport = viewportSize * 0.5f;
            
            for (int i = 0; i < stars.Count; i++)
            {
                var star = stars[i];
                var relativePos = star.Position - cameraPos;
                
                if (Mathf.Abs(relativePos.X) > halfViewport.X + spawnMargin ||
                    Mathf.Abs(relativePos.Y) > halfViewport.Y + spawnMargin)
                {
                    Vector2 newPosition;
                    
                    if (Mathf.Abs(relativePos.X) > halfViewport.X + spawnMargin)
                    {
                        var newX = relativePos.X > 0 ? 
                            cameraPos.X - halfViewport.X - spawnMargin * 0.5f : 
                            cameraPos.X + halfViewport.X + spawnMargin * 0.5f;
                        newX += ((float)rand.NextDouble() - 0.5f) * spawnMargin;
                        var newY = cameraPos.Y + ((float)rand.NextDouble() - 0.5f) * 
                            (viewportSize.Y + spawnMargin * 2);
                        newPosition = new Vector2(newX, newY);
                    }
                    else
                    {
                        var newY = relativePos.Y > 0 ? 
                            cameraPos.Y - halfViewport.Y - spawnMargin * 0.5f : 
                            cameraPos.Y + halfViewport.Y + spawnMargin * 0.5f;
                        newY += ((float)rand.NextDouble() - 0.5f) * spawnMargin;
                        var newX = cameraPos.X + ((float)rand.NextDouble() - 0.5f) * 
                            (viewportSize.X + spawnMargin * 2);
                        newPosition = new Vector2(newX, newY);
                    }
                    
                    // Regenerate star with new distance properties
                    var distance = 1.0f - (float)Math.Pow(rand.NextDouble(), 3.0);
                    star.Position = newPosition;
                    star.Size = Mathf.Lerp(StarSizeRange.X, StarSizeRange.Y, distance * distance);
                    star.Speed = Mathf.Lerp(StarSpeedRange.X, StarSpeedRange.Y, distance);
                    star.Color = GetDeepSpaceStarColor(rand, distance);
                    star.Distance = distance;
                    star.Twinkle = (float)rand.NextDouble() * Mathf.Pi * 2;
                    
                    stars[i] = star;
                }
            }
        }
        
        public override void _Draw()
        {
             if (camera == null) return;

            var cameraPos = camera.GlobalPosition;
            var halfViewport = viewportSize * 0.5f;
            
            foreach (Star star in stars)
            {
                var relativePos = star.Position - cameraPos;
                
                if (Mathf.Abs(relativePos.X) <= halfViewport.X + 100f &&
                    Mathf.Abs(relativePos.Y) <= halfViewport.Y + 100f)
                {
                    var localPos = star.Position - GlobalPosition;
                    
                    var twinkleMultiplier = 1.0f + Mathf.Sin(star.Twinkle) * 0.1f * star.Distance;
                    var drawColor = new Color(
                        star.Color.R * twinkleMultiplier,
                        star.Color.G * twinkleMultiplier,
                        star.Color.B * twinkleMultiplier,
                        star.Color.A
                    );
                    
                    var drawSize = star.Size * twinkleMultiplier;
                    
                    DrawCircle(localPos, drawSize, drawColor);
                }
            }
        }
    }
}