using System;
using System.Collections.Generic;
using Jypeli;

using Timer = Jypeli.Timer;

/// @author minttu lyytinen
/// @version 23.11.2026
/// <summary>
/// Kukkienkeräilijä -peli Ohjelmointi 1 -kurssi
/// </summary>
public class Kukkienkerailija : PhysicsGame
{
    private const double PELIN_KESTO = 30.0; //Pelin kesto sekunteina
    private const double HAHMON_NOPEUS = 1000.0;

    private static readonly Image PRINSESSA_KUVA = LoadImage("prinsessa.png");
    private static readonly Image KUKKA_KUVA = LoadImage("kukka.png");

    /// <summary>
    /// Ajastin joka luo uusia kukkia.
    /// </summary>
    private Timer kukkaAjastin;
    
    /// <summary>
    /// Laskuri kerätyille kukille.
    /// </summary>
    private IntMeter pistelaskuri;
    
    /// <summary>
    /// Laskuri peliajalle.
    /// </summary>
    private DoubleMeter aikalaskuri;
    
    /// <summary>
    /// Pelaajan hahmo.
    /// </summary>
    private PhysicsObject prinsessa;
    
    /// <summary>
    /// Pelikentän maa olio.
    /// </summary>
    private PhysicsObject maa;
    
    /// <summary>
    /// Lista kentällä olevista kukista.
    /// </summary>
    private List<PhysicsObject> aktiivisetKukat = new List<PhysicsObject>();
    
    /// <summary>
    /// Alustaa pelin ja käynnistää pelitoiminnot.
    /// </summary>
    public override void Begin()
    {
        Gravity = new Vector(0, -400);

        LuoKentta();
        LuoPelaaja();
        AsetaOhjaimet();
        LuoKukkaAjastin();
        LuoPistelaskuri();
        LuoAikalaskuri();

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

        maa = PhysicsObject.CreateStaticObject(Level.Width, 150);
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
        prinsessa = new PhysicsObject(150.0, 200.0);
        prinsessa.Shape = Shape.Rectangle;
        prinsessa.Image = PRINSESSA_KUVA;

        prinsessa.X = 0.0;
        prinsessa.Y = -300.0;
        prinsessa.Restitution = 0.0;

        prinsessa.MomentOfInertia = Double.PositiveInfinity;

        Add(prinsessa);
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
    /// <param name="nopeus">Hahmon liikenopeus vaakasuunnassa.</param>
    private void LiikutaHahmoa(double nopeus)
    {
        prinsessa.Velocity = new Vector(nopeus, prinsessa.Velocity.Y);
    }

    
    /// <summary>
    /// Luodaan ajastin, joka luo uuden kukan säännöllisin välein
    /// </summary>
    private void LuoKukkaAjastin()
    {
        kukkaAjastin = new Timer();
        kukkaAjastin.Interval = 1.5; //uusi kukka 1,5 sekunnin välein
        kukkaAjastin.Timeout += LuoKukka;
        kukkaAjastin.Start();
    }
    
    
    /// <summary>
    /// Luodaan kukka kentän ylälaitaan satunnaiseen kohtaan. 
    /// </summary>
    private void LuoKukka()
    {
        PhysicsObject kukka = new PhysicsObject(50.0, 50.0);
        kukka.Shape = Shape.Circle;
        kukka.Color = Color.Yellow;
        kukka.Image = KUKKA_KUVA;

        double satunnainenX = RandomGen.NextDouble(Level.Left + 50, Level.Right - 50.0);
        kukka.X = satunnainenX;
        kukka.Y = Level.Top - 40;

        aktiivisetKukat.Add(kukka);

        AddCollisionHandler(prinsessa, kukka, OsumaKukkaan);
        AddCollisionHandler(maa, kukka, KukkaOsuiMaahan);

        Add(kukka);
    }

    
    /// <summary>
    /// Poistetaan kerätty kukka listasta ja pelialueelta
    /// </summary>
    /// <param name="pelaaja">Pelaajaolio, joka osuu kukkaan.</param>
    /// <param name="kukka">Kerättävä kukka-olio.</param>
    private void OsumaKukkaan(PhysicsObject pelaaja, PhysicsObject kukka)
    {
        pistelaskuri.Value += 1;
        aktiivisetKukat.Remove(kukka);
        kukka.Destroy();
    }
    
    
    /// <summary>
    /// Poistetaan maahan osunut kukka listasta ja pelialueelta.
    /// </summary>
    /// <param name="maaOlio">Maa-olio, johon kukka osuu. </param>
    ///  <param name="kukka">Maahan osuva kukka-olio.</param>
    private void KukkaOsuiMaahan(PhysicsObject maaOlio, PhysicsObject kukka)
    {
        aktiivisetKukat.Remove(kukka);
        kukka.Destroy();
    }

    
    /// <summary>
    /// Luodaan pistelaskuri näkyviin
    /// </summary>
    private void LuoPistelaskuri()
    {
        pistelaskuri = new IntMeter(0);

        Label pistenaytto = new Label();
        pistenaytto.X = Screen.Left + 150;
        pistenaytto.Y = Screen.Top - 100;
        pistenaytto.Color = Color.LightPink;
        pistenaytto.Title = "Pisteet: ";
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
        aikanaytto.Title = "Aika: ";
        aikanaytto.BindTo(aikalaskuri);

        Add(aikanaytto);
    }
    
    
    /// <summary>
    /// Kutsutaan kun aika loppuu. Pysäyttää ajastimen, tyhjentää kentän ja näyttää lopputuloksen.
    /// </summary>
    private void PeliPaattyi()
    {
        kukkaAjastin.Stop();

        foreach (PhysicsObject kukka in aktiivisetKukat)
        {
            kukka.Destroy();
        }

        aktiivisetKukat.Clear();

        int lopputulos = LaskeLopputulos(pistelaskuri.Value);

        MessageWindow viesti =
            new MessageWindow($"Aika loppui!\nKerätyt kukat: {pistelaskuri.Value}\nLopulliset pisteet: {lopputulos}");
        viesti.Closed += delegate { ConfirmExit(); };
        Add(viesti);
    }


    /// <summary>
    /// Laskee lopulliset pisteet kerättyjen kukkien perusteella.
    /// </summary>
    /// <param name="keratytKukat">Kerättyjen kukkien kokonaismäärä.</param>
    /// <returns>Lopullinen pistemäärä.</returns>
    private int LaskeLopputulos(int keratytKukat)
    {
        return keratytKukat;
    }
}