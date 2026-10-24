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
    private const double PELIN_KESTO = 60.0; //Ei vielä asetettu koodiin
    private const double HAHMON_NOPEUS = 1000.0;

    private static readonly Image PRINSESSA_KUVA = LoadImage("prinsessa.png");
    private static readonly Image KUKKA_KUVA = LoadImage("kukka.png");

    private Timer kolikkoAjastin;
    private IntMeter pistelaskuri;
    private DoubleMeter aikalaskuri;
    private PhysicsObject suorakolmio;
    private List<PhysicsObject> aktiivisetKolikot = new List<PhysicsObject>();

    public override void Begin()
    {
        Gravity = new Vector(0, -400);

        LuoKentta();
        LuoPelaaja();
        AsetaOhjaimet();
        LuoKolikkoAjastin();
        LuoPistelaskuri();
        LuoAikalaskuri();
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
        suorakolmio = new PhysicsObject(150.0, 200.0);
        suorakolmio.Shape = Shape.Rectangle;
        suorakolmio.Image = PRINSESSA_KUVA;

        suorakolmio.X = -0.0;
        suorakolmio.Y = -300.0;
        suorakolmio.Restitution = 0.0;

        suorakolmio.MomentOfInertia = Double.PositiveInfinity;

        Add(suorakolmio);
    }

    /// <summary>
    /// Asetetaan ohjaimet
    /// </summary>
    private void AsetaOhjaimet()
    {
        Keyboard.Listen(Key.Left, ButtonState.Down, LiikutaHahmoa, "Liiku vasemmalle", -HAHMON_NOPEUS);
        Keyboard.Listen(Key.Right, ButtonState.Down, LiikutaHahmoa, "Liiku oikealle", HAHMON_NOPEUS);

        Keyboard.Listen(Key.Left, ButtonState.Released, LiikutaHahmoa, null, 0.0);
        Keyboard.Listen(Key.Right, ButtonState.Released, LiikutaHahmoa, null, 0.0);
    }

    /// <summary>
    /// Asetetaan hahmon liikenopeus
    /// </summary>
    /// <param name="nopeus"></param>
    private void LiikutaHahmoa(double nopeus)
    {
        suorakolmio.Velocity = new Vector(nopeus, suorakolmio.Velocity.Y);
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
        PhysicsObject kolikko = new PhysicsObject(50.0, 50.0);
        kolikko.Shape = Shape.Circle;
        kolikko.Color = Color.Yellow;
        kolikko.Image = KUKKA_KUVA;

        double satunnainenX = RandomGen.NextDouble(Level.Left + 50, Level.Right - 50.0);
        kolikko.X = satunnainenX;
        kolikko.Y = Level.Top - 40;

        aktiivisetKolikot.Add(kolikko);

        AddCollisionHandler(suorakolmio, kolikko, OsumaKolikkoon);

        Add(kolikko);
    }

    /// <summary>
    /// Poistetaan kerätty kolikko listasta ja pelialueelta
    /// </summary>
    /// <param name="pelaaja"></param>
    /// <param name="kolikko"></param>
    private void OsumaKolikkoon(PhysicsObject pelaaja, PhysicsObject kolikko)
    {
        pistelaskuri.Value += 1;
        aktiivisetKolikot.Remove(kolikko);
        kolikko.Destroy();
    }

    /// <summary>
    /// Luodaan pistelaskuri näkyviin
    /// </summary>
    private void LuoPistelaskuri()
    {
        pistelaskuri = new IntMeter(0);

        Label pistenaytto = new Label();
        pistenaytto.X = Screen.Left + 50;
        pistenaytto.Y = Screen.Top - 100;
        pistenaytto.Color = Color.LightPink;
        pistenaytto.Title = ("Pisteet:");
        pistenaytto.BindTo(pistelaskuri);

        Add(pistenaytto);
    }

    /// <summary>
    /// Luodaan aikalaskuri
    /// </summary>
    private void LuoAikalaskuri()
    {
        aikalaskuri = new DoubleMeter(PELIN_KESTO);

        Timer aikaAjastin = new Timer();
        aikaAjastin.Interval = 0.1;
        aikaAjastin.Timeout += delegate
        {
            aikalaskuri.Value -= 0.1;
            if (aikalaskuri.Value <= 0)
            {
                aikaAjastin.Stop();
                PeliPaattyi();
            }
        };
        aikaAjastin.Start();

        Label aikanaytto = new Label();
        aikanaytto.X = Screen.Right - 150;
        aikanaytto.Y = Screen.Top - 100;
        aikanaytto.Color = Color.LightPink;
        aikanaytto.DecimalPlaces = 1;
        aikanaytto.Title = ("Aika: ");
        aikanaytto.BindTo(aikalaskuri);

        Add(aikanaytto);
    }

    private void PeliPaattyi()
    {
        kolikkoAjastin.Stop();

        foreach (PhysicsObject kolikko in aktiivisetKolikot)
        {
            kolikko.Destroy();
        }

        aktiivisetKolikot.Clear();

        int lopputulos = LaskeLopputulos(pistelaskuri.Value);

        MessageWindow viesti =
            new MessageWindow($"Aika loppui!/nKerätyt kukat: {pistelaskuri.Value}/nLopulliset pisteet: {lopputulos}");
        viesti.Closed += delegate { ConfirmExit(); };
        Add(viesti);
    }

    private int LaskeLopputulos(int keratytKukat)
    {
        int kerroin = 10;
        return keratytKukat * kerroin;
    }
}