using System;
using System.Collections.Generic;

namespace WpfApp4
{
    public abstract class CellBase
    {

        protected const int HealthLossRate = 1; // Константа потери здоровья

        public abstract int X { get; set; }
        public abstract int Y { get; set; }
        public abstract int DirectionX { get; set; }
        public abstract int DirectionY { get; set; }
        public abstract int Speed { get; set; }
        public abstract int Size { get; set; }
        public abstract int Health { get; set; }
        public abstract int Strength { get; set; }

        public abstract CellBase Divide();
        public abstract void Eat();
        public abstract bool Touches();
        public abstract void UpdatePosition();
    }
}