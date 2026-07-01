using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace DungeonSlime;


public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private PrimitiveLineBatch PrimitiveLineBatch;
    private VertexPositionColor[] vertices;
private BasicEffect effect;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        effect = new BasicEffect(GraphicsDevice)
        {
            VertexColorEnabled = true,
            TextureEnabled = false,
            LightingEnabled = false,
            World = Matrix.Identity,
            View = Matrix.Identity,
            Projection = Matrix.CreateOrthographicOffCenter(
                0,
                GraphicsDevice.Viewport.Width,
                GraphicsDevice.Viewport.Height,
                0,
                0,
                1)
        };
        // TODO: use this.Content to load your game content here
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // TODO: Add your update logic here

        base.Update(gameTime);
    }

    public static List<Vector2> GenerateCatmullRom(
    IReadOnlyList<Vector2> controlPoints,
    int samplesPerSegment = 20)
{
    var result = new List<Vector2>();

    if (controlPoints.Count < 2)
        return result;

    for (int i = 0; i < controlPoints.Count - 1; i++)
    {
        Vector2 p0 = i == 0
            ? controlPoints[i]
            : controlPoints[i - 1];

        Vector2 p1 = controlPoints[i];
        Vector2 p2 = controlPoints[i + 1];

        Vector2 p3 = i + 2 >= controlPoints.Count
            ? controlPoints[i + 1]
            : controlPoints[i + 2];

        for (int j = 0; j < samplesPerSegment; j++)
        {
            float t = j / (float)samplesPerSegment;

            result.Add(Vector2.CatmullRom(
                p0,
                p1,
                p2,
                p3,
                t));
        }
    }

    // Add the final endpoint.
    result.Add(controlPoints[^1]);

    return result;
}

    protected override void Draw(GameTime gameTime)
    {
        PrimitiveLineBatch = new PrimitiveLineBatch(_graphics.GraphicsDevice);

        GraphicsDevice.Clear(Color.CornflowerBlue);

        // TODO: Add your drawing code here

       // PrimitiveLineBatch.Begin();
        List<Vector2> towns =
        [
            new(100, 300),
            new(200, 100),
            new(350, 180),
            new(500, 350),
            new(700, 250)
        ];

        var spline = GenerateCatmullRom(towns, 20);

       this.vertices =
            spline.Select(p => new VertexPositionColor(
                new Vector3(p, 0),
                Color.Gold))
            .ToArray();

    foreach (EffectPass pass in effect.CurrentTechnique.Passes)
    {
        pass.Apply();

        GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(
            PrimitiveType.LineStrip,
            vertices,
            0,
            vertices.Length - 1);
    }

        base.Draw(gameTime);
    }
}

public sealed class PrimitiveLineBatch
{
    private readonly GraphicsDevice graphicsDevice;
    private readonly BasicEffect effect;

    private readonly List<VertexPositionColor> vertices = new();

    public PrimitiveLineBatch(GraphicsDevice graphicsDevice)
    {
        this.graphicsDevice = graphicsDevice;

        effect = new BasicEffect(graphicsDevice)
        {
            VertexColorEnabled = true,
            TextureEnabled = false,
            LightingEnabled = false,
            Projection = Matrix.CreateOrthographicOffCenter(
                0,
                graphicsDevice.Viewport.Width,
                graphicsDevice.Viewport.Height,
                0,
                0,
                1),
            View = Matrix.Identity,
            World = Matrix.Identity
        };
    }

    public void Begin()
    {
        vertices.Clear();
    }

    public void DrawLine(Vector2 a, Vector2 b, Color color)
    {
        vertices.Add(new VertexPositionColor(
            new Vector3(a, 0),
            color));

        vertices.Add(new VertexPositionColor(
            new Vector3(b, 0),
            color));
    }

    public void End()
    {
        if (vertices.Count == 0)
            return;

        foreach (var pass in effect.CurrentTechnique.Passes)
        {
            pass.Apply();

            graphicsDevice.DrawUserPrimitives<VertexPositionColor>(
                PrimitiveType.LineList,
                vertices.ToArray(),
                0,
                vertices.Count / 2);
        }
    }

}
