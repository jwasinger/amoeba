using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Linq;

public class ZOC
{
    private List<Vector2> nodes;

    public ZOC(Vector2 origin)
    {
        
    }

    public void Update()
    {
        // compute the center point
        Vector2 center = new(
    nodes.Average<Vector2>(p => p.X),
    nodes.Average<Vector2>(p => p.Y));

        // expand the ZOC by moving each point out from the center
        // if nodes on the border become too sparse, create new ones.
        
    }
}

class zocNode
{
    Vector2 Pos;
}