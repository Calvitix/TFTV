using PhoenixPoint.Modding;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;

namespace TFTV
{
    /// <summary>
    /// ModConfig is mod settings that players can change from within the game.
    /// Config is only editable from players in main menu.
    /// Only one config can exist per mod assembly.
    /// Config is serialized on disk as json.
    /// </summary>

    public class TFTVConfig : ModConfig
    {
        //Default settings
        [ConfigField(text: "CONFIG TFTV PAR D�FAUT",
            description: "D�finit tous les param�tres par d�faut, pour offrir l'exp�rience de 'Terror from the Void' telle qu'elle a �t� imagin�e par ses cr�ateurs.")]
        //Sets all settings to default, to provide the Terror from the Void experience as envisioned by its creators
        public bool defaultSettings = false;

        [ConfigField(text: "ANIMATIONS CONTINUES DURANT LE TIR",
            description: "Les personnages continueront � s'animer pendant les s�quences de tir et les cibles touch�es pourront tressaillir, " +
            "les prises de vue suivantes d'une rafale sont rat�es lors de la prise de vue en mode de prise de vue libre.")]
        //The characters will continue to animate during shooting sequences and targets that are hit may flinch, causing subsequent shots in a burst to miss when shooting in freeaim mode."
        public bool AnimateWhileShooting = false;



        //BetterEnemies
        [ConfigField(text: "RENDRE LES PANDORIENS PLUS FORTS",
       description: "Applique les changements de 'Dtony BetterEnemies' qui donne plus de challenge en modifiant le comportement et certaines caract�ristiques des Pandoriens.")]
        public bool BetterEnemiesOn = false;

       [ConfigField(text: "FORCER LES PARAM�TRES DE DIFFICULT� DES D�BUTANTS",
          description: "Certains param�tres de configuration sont d�finis par d�faut � un certain niveau pour les d�butants (voir chaque option de configuration pour plus de d�tails). Si vous souhaitez les forcer, cochez cette case.")]
        public bool OverrideRookieDifficultySettings = false;

        [ConfigField(text: "MISSIONS TACTIQUES FACILES",
          description: "Tous les ennemis gagnent un trait sp�cial augmentant les d�g�ts qui leur sont inflig�s de 50%, les Pandorians n'ont jamais plus de 20 d'armure, Les Scylla et Noeuds ont moins de PV." +
            "Tous les agents de Phoenix b�n�ficient d'une caract�ristique sp�ciale qui augmente leur r�sistance aux d�g�ts de 50 %. R�gl� par d�faut � 'Oui' en mode d�butant")]
        public bool EasyTactical = false;

        [ConfigField(text: "GEOSCAPE FACILE",
          description: "Toutes les r�compenses diplomatiques, les r�compenses en ressources des missions, les r�sultats de la recherche sont doubl�s et toutes les p�nalit�s diplo. sont r�duites de moiti�. " +
            " R�gl� par d�faut � 'Oui' en mode d�butant")]
        public bool EasyGeoscape = false;

        [ConfigField(text: "JE SUIS ETERMES",
                  description: "...Et tout est toujours trop facile pour vous.Tous les agents de Phoenix subissent 25 % de d�g�ts suppl�mentaires." +
                  "...Et tout est toujours trop facile pour vous. Tous les ennemis subissent 25 % de d�g�ts en moins.")]
        public bool EtermesMode = false;

        [ConfigField(text: "JOUER AVEC LE PR�SAGE DU N�ANT : PLUS DE BRUME",
            description: "Si vous jouez sur un PC d'entr�e de gamme et que vous ressentez du lag avec ce 'Pr�sage du N�ant', vous pouvez le d�sactiver ici. Cela l'emp�chera d'appara�tre" +
            " et s'il est d�j� lanc�, il n'aura aucun effet.")]
        // If you are playing on a Low-end system and experience lag with this Void Omen, you can turn it off here. This will prevent it from rolling and if already rolled, will prevent it from having any effect
        public bool MoreMistVO = true;

