using System.Collections;
using System;

class Bot_Handler
{
    public Bot bot;
    public Minimap map, cur_map;
    public double turn_speed = Math.PI/2;
    public double duration = 0;
    public double counter = 0;
    public double angle_vision = Math.PI / 4;
    public double radius_vision = 300;
    public string state = "idle";
    public string instruction = "";
    public bool ready = true;
    public bool enemy_spotted = false;
    public bool enemy_lost = false;
    public int[] destination;
    public int[] cur_position;
    public double[] next_position = new double[] { 0, 0 };
    public ArrayList path;
    public Queue cmds;
    public Bot_Handler(Bot b)
    {
        bot = b;
        map = new Minimap();
    }
    public void GetCommand(string s)
    {
        string[] cmd = s.Split();
        state = cmd[0];
        duration = Int32.Parse(cmd[1]);
        counter = 0;
        ready = false;
    }
    public void Act(string s)
    {
        if (ready)
        {
            GetCommand(s);
            if (state == "up") bot.direction = Direction.UP;
            else if (state == "down") bot.direction = Direction.DOWN;
            else if (state == "left") bot.direction = Direction.LEFT;
            else if (state == "right") bot.direction = Direction.RIGHT;
            else bot.direction = Direction.NONE;
            bot.Set_Velocity();
        }
        else
        {
            if (state == "idle")
            {
                if (counter < duration)
                {
                    counter++;
                }
                else
                {
                    ready = true;
                }
            }
            else if(state=="up" || state=="down" || state=="left" || state == "right")
            {
                if (counter < duration * 160)
                {
                    bot.body.Move();
                    counter += bot.body.velocity.Scalar;
                }
                else
                {
                    ready = true;
                }
            }
        }
    }
    public bool Check_Cell(int x, int y)
    {
        if (cur_map.cells[x, y])
        {
            cur_map.cells[cur_position[0], cur_position[1]] = false;
            cur_position[0] = x;
            cur_position[1] = y;
            path.Add(new int[] { x, y });
            return true;
        }
        else return false;
    }
    public int[] GetPosition()
    {
        return new int[] { (int)(bot.body.y - 640) / 160, (int)(bot.body.x - 640) / 160 };
    }
    public void Reach(int m, int n)
    {
        destination = new int[] { m, n };
        cur_position = GetPosition();
        map = new Minimap();
        cur_map = new Minimap();
        path = new ArrayList();
        cmds = new Queue();
        int i, j;
        if (cur_position[0] == destination[0] && cur_position[1] == destination[1]) return;
        while (cur_position[0] != destination[0] || cur_position[1] != destination[1])
        {
            i = destination[0] - cur_position[0];
            if (i > 0)
            {
                if (Check_Cell(cur_position[0] + 1, cur_position[1])) continue;
            }
            if (i < 0)
            {
                if (Check_Cell(cur_position[0] - 1, cur_position[1])) continue;
            }
            j = destination[1] - cur_position[1];
            if (j > 0)
            {
                if (Check_Cell(cur_position[0], cur_position[1] + 1)) continue;
            }
            if (j < 0)
            {
                if (Check_Cell(cur_position[0], cur_position[1] - 1)) continue;
            }
            if (i > 0)
            {
                if (Check_Cell(cur_position[0] - 1, cur_position[1])) continue;
            }
            if (i < 0)
            {
                if (Check_Cell(cur_position[0] + 1, cur_position[1])) continue;
            }
            if (i == 0)
            {
                if (Check_Cell(cur_position[0] + 1, cur_position[1])) continue;
                if (Check_Cell(cur_position[0] - 1, cur_position[1])) continue;
            }
            if (j > 0)
            {
                if (Check_Cell(cur_position[0], cur_position[1] - 1)) continue;
            }
            if (j < 0)
            {
                if (Check_Cell(cur_position[0], cur_position[1] + 1)) continue;
            }
            if (j == 0)
            {
                if (Check_Cell(cur_position[0], cur_position[1] + 1)) continue;
                if (Check_Cell(cur_position[0], cur_position[1] - 1)) continue;
            }
            map.cells[cur_position[0], cur_position[1]] = false;
            for (int k = 0; k < map.cells.GetLength(0); k++)
            {
                for (int l = 0; l < map.cells.GetLength(1); l++)
                {
                    cur_map.cells[k, l] = map.cells[k, l];
                }
            }
            cur_position = GetPosition();
            path.Clear();
        }
        cur_position = GetPosition();
        foreach (int[] v in path)
        {
            if (v[0] == cur_position[0])
            {
                if (v[1] > cur_position[1])
                {
                    cmds.Enqueue("right 1");
                }
                else
                {
                    cmds.Enqueue("left 1");
                }
            }
            else if (v[1] == cur_position[1])
            {
                if (v[0] > cur_position[0])
                {
                    cmds.Enqueue("down 1");
                }
                else
                {
                    cmds.Enqueue("up 1");
                }
            }
            cur_position[0] = v[0];
            cur_position[1] = v[1];
        }
        cmds.Enqueue("idle 10");
    }
    public void Turn()
    {
        if(bot.direction==Direction.NONE) return;
        double ang = 0;
        switch (bot.direction)
        {
            case Direction.UP:
                ang = -Math.PI / 2;
                break;
            case Direction.DOWN:
                ang = Math.PI / 2;
                break;
            case Direction.LEFT:
                if(bot.body.angle<0) ang = -Math.PI;
                else ang = Math.PI;
                break;
            case Direction.RIGHT:
                ang = 0;
                break;
        }
        if (bot.body.angle < ang)
        {
            bot.body.angle += turn_speed * bot.time / 1000;
        }
        else if(bot.body.angle > ang)
        {
            bot.body.angle -= turn_speed * bot.time / 1000;
        }

    }
    public void Follow(Bot enemy)
    {
        bot.body.angle = Math.Atan2(enemy.body.y - bot.body.y, enemy.body.x - bot.body.x);
    }
    public void Check_Vision(Bot enemy)
    {
        Vector v = new Vector();
        double d = 0;
        v.SetXY(enemy.body.x - bot.body.x, enemy.body.y - bot.body.y);
        d = bot.body.angle - v.Angle;
        if (d < -Math.PI) d += Math.PI * 2;
        else if (d > Math.PI) d -= Math.PI * 2;
        if ((d >= -angle_vision && d <= angle_vision) && (v.Scalar2 <= radius_vision * radius_vision))
        {
            enemy_spotted = true;
        }
        else
        {
            if (enemy_spotted) enemy_lost = true;
            enemy_spotted = false;
        }
    }
    public void UpdateVision(Bot enemy)
    {
        Check_Vision(enemy);
        if (enemy_spotted)
        {
            Follow(enemy);
            bot.Fire();
        }
    }
    public void Update(Bot target)
    {
        if (enemy_spotted)
        {
            instruction = "idle 1";
        }
        else
        {
            if (enemy_lost)
            {
                cmds.Clear();
                enemy_lost = false;
            }
            Turn();
            if (cmds.Count != 0)
            {
                if (ready) instruction = (string)cmds.Dequeue();
            }
            else
            {
                instruction = "idle 1";
                Reach((int)(target.body.y - 640) / 160, (int)(target.body.x - 640) / 160);
            }
        }        
        Act(instruction);
        if (bot.reload_counter > 0)
        {
            bot.reload_counter--;
        }
    }
}