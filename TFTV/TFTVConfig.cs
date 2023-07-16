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
        [ConfigField(text: "CONFIG TFTV PAR DÉFAUT",
            description: "Définit tous les paramètres par défaut, pour offrir l'expérience de 'Terror from the Void' telle qu'elle a été imaginée par ses créateurs.")]
        //Sets all settings to default, to provide the Terror from the Void experience as envisioned by its creators
        public bool defaultSettings = false;

        [ConfigField(text: "ANIMATIONS CONTINUES DURANT LE TIR",
            description: "Les personnages continueront à s'animer pendant les séquences de tir et les cibles touchées pourront tressaillir, " +
            "les prises de vue suivantes d'une rafale sont ratées lors de la prise de vue en mode de prise de vue libre.")]
        //The characters will continue to animate during shooting sequences and targets that are hit may flinch, causing subsequent shots in a burst to miss when shooting in freeaim mode."
        public bool AnimateWhileShooting = false;



        //BetterEnemies
        [ConfigField(text: "RENDRE LES PANDORIENS PLUS FORTS",
       description: "Applique les changements de 'Dtony BetterEnemies' qui donne plus de challenge en modifiant le comportement et certaines caractéristiques des Pandoriens.")]
        public bool BetterEnemiesOn = false;

       [ConfigField(text: "FORCER LES PARAMÈTRES DE DIFFICULTÉ DES DÉBUTANTS",
          description: "Certains paramètres de configuration sont définis par défaut à un certain niveau pour les débutants (voir chaque option de configuration pour plus de détails). Si vous souhaitez les forcer, cochez cette case.")]
        public bool OverrideRookieDifficultySettings = false;

        [ConfigField(text: "MISSIONS TACTIQUES FACILES",
          description: "Tous les ennemis gagnent un trait spécial augmentant les dégâts qui leur sont infligés de 50%, les Pandorians n'ont jamais plus de 20 d'armure, Les Scylla et Noeuds ont moins de PV." +
            "Tous les agents de Phoenix bénéficient d'une caractéristique spéciale qui augmente leur résistance aux dégâts de 50 %. Réglé par défaut à 'Oui' en mode débutant")]
        public bool EasyTactical = false;

        [ConfigField(text: "GEOSCAPE FACILE",
          description: "Toutes les récompenses diplomatiques, les récompenses en ressources des missions, les résultats de la recherche sont doublés et toutes les pénalités diplo. sont réduites de moitié. " +
            " Réglé par défaut à 'Oui' en mode débutant")]
        public bool EasyGeoscape = false;

        [ConfigField(text: "JE SUIS ETERMES",
                  description: "...Et tout est toujours trop facile pour vous.Tous les agents de Phoenix subissent 25 % de dégâts supplémentaires." +
                  "...Et tout est toujours trop facile pour vous. Tous les ennemis subissent 25 % de dégâts en moins.")]
        public bool EtermesMode = false;

        [ConfigField(text: "JOUER AVEC LE PRÉSAGE DU NÉANT : PLUS DE BRUME",
            description: "Si vous jouez sur un PC d'entrée de gamme et que vous ressentez du lag avec ce 'Présage du Néant', vous pouvez le désactiver ici. Cela l'empêchera d'apparaître" +
            " et s'il est déjà lancé, il n'aura aucun effet.")]
        // If you are playing on a Low-end system and experience lag with this Void Omen, you can turn it off here. This will prevent it from rolling and if already rolled, will prevent it from having any effect
        public bool MoreMistVO = true;

        [ConfigField(text: "PASSER LES CINÉMATIQUES",
             description: "Choisissez d'ignorer les logos sur les cinématiques de lancement du jeu, d'introduction et d'atterrissage. Adapté de Mad's Assorted Adjustments..")]
        public bool SkipMovies = false;

        //New LOTA settings
        [ConfigField(text: "QUANTITÉ DE RESSOURCES EXOTIQUES",
         description: "Choisissez la quantité de ressources exotiques que vous voulez avoir dans votre jeu par partie. Chaque unité fournit suffisamment de ressources pour fabriquer un ensemble d'armes impossibles. " +
          "Ainsi, si vous voulez avoir deux séries complètes, réglez ce nombre sur 2, et ainsi de suite. Par défaut, ce nombre est défini par le niveau de difficulté: 2.5 en Débutant, 2 en Vétéran, 1.5 en Héros, 1 en Légendaire. " +
          "Nécessité de redémarrer le jeu pour que les changements prennent effet.")]
        public float amountOfExoticResources = 1f;

        [ConfigField(text: "AJUSTEMENTS DES ARMES IMPOSSIBLES", description: "In TFTV, Les armes antiques sont remplacées par les armes impossibles (IW) en " +
            "contrepartie. Elles ont des fonctionnalités différentes (lire : elles sont affaiblis). " +
            "et certains d'entre elles nécessitent des recherches supplémentaires sur les factions.  " +
            "Cochez cette option pour que les armes impossibles conservent les mêmes stats et fonctionnalités que les armes antiques du JEU 'Vanilla' et sans nécessiter de recherche de faction supplémentaire.")]
        public bool impossibleWeaponsAdjustments = true;

        //Starting squad

        public enum StartingSquadFaction
        {
            PHOENIX, ANU, NJ, SYNEDRION
        }
        [ConfigField(text: "Escouade de démarrage",
         description: "Vous pouvez choisir une équipe de départ différente. Si vous le faites, l'un de vos Assauts et votre Soldat Lourd de départ en mode Légende ou Héros, " +
            "Assaut en Vétéran, ou Sniper en débutant sera remplacé par un agent de la classe d'élite de la faction de votre choix. " +
            "Vous obtiendrez également la technologie de faction correspondante lorsque la faction l'aura recherchée.")]
        public StartingSquadFaction startingSquad = StartingSquadFaction.PHOENIX;

 
        public enum StartingBaseLocation
        {
            [Description("Comme 'Vanilla'")]
            Vanilla,

            [Description("Aléatoire")]
            Random,

            [Description("Afrique du Nord (Algérié)")]
            Algeria,

            [Description("Europe de l'Est (Ukraine)")]
            Ukraine,

            [Description("Amérique centrale (Mexique)")]
            Mexico,

            [Description("Amérique du Sud (Bolivie)")]
            Bolivia,

            [Description("Afrique de l'Est (Ethiopie)")]
            Ethiopia,

            [Description("Asie (Chine)")]
            China,

            [Description("Asia du Nord (Sibérie)")]
            Siberia,

            [Description("Moyen-Orient (Afghanistan)")]
            Afghanistan,

            [Description("Antarctique")]
            Antarctica,

            [Description("Amérique du Sud (Terre de Feu)")]
            Argentina,

            [Description("Australie")]
            Australia,

            [Description("Asie du Sud-Est (Cambodge)")]
            Cambodia,

            [Description("Afrique du Sud (Zimbabwe)")]
            Zimbabwe,

            [Description("Afrique de l'Ouest (Ghana)")]
            Ghana,

            [Description("Amérique Centrale (Honduras)")]
            Honduras,

            [Description("Amérique du Nord (Quebec)")]
            Quebec,

            [Description("Amérique du Nord (Alaska)")]
            Alaska,

            [Description("Groenland")]
            Greenland
        }


         [ConfigField(text: "Base de départ",
          description: "Vous pouvez choisir un lieu spécifique pour commencer. Veuillez noter que certains endroits sont plus difficiles à démarrer que d'autres.!")]
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
            (4.0f, 4.9f, 1.0f) Amérique du Nord (Quebec) 185 
            (1.3f, 5.7f, -2.6f) Amérique du Nord (Alaska) 192
            (0.7f, 6.2f, 1.5f) Greenland 187 
            */



        public enum StartingSquadCharacters
        {
            SANS_BONUS, AVEC_BONUS, ALEATOIRE
        }
        [ConfigField(text: "Personnages du tutoriels dans votre équipe de départ",
   description: "Vous pouvez choisir d'obtenir une équipe complètement aléatoire (comme dans Vanilla, sans faire le tutoriel)., " +
            "l'équipe de départ du tutoriel 'Vanilla' (avec des statistiques plus élevées), " +
            "ou une équipe qui comprendra Sophia Brown et Jacob avec des stats non améliorées. (par défaut sur TFTV). " +
            "Notez que Jacob est un sniper, comme dans l'écran titre. :)")]

        public StartingSquadCharacters tutorialCharacters = StartingSquadCharacters.SANS_BONUS;
       
        // These settings determine amount of resources player can acquire:
        [ConfigField(text: "Nombre de sites de récupération",
            description: "Nombre total de sites de pillage générés au début du jeu, sans compter les sites envahis par la végétation.\n" +
            "(Vanilla: 16, TFTV par défaut 8, parce que les embuscades génèrent des ressources supplémentaires)\n" +
            "N'aura aucun effet sur une partie en cours..")]
        public int InitialScavSites = 8; // 16 on Vanilla

        public enum ScavengingWeight
        {
            Aucun, Faible, Moyen, Haut
        }

        [ConfigField(text: "Proba. des sites avec des caisses de ressources",
           description: "Sur le nombre total de sites de récupération, choisissez les chances relatives de générer un site de pillage de ressources.\n" +
            "Choisissez aucun pour avoir 0 site de pillage de ce type (Vanilla et TFTV par défaut : élevé).\n" +
            "N'aura aucun effet dans un jeu en cours.")]
        public ScavengingWeight ChancesScavCrates = ScavengingWeight.Haut;
        [ConfigField(text: "Proba. des sites avec soldats",
           description: "Sur le nombre total de sites de récupération, choisissez les chances relatives de générer un site de sauvetage de recrues.\n" +
            "Choisissez aucun pour avoir 0 site de pillage de ce type (Vanilla et TFTV par défaut : faible)\n" +
            "N'aura aucun effet dans un jeu en cours.")]
        public ScavengingWeight ChancesScavSoldiers = ScavengingWeight.Faible;
        [ConfigField(text: "Proba. des sites avec véhicules",
           description: "Sur le nombre total de sites de sauvetage, choisissez les chances relatives de générer un site de sauvetage de véhicule.\n" +
            "Choisissez aucun pour avoir 0 site de pillage de ce type (Vanilla et TFTV par défaut : faible)\n" +
            "N'aura aucun effet dans un jeu en cours.")]
        public ScavengingWeight ChancesScavGroundVehicleRescue = ScavengingWeight.Faible;

        // Determines amount of resources gained in Events. 1f = 100% if Azazoth level.
        // 0.8f = 80% of Azazoth = Pre-Azazoth level (default on BetterGeoscape).
        // For example, to double amount of resources from current Vanilla (Azazoth level), change to 2f 
        // Can be applied to game in progress
        [ConfigField(text: "Quantité de ressources gagnées des évén.",
           description: "Pour la version actuelle (post patch Azazoth) de Vanilla, réglez à 1. Pour le TFTV par défaut et vanilla avant le patch Azazoth, mettez 0.8.\n" +
            "Peut être appliqué sur une partie en cours.\n"+"Valeur par défaut en mode débutant : 1.2")] //done
        public float ResourceMultiplier = 0.8f;

        // Changing the settings below will make the game easier:

        // Determines if diplomatic penalties are applied when cozying up to one of the factions by the two other factions
        // Can be applied to game in progress
        [ConfigField(text: "Pénalités diplomatiques élevées",
           description: "Les pénalités diplomatiques découlant des choix effectués lors des événements sont doublées et le fait de révéler des missions diplomatiques pour une faction entraîne une pénalité diplomatique avec les autres factions.\n" +
                        "Peut être appliqué sur une partie en cours\n" + "Valeur par défaut en mode débutant : non.")] //done
        public bool DiplomaticPenalties = true;


        // If set to false, a disabled limb in tactical will not set character's Stamina to zero in geo
        [ConfigField(text: "Endurance drainée par les blessures",
           description: "L'endurance de tout agent qui subit une blessure au combat entraînant une invalidité d'une partie du corps sera remise à zéro après la mission.\n" +
            "Peut être appliqué sur une partie en cours.\n" + "Valeur par défaut en mode débutant : non.")] //done
        public bool StaminaPenaltyFromInjury = true;

        // If set to false, applying a mutation will not set character's Stamina to zero
        [ConfigField(text: "Endurance drainée par les mutations",
          description: "L'endurance de tout agent qui subit une mutation sera mise à zéro..\n" +
           "Peut être appliqué sur une partie en cours.\n" + "Valeur par défaut en mode débutant : non.")] //done
        public bool StaminaPenaltyFromMutation = true;

        // If set to false, adding a bionic will not set character's Stamina to zero
        [ConfigField(text: "Endurance drainée par les augment. bioniques",
          description: "L'endurance de tout opérateur qui subit une augmentation bionique sera mise à zéro..\n" +
           "Peut être appliqué sur une partie en cours.\n" + "Valeur par défaut en mode débutant : non.")] //done")]
        public bool StaminaPenaltyFromBionics = true;

        // If set to false, ambushes will happen as rarely as in Vanilla, and will not have crates in them
        [ConfigField(text: "Nouvelles embuscades",
          description: "Les embuscades se produiront plus souvent et seront plus difficiles. Indépendamment de ce paramètre, toutes les embuscades contiendront des caisses.\n" + "Valeur par défaut en mode débutant : non.")] //done")]
        public bool MoreAmbushes = true;

        // Changing the settings below will make the game harder:

        // If set to true, the passenger module FAR-M will no longer regenerate Stamina in flight
        // For players who prefer having to come back to base more often
        [ConfigField(text: "Récupération d'endurance dans le RPA",
         description: "Le module de passagers de type initial, RPA, récupérera lentement l'endurance des agents à bord.\n" +
            "Désactiver si vous préférez devoir retourner à la base plus souvent")]
        public bool ActivateStaminaRecuperatonModule = true;

        // If set to true reversing engineering an item allows to research the faction technology that allows manufacturing the item 
        [ConfigField(text: "Rétro-ingénierie avancée",
        description: "Après avoir réalisé la rétro-ingénierie d'un objet, permet ensuite de rechercher la technologie de la faction qui permet de fabriquer l'objet. SI VOUS MODIFIEZ CE PARAMÈTRE, QUITTEZ LE JEU SUR LE BUREAU ET REDÉMARREZ POUR QUE LES CHANGEMENTS PRENNENT EFFET.")]
        public bool ActivateReverseEngineeringResearch = true;

        // Below are advanced settings. The mod was designed to be played with all of them turned on

        //If set to true, modifes stats of some weapons and modules, and adds random chance that weapon will be lost when disengaging
        [ConfigField(text: "Changements du combat aérien",
       description: "Modifie les stats de certaines armes et modules.")]
        public bool ActivateAirCombatChanges = true;

        // If set to true activates DLC5 Kaos Engines story rework (in progress)
        [ConfigField(text: "Changements du Marché DLC5 (en cours)",
       description: "Supprime les cutscenes et les missions, tous les articles sont disponibles aux prix les plus bas 24 heures après leur découverte sur le marché.")]
        public bool ActivateKERework = true;

        // If set to true, unrevealed havens will be revealed when attacked
        [ConfigField(text: "Révélation des Refuges attaqués",
       description: "Les Refuges attaqués enverront un SOS, révélant leur emplacement au joueur.")]
        public bool HavenSOS = true;

        //If set to 1, shows when any error ocurrs. Do not change unless you know what you are doing.
        [ConfigField(text: "Debug log & messages",
       description: "Montre quand une erreur se produit. S'il vous plaît, ne changez rien sans savoir ce que vous faites..")]
        public bool Debug = true;

        [ConfigField(text: "Apprendre la première compétence personnelle",
           description: "Si elle est activée, la première compétence personnelle (niveau 1) est définie juste après la création d'un personnage (soldats de départ, nouvelles recrues des refuges, récompenses, etc).")]
        public bool LearnFirstPersonalSkill = true;

        // Deactivate auto standy in tactical missions
        [ConfigField(text: "Désactiver la mise en veille automatique tactique",
            description: "Désactive ou active le comportement 'vanilla' qui consiste à mettre automatiquement un acteur en mode veille et donc à passer à l'acteur suivant lorsque tous les AP sont utilisés.\n"
                         + "ATTENTION: This function is WIP, i.e. currently still experimental!")]
        public bool DeactivateTacticalAutoStandby = false;

        // Infiltrator Crossbow Ammo changes
        [ConfigField(text: "Munitions de l'Eros",
            description: "Définissez le nombre de carreaux pour le magasin des arbalètes de base. (Eros CRB III)")]
        public int BaseCrossbow_Ammo = 6; //3
        [ConfigField(text: "Munitions du Psyche",
            description: "Définissez le nombre de carreaux pour le magasin des arbalètes avancées (Psyche CRB IV)")]
        public int VenomCrossbow_Ammo = 4; //3

        // Always show helmets
      /*  [ConfigField(text: "Operatives will appear without helmets in the personnel screen",
            description: "Turn off if you don't like to see the faces of the individuals you are sending to be clawed and devoured by horrifying monstrosities")]
        public bool ShowFaces = true;*/

        [ConfigField(text: "Changements de Gameplay Calvitix",
            description: "Nb max en mission : 16," + "\n" 
            +"+1 slot dans les avions," + "\n" 
            +"Coût des munitions / 2," + "\n" 
            +"Coût des bâtiments x 2," + "\n" 
            +"Production de matériau et tech par les batiments," + "\n" 
            +"Plus de munitions pour les véhicules " + "\n" 
            +"Reload avec Grenade Launcher ammo," + "\n" 
            +"Tyr Autocanon, 1 coup après l'autre,\n...")]
        public bool ApplyCalvitixChanges = false;


    }
}
