using System;
using System.Collections.Generic;
using System.Threading;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Widgets;
using Timer = Jypeli.Timer;

/// @author minttu lyytinen
/// @version 13.10.2026
/// <summary>
/// Kolikkokeräilijä -peli Ohjelmointi 1 -kurssi
/// </summary>
public class Kolikkokerailija : PhysicsGame
{
    private PhysicsObject pallo;
    private List<PhysicsObject> aktiivisetKolikot = new List<PhysicsObject>();

    public override void Begin()
    {
        Gravity = new Vector(0, -400);
        
        LuoKentta();
        LuoPelaaja();
        AsetaOhjaimet();
        LuoKolikkoAjastin();
        // Kirjoita ohjelmakoodisi tähän

        PhoneBackButton.Listen(ConfirmExit, "Lopeta peli");
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");
    }
/// <summary>
/// Luodaan pelikentälle rajat ja tausta
/// </summary>
    private void LuoKentta()
    {
        Level.Width = 1200;
        Level.Height = 800;

        Level.CreateBorders();

        Level.Background.Color = Color.LightBlue;

        PhysicsObject maa = PhysicsObject.CreateStaticObject(Level.Width, 150);
        maa.Y = Level.Bottom + 50;
        maa.Color = Color.ForestGreen;
        Add(maa);

        Camera.ZoomToLevel();
    }
/// <summary>
/// Luodaan pelaaja
/// </summary>
    private void LuoPelaaja()
    {
        pallo = new PhysicsObject(50.0, 50.0);
        pallo.Shape = Shape.Circle;
        pallo.Color = Color.Pink;
        pallo.X = -0.0;
        pallo.Y = -300.0;
        Add(pallo);
    }
/// <summary>
/// Asetetaan ohjaimet
/// </summary>
    private void AsetaOhjaimet()
    {
        Keyboard.Listen(Key.Left, ButtonState.Down, LiikutaPalloa, "Liiku vasemmalle", -1000.0);
        Keyboard.Listen(Key.Right, ButtonState.Down, LiikutaPalloa, "Liiku oikealle", 1000.0);
        
        Keyboard.Listen(Key.Left, ButtonState.Released, LiikutaPalloa, null, 0.0);
        Keyboard.Listen(Key.Right, ButtonState.Released, LiikutaPalloa, null, 0.0);
    }
/// <summary>
/// Asetetaan pallon liikenopeus
/// </summary>
/// <param name="nopeus"></param>
    private void LiikutaPalloa(double nopeus)
    {
        pallo.Velocity = new Vector(nopeus, pallo.Velocity.Y);
    }

/// <summary >
/// Luodaan ajastin, joka luo uuden kolikon säännöllisin välein
/// </summary>
    private void LuoKolikkoAjastin()
    {
        Timer kolikkoAjastin = new Timer();
        kolikkoAjastin.Interval = 1.5; //uusi kolikko 1,5 sekunnin välein
        kolikkoAjastin.Timeout += LuoKolikko;
        kolikkoAjastin.Start();
    }

    private void LuoKolikko()
    {
        PhysicsObject kolikko = new PhysicsObject(30.0, 30.0);
        kolikko.Shape = Shape.Circle;
        kolikko.Color = Color.Yellow;

        double satunnainenX = RandomGen.NextDouble(Level.Left + 50, Level.Right - 50.0);
        kolikko.X = satunnainenX;
        kolikko.Y = Level.Top - 40;
        
        aktiivisetKolikot.Add(kolikko);

        AddCollisionHandler(pallo, kolikko, OsumaKolikkoon);
        
        Add(kolikko);
    }
/// <summary>
/// Poistetaan kerätty kolikko listasta ja pelialueelta
/// </summary>
/// <param name="pelaaja"></param>
/// <param name="kolikko"></param>
    private void OsumaKolikkoon(PhysicsObject pelaaja, PhysicsObject kolikko)
    {
        aktiivisetKolikot.Remove(kolikko);
        kolikko.Destroy();
    }
    
}