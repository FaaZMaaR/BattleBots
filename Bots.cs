using System.Drawing;
using System;

enum Direction { NONE, UP, DOWN, LEFT, RIGHT, UPLEFT, UPRIGHT, DOWNLEFT, DOWNRIGHT };

class Bot
{
    public Circle body;
    public double hp, damage, speed;
    public Image img;
    public int time;
    public int reload_counter = 0;
    public int reload_time;
    public Rocket[] rockets;
    public Direction direction=Direction.NONE;
    public Bot(int t, string i)
    {
        body = new Circle(0, 0, 20, -Math.PI / 2);
        hp = 100;
        damage = 20;
        speed = 100;
        time = t;
        reload_time = 1000 / time;
        img = Image.FromFile(i);
        rockets = new Rocket[10];
        for(int j = 0; j < rockets.Length; j++)
        {
            rockets[j] = new Rocket(this);
        }
    }
    public void Set_Velocity()
    {
        switch (direction)
        {
            case Direction.UP:
                body.velocity.SetAS(-Math.PI / 2, speed * time / 1000);
                break;
            case Direction.DOWN:
                body.velocity.SetAS(Math.PI / 2, speed * time / 1000);
                break;
            case Direction.LEFT:
                body.velocity.SetAS(Math.PI, speed * time / 1000);
                break;
            case Direction.RIGHT:
                body.velocity.SetAS(0, speed * time / 1000);
                break;
            case Direction.UPLEFT:
                body.velocity.SetAS(-Math.PI * 3 / 4, speed * time / 1000);
                break;
            case Direction.UPRIGHT:
                body.velocity.SetAS(-Math.PI * 1 / 4, speed * time / 1000);
                break;
            case Direction.DOWNLEFT:
                body.velocity.SetAS(Math.PI * 3 / 4, speed * time / 1000);
                break;
            case Direction.DOWNRIGHT:
                body.velocity.SetAS(Math.PI * 1 / 4, speed * time / 1000);
                break;
            default:
                body.velocity.SetNull();
                break;
        }
    }
    public void Update()
    {
        body.Move();
        if (reload_counter > 0)
        {
            reload_counter--;
        }
    }
    public void Fire()
    {
        if (reload_counter > 0) return;
        for(int i=0;i< rockets.Length; i++)
        {
            if (rockets[i].destroyed)
            {
                rockets[i].Init();
                reload_counter = reload_time;
                break;
            }
        }
    }
    public void Draw(Graphics g, float[] map)
    {
        System.Drawing.Drawing2D.GraphicsState state = g.Save();
        g.TranslateTransform((float)body.x + map[0], (float)body.y + map[1]);
        g.RotateTransform((float)(body.angle * 180 / Math.PI + 90));
        g.DrawImage(img, -img.Width / 2, -img.Height / 2);
        g.Restore(state);
    }
    public void DrawHealthBar(Graphics g, Map map)
    {
        g.FillRectangle(new SolidBrush(Color.Red), (float)(body.x - 25 + map.x), (float)(body.y + 35 + map.y), (float)(0.5 * hp), 10);
        g.FillRectangle(new SolidBrush(Color.Black), (float)(body.x - 25 + 0.5 * hp + map.x), (float)(body.y + 35 + map.y), (float)(50 - 0.5 * hp), 10);
    }
}

class Rocket
{
    public double xs, ys;
    public Vector distance = new Vector();
    public Circle body;
    public Bot bot;
    public double speed = 400;
    public double range = 800;
    public bool destroyed = true;
    public Rocket(Bot b)
    {
        bot= b;
        body = new Circle(xs, ys, 10, bot.body.angle);
    }
    public void Init()
    {
        Vector v = new Vector();
        v.SetAS(bot.body.angle, bot.body.radius + 5);
        xs = bot.body.x + v[0];
        ys = bot.body.y + v[1];
        body = new Circle(xs, ys, 10, bot.body.angle);
        body.velocity.SetAS(body.angle, speed * bot.time / 1000);
        destroyed = false;
    }
    public void Update()
    {
        if (destroyed) return;
        distance.SetXY(body.x - xs, body.y - ys);
        if (distance.Scalar2 <= range * range && !body.velocity.IsNull())
        {
            body.Move();
        }
        else
        {
            destroyed = true;
        }
    }
    public void Draw(Graphics g, float[] map)
    {
        if (destroyed) return;
        g.FillEllipse(new SolidBrush(Color.Magenta), (float)body.x + map[0] - 5, (float)body.y + map[1] - 5, 10, 10);
    }
}