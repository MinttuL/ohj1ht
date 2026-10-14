using System;
using System.Collections.Generic;
using System.Threading;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Widgets;

/// @author mintt
/// @version 13.10.2026
/// <summary>
/// 
/// </summary>
public class Kolikkokerailija : PhysicsGame
{
    private PhysicsObject pallo;

    public override void Begin()
    {
        LuoKentta();
        LuoPelaaja();
        AsetaOhjaimet();
        // Kirjoita ohjelmakoodisi tähän

        PhoneBackButton.Listen(ConfirmExit, "Lopeta peli");
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");
    }

// Luodaan pelimaailmalle rajat ja tausta
    private void LuoKentta()
    {
        Level.Width = 1200;
        Level.Height = 800;

        Level.CreateBorders();

        Level.BackgroundColor = Color.LightBlue;

        PhysicsObject maa = PhysicsObject.CreateStaticObject(Level.Width, 150);
        maa.Y = Level.Bottom + 50;
        maa.Color = Color.ForestGreen;
        Add(maa);

        Camera.ZoomToLevel();
    }

    private void LuoPelaaja()
    {
        pallo = new PhysicsObject(50.0, 50.0);
        pallo.Shape = Shape.Circle;
        pallo.Color = Color.Pink;
        pallo.X = -0.0;
        pallo.Y = -300.0;
        Add(pallo);
    }

    private void AsetaOhjaimet()
    {
        Keyboard.Listen(Key.Left, ButtonState.Down, LiikutaPalloa, "Liiku vasemmalle", new Vector(-1000, 0));
        Keyboard.Listen(Key.Right, ButtonState.Down, LiikutaPalloa, "Liiku oikealle", new Vector(1000, 0));
    }

    private void LiikutaPalloa(Vector voima)
    {
        pallo.Push(voima);
    }
}