        [ConfigField(text: "PASSER LES CIN�MATIQUES",
             description: "Choisissez d'ignorer les logos sur les cin�matiques de lancement du jeu, d'introduction et d'atterrissage. Adapt� de Mad's Assorted Adjustments..")]
        public bool SkipMovies = false;

        //New LOTA settings
        [ConfigField(text: "QUANTIT� DE RESSOURCES EXOTIQUES",
         description: "Choisissez la quantit� de ressources exotiques que vous voulez avoir dans votre jeu par partie. Chaque unit� fournit suffisamment de ressources pour fabriquer un ensemble d'armes impossibles. " +
          "Ainsi, si vous voulez avoir deux s�ries compl�tes, r�glez ce nombre sur 2, et ainsi de suite. Par d�faut, ce nombre est d�fini par le niveau de difficult�: 2.5 en D�butant, 2 en V�t�ran, 1.5 en H�ros, 1 en L�gendaire. " +
          "N�cessit� de red�marrer le jeu pour que les changements prennent effet.")]
        public float amountOfExoticResources = 1f;

        [ConfigField(text: "AJUSTEMENTS DES ARMES IMPOSSIBLES", description: "In TFTV, Les armes antiques sont remplac�es par les armes impossibles (IW) en " +
            "contrepartie. Elles ont des fonctionnalit�s diff�rentes (lire : elles sont affaiblis). " +
            "et certains d'entre elles n�cessitent des recherches suppl�mentaires sur les factions.  " +
            "Cochez cette option pour que les armes impossibles conservent les m�mes stats et fonctionnalit�s que les armes antiques du JEU 'Vanilla' et sans n�cessiter de recherche de faction suppl�mentaire.")]
        public bool impossibleWeaponsAdjustments = true;

        //Starting squad

        public enum StartingSquadFaction
        {
            PHOENIX, ANU, NJ, SYNEDRION
        }
        [ConfigField(text: "Escouade de d�marrage",
         description: "Vous pouvez choisir une �quipe de d�part diff�rente. Si vous le faites, l'un de vos Assauts et votre Soldat Lourd de d�part en mode L�gende ou H�ros, " +
            "Assaut en V�t�ran, ou Sniper en d�butant sera remplac� par un agent de la classe d'�lite de la faction de votre choix. " +
            "Vous obtiendrez �galement la technologie de faction correspondante lorsque la faction l'aura recherch�e.")]
        public StartingSquadFaction startingSquad = StartingSquadFaction.PHOENIX;

 
        public enum StartingBaseLocation
        {
            [Description("Comme 'Vanilla'")]
            Vanilla,

            [Description("Al�atoire")]
            Random,

            [Description("Afrique du Nord (Alg�ri�)")]
            Algeria,

            [Description("Europe de l'Est (Ukraine)")]
            Ukraine,

            [Description("Am�rique centrale (Mexique)")]
            Mexico,

            [Description("Am�rique du Sud (Bolivie)")]
            Bolivia,

            [Description("Afrique de l'Est (Ethiopie)")]
            Ethiopia,

            [Description("Asie (Chine)")]
            China,

            [Description("Asia du Nord (Sib�rie)")]
            Siberia,

            [Description("Moyen-Orient (Afghanistan)")]
            Afghanistan,

            [Description("Antarctique")]
            Antarctica,

            [Description("Am�rique du Sud (Terre de Feu)")]
            Argentina,

            [Description("Australie")]
            Australia,

            [Description("Asie du Sud-Est (Cambodge)")]
            Cambodia,

            [Description("Afrique du Sud (Zimbabwe)")]
            Zimbabwe,

            [Description("Afrique de l'Ouest (Ghana)")]
            Ghana,

            [Description("Am�rique Centrale (Honduras)")]
            Honduras,

            [Description("Am�rique du Nord (Quebec)")]
            Quebec,

            [Description("Am�rique du Nord (Alaska)")]
            Alaska,

            [Description("Groenland")]
            Greenland
        }


         [ConfigField(text: "Base de d�part",
          description: "Vous pouvez choisir un lieu sp�cifique pour commencer. Veuillez noter que certains endroits sont plus difficiles � d�marrer que d'autres.!")]
         public StartingBaseLocation startingBaseLocation = StartingBaseLocation.Vanilla;
        

