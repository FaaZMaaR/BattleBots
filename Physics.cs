using System;
using System.Drawing;

class Vector
{
    private double x,y;
    public Vector()
    {
        x = 0;
        y = 0;
    }
    public Vector(double a,double b)
    {
        x = a;
        y = b;
    }
    public double Scalar
    {
        get
        {
            return Math.Sqrt(x * x + y * y);
        }
    }
    public double Scalar2
    {
        get
        {
            return x * x + y * y;
        }
    }
    public double Angle
    {
        get
        {
            return Math.Atan2(y, x);
        }
    }
    public void SetAS(double ang, double s)
    {
        x = s * Math.Cos(ang);
        y = s * Math.Sin(ang);
    }
    public void SetXY(double a, double b)
    {
        x = a;
        y = b;
    }
    public void SetNull()
    {
        x = 0;
        y = 0;
    }
    public void Scale(double s)
    {
        x *= s;
        y *= s;
    }
    public void Scale(double a, double b)
    {
        x *= a;
        y *= b;
    }
    public void Translate(double a, double b)
    {
        x += a;
        y += b;
    }
    public void Rotate(double ang)
    {
        double a = x;
        double b = y;
        x = a * Math.Cos(ang) - b * Math.Sin(ang);
        y = a * Math.Sin(ang) + b * Math.Cos(ang);
    }
    public Vector Normalize()
    {
        Vector v = new Vector();
        v.x = x;
        v.y = y;
        v.Scale(1 / Scalar, 1 / Scalar);
        return v;
    }
    public bool IsNull()
    {
        if(x == 0 && y == 0) return true;
        else return false;
    }
    public double this[int i]
    {
        get
        {
            if(i == 0) return x;
            else return y;
        }
    }
    public static Vector operator +(Vector a, Vector b)
    {
        return new Vector(a.x + b.x, a.y + b.y);
    }
    public static Vector operator -(Vector a, Vector b)
    {
        return new Vector(a.x - b.x, a.y - b.y);
    }
    public static double operator *(Vector a, Vector b)
    {
        return a.x * b.x + a.y * b.y;
    }
}

class Circle
{
    public double x, y, radius, angle;
    public Vector velocity;
    public Circle()
    {
        x = 0; y = 0; radius = 0; angle = 0;
        velocity = new Vector();
    }
    public Circle(double a,double b,double r,double ang)
    {
        x=a; y=b; radius=r; angle=ang;
        velocity = new Vector();
    }
    public double NextX
    {
        get
        {
            return x + velocity[0];
        }
    }
    public double NextY
    {
        get
        {
            return y + velocity[1];
        }
    }
    public void Move()
    {
        x += velocity[0];
        y += velocity[1];
    }
}

class Box
{
    public double x, y, width, height;
    public Color color = Color.Green;
    public Box(float a, float b, float w, float h)
    {
        x = a;
        y = b;
        width = w;
        height = h;
    }
    public void Draw(Graphics g, float[] map)
    {
        g.FillRectangle(new SolidBrush(color), (float)x + map[0], (float)y + map[1], (float)width, (float)height);
    }
}

class PhysHandler
{
    public bool IsCircleBoxCollide(Circle c,Box b)
    {
        if (((c.NextX + c.radius) > (b.x) && (c.NextX - c.radius) < (b.x+b.width)) &&
            ((c.NextY + c.radius) > (b.y) && (c.NextY - c.radius) < (b.y + b.height)))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool IsCircleInsideBoxCollide(Circle c,Box b)
    {
        if ((c.NextX - c.radius)<b.x || (c.NextX + c.radius)>b.x+b.width ||
            (c.NextY - c.radius) < b.y || (c.NextY + c.radius) > b.y + b.height)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool IsCirclesCollide(Circle a,Circle b)
    {
        Vector v = new Vector(b.NextX - a.NextX, b.NextY - a.NextY);
        if (v.Scalar2 <= (a.radius + b.radius) * (a.radius + b.radius))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public void ResolveCircleBoxCollision(Circle c,Box b)
    {
        if (IsCircleBoxCollide(c, b))
        {
            c.velocity.SetNull();
        }
    }
    public void ResolveCircleInsideBoxCollision(Circle c,Box b)
    {
        if(IsCircleInsideBoxCollide(c, b))
        {
            c.velocity.SetNull();
        }
    }
    public void ResolveRocketHit(Rocket r,Bot b)
    {
        if (IsCirclesCollide(r.body, b.body))
        {
            r.destroyed = true;
            b.hp-=r.bot.damage;
        }
    }
}