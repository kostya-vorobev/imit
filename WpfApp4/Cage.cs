using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WpfApp4
{
    public class Cage
    {
        private Ellipse ellipse;
        private int size = 30;
        private int posX;
        private int posY;
        private bool hasLive;
        private int health;
        private int damageStep = 20;
        private Random random = new Random();

        public Cage(int posX, int posY)
        {
            this.ellipse = new Ellipse
            {
                Height = size,
                Width = size,
                Fill = Brushes.Red,
                StrokeThickness = 1,
                Stroke = Brushes.Black
            };

            this.PosX = posX;
            this.PosY = posY;
            var rand = random.Next(5);
            this.HasLive = rand == 0;
            this.health = 100;
        }

        public Cage(Cage cage)
        {
            this.ellipse = new Ellipse
            {
                Height = size,
                Width = size,
                Fill = Brushes.Red,
                StrokeThickness = 1,
                Stroke = Brushes.Black
            };

            this.PosX = cage.posX;
            this.PosY = cage.PosY;
            this.HasLive = true;
            this.health = 100;
        }

        public Ellipse Elips { get => ellipse; set => ellipse = value; }
        public int PosX { get => posX; set => posX = value; }
        public int PosY { get => posY; set => posY = value; }
        public bool HasLive { get => hasLive; set => hasLive = value; }

        ~Cage()
        {
            this.HasLive = false;
        }

        public void ToLeft()
        {
            if (posX == 0)
                return;
            posX -= 10;
            this.health -= damageStep;
        }
        public void ToRight()
        {
            posX += 10;
            this.health -= damageStep;
        }
        public void ToTop()
        {
            if (posY == 0)
                return;
            posY -= 10;
            this.health -= damageStep;
        }
        public void ToBottom()
        {
            posY += 10;
            this.health -= damageStep;
        }
        public bool IsLive()
        {
            return this.health <= 0;
        }
    }
}
