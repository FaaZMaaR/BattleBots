using System.Drawing;

class Map
{
    public double x=0;
    public double y=0;
    public Image img;
    public Map(string i)
    {
        img = Image.FromFile(i);
    }
    public void Draw(Graphics g)
    {
        g.DrawImage(img, (float)x, (float)y);
    }
}

class Minimap
{
    public bool[,] cells;
    public Minimap()
    {
        cells = new bool[,]{
            {false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false},
            {false,true,true,true,true,true,true,false,true,true,true,true,true,true,true,true,true,true,true,true,true,false},
            {false,true,true,true,true,true,true,false,true,true,true,true,true,true,true,true,true,true,true,true,true,false},
            {false,false,false,true,true,false,false,false,true,true,false,false,false,false,true,false,false,false,true,true,true,false},
            {false,true,true,true,true,true,true,true,true,true,true,true,true,false,true,false,false,false,false,true,true,false},
            {false,true,true,true,true,true,true,true,true,false,false,false,true,false,true,true,true,true,false,true,true,false},
            {false,true,false,false,true,false,false,true,true,false,false,false,true,false,false,false,false,true,false,true,true,false},
            {false,true,false,false,true,false,false,true,true,true,true,true,true,false,true,true,true,true,false,true,true,false},
            {false,true,true,true,true,true,true,true,false,false,true,true,false,false,true,false,false,false,false,true,true,false},
            {false,true,false,false,true,false,false,false,false,true,true,true,true,false,true,true,true,true,false,true,true,false},
            {false,true,false,false,true,true,true,true,true,true,true,true,true,false,false,false,false,true,false,true,true,false},
            {false,true,true,true,true,false,false,false,false,true,true,true,true,true,true,true,true,true,false,true,true,false},
            {false,false,false,false,true,false,false,false,false,true,true,true,true,false,false,false,false,false,false,true,true,false},
            {false,true,true,true,true,true,true,true,false,false,true,true,false,false,true,true,true,true,true,true,true,false},
            {false,true,false,true,true,true,true,false,false,false,true,true,true,true,true,true,true,true,true,true,true,false},
            {false,true,false,true,false,false,false,false,true,true,true,true,false,false,false,false,true,false,false,false,true,false},
            {false,true,false,true,true,true,true,true,true,true,true,true,false,false,false,false,true,true,true,false,true,false},
            {false,true,true,true,true,true,true,true,true,false,true,true,false,false,false,false,true,true,true,false,true,false},
            {false,true,false,false,false,true,true,true,false,false,true,true,false,false,false,false,true,true,true,false,true,false},
            {false,true,false,false,true,true,false,true,true,true,true,true,true,true,true,true,true,true,false,false,true,false},
            {false,true,true,true,true,true,true,true,false,false,false,false,false,false,true,true,true,true,true,true,true,false},
            {false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false}
        };
    }
    public void Draw(Graphics g)
    {
        int side = 10;
        for (int i = 0; i < cells.GetLength(0); i++)
        {
            for (int j = 0; j < cells.GetLength(1); j++)
            {
                if (cells[i, j]) g.FillRectangle(new SolidBrush(Color.White), 1300 + j * side, 50 + i * side, side, side);
                else g.FillRectangle(new SolidBrush(Color.Black), 1300 + j * side, 50 + i * side, side, side);
            }
        }
    }
}