        /*
            (-0.4f, 3.7f, 5.2f),  North Africa (Algeria) 165 
            (-1.9f, 4.8f, 3.8f), Eastern Europe (Ukraine) 166
            (5.3f, 3.1f, -1.7f), Central America (Mexico) 167
            (5.5f, -2.0f, 2.7f), South America (Bolivia) 168
            (-4.2f, 1.1f, 4.7f), East Africa (Ethiopia) 169
            (-5.1f, 3.8f, -0.8f), Asia (China)  170
            (-1.9f, 6.0f, -1.2f), Northern Asia (Siberia) 171
            (-4.8f, 3.7f, 2.2f) Middle East (Afghanistan) 172
            (0.0f, -6.4f, 0.1f) Antarctica 584
            (3.5f, -5.2f, 1.3f)  South America (Tierra de Fuego) 193
            (-4.5f, -1.5f, -4.3f) Australia 191
            (-6.0f, 1.3f, -1.9f) Southeast Asia (Cambodia) 190
            (-2.7f, -2.4f, 5.3f) South Africa (Zimbabwe) 189
            (0.5f, 0.8f, 6.3f) West Africa (Ghana) 188
            (6.2f, 1.6f, 0.4f) Central America (Honduras) 186
            (4.0f, 4.9f, 1.0f) Am�rique du Nord (Quebec) 185 
            (1.3f, 5.7f, -2.6f) Am�rique du Nord (Alaska) 192
            (0.7f, 6.2f, 1.5f) Greenland 187 
            */



        public enum StartingSquadCharacters
        {
            SANS_BONUS, AVEC_BONUS, ALEATOIRE
        }
        [ConfigField(text: "Personnages du tutoriels dans votre �quipe de d�part",
   description: "Vous pouvez choisir d'obtenir une �quipe compl�tement al�atoire (comme dans Vanilla, sans faire le tutoriel)., " +
            "l'�quipe de d�part du tutoriel 'Vanilla' (avec des statistiques plus �lev�es), " +
            "ou une �quipe qui comprendra Sophia Brown et Jacob avec des stats non am�lior�es. (par d�faut sur TFTV). " +
            "Notez que Jacob est un sniper, comme dans l'�cran titre. :)")]

        public StartingSquadCharacters tutorialCharacters = StartingSquadCharacters.SANS_BONUS;
       
        // These settings determine amount of resources player can acquire:
        [ConfigField(text: "Nombre de sites de r�cup�ration",
            description: "Nombre total de sites de pillage g�n�r�s au d�but du jeu, sans compter les sites envahis par la v�g�tation.\n" +
            "(Vanilla: 16, TFTV par d�faut 8, parce que les embuscades g�n�rent des ressources suppl�mentaires)\n" +
            "N'aura aucun effet sur une partie en cours..")]
        public int InitialScavSites = 8; // 16 on Vanilla

        public enum ScavengingWeight
        {
            Aucun, Faible, Moyen, Haut
        }

        [ConfigField(text: "Proba. des sites avec des caisses de ressources",
           description: "Sur le nombre total de sites de r�cup�ration, choisissez les chances relatives de g�n�rer un site de pillage de ressources.\n" +
            "Choisissez aucun pour avoir 0 site de pillage de ce type (Vanilla et TFTV par d�faut : �lev�).\n" +
            "N'aura aucun effet dans un jeu en cours.")]
        public ScavengingWeight ChancesScavCrates = ScavengingWeight.Haut;
        [ConfigField(text: "Proba. des sites avec soldats",
           description: "Sur le nombre total de sites de r�cup�ration, choisissez les chances relatives de g�n�rer un site de sauvetage de recrues.\n" +
            "Choisissez aucun pour avoir 0 site de pillage de ce type (Vanilla et TFTV par d�faut : faible)\n" +
            "N'aura aucun effet dans un jeu en cours.")]
        public ScavengingWeight ChancesScavSoldiers = ScavengingWeight.Faible;
        [ConfigField(text: "Proba. des sites avec v�hicules",
           description: "Sur le nombre total de sites de sauvetage, choisissez les chances relatives de g�n�rer un site de sauvetage de v�hicule.\n" +
            "Choisissez aucun pour avoir 0 site de pillage de ce type (Vanilla et TFTV par d�faut : faible)\n" +
            "N'aura aucun effet dans un jeu en cours.")]
        public ScavengingWeight ChancesScavGroundVehicleRescue = ScavengingWeight.Faible;

