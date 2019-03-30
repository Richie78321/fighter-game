using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FighterGame.Entities.Player;
using PolygonalPhysics;
using FighterGame.Entities;

namespace FighterGame.Map
{
    class Camera
    {
        public const float PLAYER_VIEW_PADDING = 8F;
        private const float SCALE_ACCELERATION = .025F;
        private const float TARGET_FRAME_TIME = 1000F / 60;

        private const float VIEWPOINT_ACCELERATION = .080F;

        //Object
        private GameMap map;

        private AcceleratingFloat acceleratingScale;
        public float Scale => acceleratingScale.CurrentValue;

        private AcceleratingFloat acceleratingFocusX, acceleratingFocusY;
        public Vector2 FocusPoint => new Vector2(acceleratingFocusX.CurrentValue, acceleratingFocusY.CurrentValue);

        public Camera(GameMap map, GraphicsDevice graphicsDevice)
        {
            this.map = map;

            acceleratingScale = new AcceleratingFloat(SCALE_ACCELERATION);
            acceleratingFocusX = new AcceleratingFloat(VIEWPOINT_ACCELERATION);
            acceleratingFocusY = new AcceleratingFloat(VIEWPOINT_ACCELERATION);

            Vector2 targetFocusPoint;
            acceleratingScale.TargetValue = FindTargetScale(graphicsDevice, out targetFocusPoint);
            acceleratingFocusX.TargetValue = targetFocusPoint.X;
            acceleratingFocusY.TargetValue = targetFocusPoint.Y;
        }

        private float FindTargetScale(GraphicsDevice graphicsDevice, out Vector2 focusPoint)
        {
            Player[] players = map.Players.Where(t => !t.IsDead).ToArray();
            NetworkPlayer[] networkPlayers = map.NetworkPlayers.Where(t => !t.IsDead).ToArray();
            if (players.Length + networkPlayers.Length > 0)
            {
                //Find boundary rectangles
                RectangleF[] boundaryRectangles = new RectangleF[players.Length + networkPlayers.Length];
                for (int i = 0; i < players.Length; i++) boundaryRectangles[i] = new RectangleF(players[i].RigidBody.CollisionPolygon.CenterPoint.X - ((players[i].DrawDimensions.X / 2) * PLAYER_VIEW_PADDING), players[i].RigidBody.CollisionPolygon.CenterPoint.Y - ((players[i].DrawDimensions.Y / 2) * PLAYER_VIEW_PADDING), players[i].DrawDimensions.X * PLAYER_VIEW_PADDING, players[i].DrawDimensions.Y * PLAYER_VIEW_PADDING);
                for (int i = 0; i < networkPlayers.Length; i++) boundaryRectangles[players.Length + i] = new RectangleF(networkPlayers[i].PlayerPosition.X - ((networkPlayers[i].DrawDimensions.X / 2) * PLAYER_VIEW_PADDING), networkPlayers[i].PlayerPosition.Y - ((networkPlayers[i].DrawDimensions.Y / 2) * PLAYER_VIEW_PADDING), networkPlayers[i].DrawDimensions.X * PLAYER_VIEW_PADDING, networkPlayers[i].DrawDimensions.Y * PLAYER_VIEW_PADDING);

                //Merge rectangles
                float minX = boundaryRectangles[0].X, minY = boundaryRectangles[0].Y, maxX = boundaryRectangles[0].X + boundaryRectangles[0].Width, maxY = boundaryRectangles[0].Y + boundaryRectangles[0].Height;
                for (int i = 1; i < boundaryRectangles.Length; i++)
                {
                    if (boundaryRectangles[i].X < minX) minX = boundaryRectangles[i].X;
                    if (boundaryRectangles[i].Y < minY) minY = boundaryRectangles[i].Y;

                    float value = boundaryRectangles[i].X + boundaryRectangles[i].Width;
                    if (value > maxX) maxX = value;
                    value = boundaryRectangles[i].Y + boundaryRectangles[i].Height;
                    if (value > maxY) maxY = value;
                }
                RectangleF mergeRectangle = new RectangleF(minX, minY, maxX - minX, maxY - minY);

                focusPoint = new Vector2(mergeRectangle.X + (mergeRectangle.Width / 2), mergeRectangle.Y + (mergeRectangle.Height / 2));
                float widthScale = graphicsDevice.Viewport.Width / mergeRectangle.Width;
                float heightScale = graphicsDevice.Viewport.Height / mergeRectangle.Height;
                if (widthScale < heightScale) return widthScale;
                else return heightScale;
            }
            else
            {
                //Fit to screen
                float smallestDimension = graphicsDevice.Viewport.Width;
                if (graphicsDevice.Viewport.Height < smallestDimension) smallestDimension = graphicsDevice.Viewport.Height;

                focusPoint = new Vector2(MapStandards.MAP_SIZE / 2, MapStandards.MAP_SIZE / 2);
                return smallestDimension / MapStandards.MAP_SIZE;
            }
        }

        public void Update(GraphicsDevice graphicsDevice, GameTime gameTime)
        {
            Vector2 targetFocusPoint;
            acceleratingScale.TargetValue = FindTargetScale(graphicsDevice, out targetFocusPoint);
            acceleratingFocusX.TargetValue = targetFocusPoint.X;
            acceleratingFocusY.TargetValue = targetFocusPoint.Y;

            acceleratingFocusX.Update(gameTime);
            acceleratingFocusY.Update(gameTime);
            acceleratingScale.Update(gameTime);
        }
    }

    public class AcceleratingFloat
    {
        public float TargetValue = 0;

        private float _currentValue = 0;
        public float CurrentValue => _currentValue;

        private float valueAcceleration;
        private float currentVelocity;

        public AcceleratingFloat(float valueAcceleration)
        {
            this.valueAcceleration = valueAcceleration;
        }

        public float Update(GameTime gameTime)
        {
            bool willAccelerate = false;
            if (TargetValue - _currentValue > 0)
            {
                //Should accelerate
                willAccelerate = !((((Math.Abs(currentVelocity) / valueAcceleration) + 2) * currentVelocity) + _currentValue >= TargetValue);
            }
            else if (TargetValue - _currentValue < 0)
            {
                willAccelerate = !((((Math.Abs(currentVelocity) / valueAcceleration) + 2) * currentVelocity) + _currentValue <= TargetValue);
            }

            if (!willAccelerate)
            {
                //Deccelerate
                currentVelocity += MathHelper.Clamp(-currentVelocity, -valueAcceleration, valueAcceleration);
            }
            else
            {
                //Accelerate
                if (Math.Abs(TargetValue - _currentValue) >= valueAcceleration)
                {
                    if (TargetValue - _currentValue > 0)
                    {
                        currentVelocity += valueAcceleration;
                    }
                    else
                    {
                        currentVelocity -= valueAcceleration;
                    }
                }
            }

            _currentValue += currentVelocity;
            return _currentValue;
        }
    }
}
