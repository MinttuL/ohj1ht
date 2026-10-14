using System;
using System.Collections.Generic;
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
    public override void Begin()
    {
        LuoKentta();
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
}