        // Determines amount of resources gained in Events. 1f = 100% if Azazoth level.
        // 0.8f = 80% of Azazoth = Pre-Azazoth level (default on BetterGeoscape).
        // For example, to double amount of resources from current Vanilla (Azazoth level), change to 2f 
        // Can be applied to game in progress
        [ConfigField(text: "Quantit� de ressources gagn�es des �v�n.",
           description: "Pour la version actuelle (post patch Azazoth) de Vanilla, r�glez � 1. Pour le TFTV par d�faut et vanilla avant le patch Azazoth, mettez 0.8.\n" +
            "Peut �tre appliqu� sur une partie en cours.\n"+"Valeur par d�faut en mode d�butant : 1.2")] //done
        public float ResourceMultiplier = 0.8f;

        // Changing the settings below will make the game easier:

        // Determines if diplomatic penalties are applied when cozying up to one of the factions by the two other factions
        // Can be applied to game in progress
        [ConfigField(text: "P�nalit�s diplomatiques �lev�es",
           description: "Les p�nalit�s diplomatiques d�coulant des choix effectu�s lors des �v�nements sont doubl�es et le fait de r�v�ler des missions diplomatiques pour une faction entra�ne une p�nalit� diplomatique avec les autres factions.\n" +
                        "Peut �tre appliqu� sur une partie en cours\n" + "Valeur par d�faut en mode d�butant : non.")] //done
        public bool DiplomaticPenalties = true;


        // If set to false, a disabled limb in tactical will not set character's Stamina to zero in geo
        [ConfigField(text: "Endurance drain�e par les blessures",
           description: "L'endurance de tout agent qui subit une blessure au combat entra�nant une invalidit� d'une partie du corps sera remise � z�ro apr�s la mission.\n" +
            "Peut �tre appliqu� sur une partie en cours.\n" + "Valeur par d�faut en mode d�butant : non.")] //done
        public bool StaminaPenaltyFromInjury = true;

        // If set to false, applying a mutation will not set character's Stamina to zero
        [ConfigField(text: "Endurance drain�e par les mutations",
          description: "L'endurance de tout agent qui subit une mutation sera mise � z�ro..\n" +
           "Peut �tre appliqu� sur une partie en cours.\n" + "Valeur par d�faut en mode d�butant : non.")] //done
        public bool StaminaPenaltyFromMutation = true;

        // If set to false, adding a bionic will not set character's Stamina to zero
        [ConfigField(text: "Endurance drain�e par les augment. bioniques",
          description: "L'endurance de tout op�rateur qui subit une augmentation bionique sera mise � z�ro..\n" +
           "Peut �tre appliqu� sur une partie en cours.\n" + "Valeur par d�faut en mode d�butant : non.")] //done")]
        public bool StaminaPenaltyFromBionics = true;

        // If set to false, ambushes will happen as rarely as in Vanilla, and will not have crates in them
        [ConfigField(text: "Nouvelles embuscades",
          description: "Les embuscades se produiront plus souvent et seront plus difficiles. Ind�pendamment de ce param�tre, toutes les embuscades contiendront des caisses.\n" + "Valeur par d�faut en mode d�butant : non.")] //done")]
        public bool MoreAmbushes = true;

        // Changing the settings below will make the game harder:

        // If set to true, the passenger module FAR-M will no longer regenerate Stamina in flight
        // For players who prefer having to come back to base more often
        [ConfigField(text: "R�cup�ration d'endurance dans le RPA",
         description: "Le module de passagers de type initial, RPA, r�cup�rera lentement l'endurance des agents � bord.\n" +
            "D�sactiver si vous pr�f�rez devoir retourner � la base plus souvent")]
        public bool ActivateStaminaRecuperatonModule = true;

