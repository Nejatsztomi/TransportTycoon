using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace TransportTycoon.Model
{
    public enum Direction
    {
        Up = 0, Down = 1, Left = 2, Right = 3
    }
    public enum VehicleType
    {
        Van=0,Pickup=1,Truck=2,LiquidTruck=3,SmallBus=4,BigBus=5
    }
    public abstract class Vehicle
    {
        #region Fields
        public int TopSpeed { get; protected set; }
        public int CurrentSpeed { get; protected set; }
        public Load? CurrentLoad { get; protected set; }
        public int MaxCapacity { get; protected set; }
        public int CurrentCapacity { get; protected set; }
        public Prouth? Route { get; protected set; }
        public VehicleType Type { get; protected set; }
        public int X { get; protected set; }
        public int Y { get; protected set; }
        public Direction Direction { get; protected set; }
        public int Price { get; protected set; }
        public int Maintance { get; protected set; }
        #endregion

        #region Public methods
        public void ChangeSpeed(int speed)
        {
            CurrentSpeed = speed;
        }

        public void Step(Direction dir)
        {
            switch (dir)
            {
                case Direction.Up:
                    Y--;
                    break;
                case Direction.Down:
                    Y++;
                    break;
                case Direction.Left:
                    X--;
                    break;
                case Direction.Right:
                    X++;
                    break;
                default:
                    break;
            }
        }

        public int Load(int quantity, Load load) //returns leftover
        {
            if (CurrentLoad != load) return quantity;
            else if (CurrentLoad == null)
            {
                CurrentLoad = load;
                if (quantity <= MaxCapacity)
                {
                    CurrentCapacity = quantity;
                    return 0;
                }
                else
                {
                    CurrentCapacity = MaxCapacity;
                    return quantity - MaxCapacity;
                }
            }
            else if (CurrentLoad != null){
                if (CurrentCapacity + quantity <= MaxCapacity)
                {
                    CurrentCapacity = quantity;
                    return 0;
                }
                else
                {
                    CurrentCapacity = MaxCapacity;
                    return quantity - (MaxCapacity - CurrentCapacity);
                }
            }
            return quantity;
        }

        public int UnLoad(int quantity, Load load) //returns unloaded quantity
        {
            if (CurrentLoad == null || CurrentLoad != load) return 0;
            else if (quantity < CurrentCapacity)
            {
                CurrentCapacity -= quantity;
                return quantity;
            }
            else if (quantity == CurrentCapacity)
            {
                CurrentCapacity -= quantity;
                CurrentLoad = null;
                return quantity;
            }
            else if (quantity > CurrentCapacity)
            {
                int tmp = CurrentCapacity;
                CurrentCapacity = 0;
                CurrentLoad = null;
                return tmp;
            }
            return 0;
        }
        #endregion
    }
}