        // If set to true reversing engineering an item allows to research the faction technology that allows manufacturing the item 
        [ConfigField(text: "R�tro-ing�nierie avanc�e",
        description: "Apr�s avoir r�alis� la r�tro-ing�nierie d'un objet, permet ensuite de rechercher la technologie de la faction qui permet de fabriquer l'objet. SI VOUS MODIFIEZ CE PARAM�TRE, QUITTEZ LE JEU SUR LE BUREAU ET RED�MARREZ POUR QUE LES CHANGEMENTS PRENNENT EFFET.")]
        public bool ActivateReverseEngineeringResearch = true;

        // Below are advanced settings. The mod was designed to be played with all of them turned on

        //If set to true, modifes stats of some weapons and modules, and adds random chance that weapon will be lost when disengaging
        [ConfigField(text: "Changements du combat a�rien",
       description: "Modifie les stats de certaines armes et modules.")]
        public bool ActivateAirCombatChanges = true;

        // If set to true activates DLC5 Kaos Engines story rework (in progress)
        [ConfigField(text: "Changements du March� DLC5 (en cours)",
       description: "Supprime les cutscenes et les missions, tous les articles sont disponibles aux prix les plus bas 24 heures apr�s leur d�couverte sur le march�.")]
        public bool ActivateKERework = true;

        // If set to true, unrevealed havens will be revealed when attacked
        [ConfigField(text: "R�v�lation des Refuges attaqu�s",
       description: "Les Refuges attaqu�s enverront un SOS, r�v�lant leur emplacement au joueur.")]
        public bool HavenSOS = true;

        //If set to 1, shows when any error ocurrs. Do not change unless you know what you are doing.
        [ConfigField(text: "Debug log & messages",
       description: "Montre quand une erreur se produit. S'il vous pla�t, ne changez rien sans savoir ce que vous faites..")]
        public bool Debug = true;

        [ConfigField(text: "Apprendre la premi�re comp�tence personnelle",
           description: "Si elle est activ�e, la premi�re comp�tence personnelle (niveau 1) est d�finie juste apr�s la cr�ation d'un personnage (soldats de d�part, nouvelles recrues des refuges, r�compenses, etc).")]
        public bool LearnFirstPersonalSkill = true;

        // Deactivate auto standy in tactical missions
        [ConfigField(text: "D�sactiver la mise en veille automatique tactique",
            description: "D�sactive ou active le comportement 'vanilla' qui consiste � mettre automatiquement un acteur en mode veille et donc � passer � l'acteur suivant lorsque tous les AP sont utilis�s.\n"
                         + "ATTENTION: This function is WIP, i.e. currently still experimental!")]
        public bool DeactivateTacticalAutoStandby = false;

        // Infiltrator Crossbow Ammo changes
        [ConfigField(text: "Munitions de l'Eros",
            description: "D�finissez le nombre de carreaux pour le magasin des arbal�tes de base. (Eros CRB III)")]
        public int BaseCrossbow_Ammo = 6; //3
        [ConfigField(text: "Munitions du Psyche",
            description: "D�finissez le nombre de carreaux pour le magasin des arbal�tes avanc�es (Psyche CRB IV)")]
        public int VenomCrossbow_Ammo = 4; //3

        // Always show helmets
      /*  [ConfigField(text: "Operatives will appear without helmets in the personnel screen",
            description: "Turn off if you don't like to see the faces of the individuals you are sending to be clawed and devoured by horrifying monstrosities")]
        public bool ShowFaces = true;*/

        [ConfigField(text: "Changements de Gameplay Calvitix",
            description: "Nb max en mission : 16," + "\n" 
            +"+1 slot dans les avions," + "\n" 
            +"Co�t des munitions / 2," + "\n" 
            +"Co�t des b�timents x 2," + "\n" 
            +"Production de mat�riau et tech par les batiments," + "\n" 
            +"Plus de munitions pour les v�hicules " + "\n" 
            +"Reload avec Grenade Launcher ammo," + "\n" 
            +"Tyr Autocanon, 1 coup apr�s l'autre,\n...")]
        public bool ApplyCalvitixChanges = false;


    }
}
