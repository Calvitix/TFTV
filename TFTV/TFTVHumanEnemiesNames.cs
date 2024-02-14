using Base.UI;
using System;
using System.Collections.Generic;

namespace TFTV
{
    internal class TFTVHumanEnemiesNames
    {
        //Calvitix Trad
		public static string[] adjectives = new string[] {"Fous", "Déjantés", "Sournois", "Sanglants", "Déshonorés", "Glorieux", "Sombres", "des décharges",
          "Rouge", "Verts", "Bleux", "Or", "Morts", "Bullet", "Laser", "Déchiquetés", "Acides", "Toxiques", "Rampants", "Suppurants", "Corrompus", "du Kaos",
          "Explosifs", "Brisés", "Obscènes", "Féroces", "de Feu", "de Glace", "Moyens", "Mech", "de Guerre", "Suicidaires", "Instictifs", "Tranchants", "Blasés", "Sanguinaires",
          "Broyés", "Maudits", "Bénis", "Alpha", "Bravo", "Charlie", "Tango", "Zoulous", "Noirs", "Blancs", "Zombies", "affamés", "assoiffés", "de la Mort"};
        public static string[] nouns = new string[] { "Vipères", "Singes", "Boucs", "Arthrons", "Pêcheurs", "Corrupteurs", "Chirons",
            "Sirènes", "Scyllas", "Achérons", "Locustes", "Busards", "Vautours", "Aigles", "Bâtards", "Bâtards", "Tueurs", "Echos",
            "Requins", "", "Taureaux", "Béhémoths", "Dillos", "Mantes", "Guerriers", "Soldats", "Chiens", "Panthères", "Tortues", "Gars",
            "Bugs", "Jokers", "Razors", "Rascals", "Raiders", "Alligators", "Gators", "Raptors", "Prêtres", "Barbares", "Seals", "Crabes",
            "Combattants", "Revenants", "Beriths", "Charuns", "Abbadons", "Seigneurs", "Faucons", "Dealers" };


        public static Dictionary<string, List<string>> names = new Dictionary<string, List<string>>();
        public static Dictionary<string, List<string>> ranks = new Dictionary<string, List<string>>();

        public static void CreateNamesDictionary()
        {
            try
            {
                names.Add("ban", new List<string>(ban_Names));
                names.Add("nj", new List<string>(nj_Names));
                names.Add("anu", new List<string>(anu_Names));
                names.Add("syn", new List<string>(syn_Names));
                names.Add("Purists", new List<string>(pu_Names));
                names.Add("FallenOnes", new List<string>(fo_Names));

            }
            catch (Exception e)
            {
                TFTVLogger.Error(e);
            }

        }


        private static List<string> ConvertKeysToStrings(List<string> locKeys)
        {
            try
            {
                List<string> strings = new List<string>();

                foreach (string key in locKeys)
                {
                    strings.Add(new LocalizedTextBind() { LocalizationKey = key }.Localize());
                }

                return strings;


            }
            catch (Exception e)
            {
                TFTVLogger.Error(e);
                throw;
            }
        }


        public static void CreateRanksDictionary()
        {
            try
            {
                ranks.Add("ban", ConvertKeysToStrings(ban_NameRanks));
                ranks.Add("nj", ConvertKeysToStrings(nj_NameRanks));
                ranks.Add("anu", ConvertKeysToStrings(anu_NameRanks));
                ranks.Add("syn", ConvertKeysToStrings(syn_NameRanks));
                ranks.Add("Purists", ConvertKeysToStrings(pu_NameRanks));
                ranks.Add("FallenOnes", ConvertKeysToStrings(fo_NameRanks));
            }
            catch (Exception e)
            {
                TFTVLogger.Error(e);
            }
        }




        public static List<string> pu_Adjectives = new List<string> { "de Métal", "Ferraille", "Titanium", "Huile", "Servo", "Mech", "Cable", "Mesh", "Robot", "Quantum", "Cyber", "Mort", "Fil", "Bit", "Gear" };

        public static List<string> ban_Names = new List<string> {"Nuka-Cola","Kessar","Vipère","Rictus","Nux","Dag","Ace","Barry","Mohican","Croc","Clunk",
            "Molosse","Fuk-Ushima","Fifi Macaffe","Sol","Billy","Bastos","Morsov","Mandrin","Maraulf","Sarse","Sav","Brad","Pat'","Jesse",
            "Emile","Jacques","Slit","Joker","Filet","Bonnie","Al Capone","Arsène","Kane","Kayser","Cervelle","ChienFou","Coyote","Mesrine",
            "Bonno","Yo-yo","Madskunky","Neron","Bobby", "Cobra", "Cyrus", "Cochise", "Rembrandt", "Luther", "Vermine", "D.J.","Orphan",
             "Tronçonneuse", "Dynamo", "Boule", "Subzéro", "Lucus le Destructeur", "Frank", "Hercule","Bob", "Jean", "Clyde", "Papillon", "Harley"};

        public static List<string> nj_Names = new List<string> {"Rockatansky","Bryant","Richter", "Ripley","Amos" ,"Draper","Caleb" ,"Chasseur","Tempete","Kruger",
            "Sinclair","Morgane","Musk","Jackson","Hicks","Vasquez","Hudson","Ferro","Spunkmeyer", "Dietrich","Frost","Drake","Wierzbowski","Payne","Ventura","Dutch",
            "Dillon","Hawkins","Mac","Poncho","Walker","Charlie","Wez","Ziggy", "Alex","Max","Miller","Kenzo","Karal","Katoa","Okoye","Fagan","Avery","Parker",
            "Joyce","Kai","Angel","Jesse","Riley","Ash", "Finley","Shaw","Vickers","Marno","Jikkola","Kris","Strid","Showalter","Grimsrud","Cox","J. Matrix","Blaine",
            "Quaid","Cohaagen","Rasczak","Diz Flores","Duderino","Justo","Zander", "Ace Levy", "Shujimi","Watkins","Deladier","Lumbreiser","Kitten Smith","Ibanez","Zim"};

        public static List<string> anu_Names = new List<string> {"Walker","Charlie","Wez","Ziggy","Alex","Max","Miller","Kenzo","Karal","Katoa",
            "Okoye","Fagan","Avery","Parker","Joyce","Kai","Angel","Jesse","Riley","Ash","Finley","Shaw","Vickers","Marno","Jikkola",
            "Kris","Strid","Showalter","Grimsrud", "Aigazy", "Bassar", "Erasyl", "Gani", "Itymbai", "Kibrik","Murat", "Rakhat","Sadvaqas", "Tjatigul", "Zhandos", "Valar",
        "Kelar", "Thandor", "Ary", "Zerik", "Ravyn","Kaelin","Sylar", "Marik", "Joran", "Aerin", "Lirael","Elys","Nyxar","Varin","Seraph","Vaelis","Kaelor","Drystan", "Soren"};

        public static List<string> syn_Names = new List<string> {"Nagata","Inaros","Avasarala","Liberté","Fraternité","Egalité","Lénine","Tenet",
            "Campion","Meseeks","Squanchy","Nimbus","Mojo","Nostromo","Odyssée","Bono","Eli","Naru","Taabe","Sanchez","Walker","Charlie",
            "Wez","Ziggy","Alex","Max","Miller","Kenzo","Karal","Katoa","Okoye","Fagan","Avery","Parker","Joyce","Kai","Angel","Jesse",
            "Riley","Ash","Finley","Shaw","Vickers","Marno","Jikkola","Kris","Strid","Showalter","Grimsrud","Scruggs","L. Gopnik","S. Ableman","R. Marshak",
            "Ulysses","La Boeuf","Reuben","Cogburn", "Constanza", "Grant", "Lebowksi", "Sobchak", "Louis"};

        public static List<string> pu_Names = new List<string> {"Walker","Charlie","Wez","Ziggy","Alex","Max","Miller","Kenzo","Karal","Katoa",
            "Okoye","Fagan","Avery","Parker","Joyce","Kai","Angel","Jesse","Riley","Ash","Finley","Shaw","Vickers", "Showalter","Grimsrud", "AirKris",
            "AndyP","Brunks","Dante","Geda","GeoDao","Hokken","Gollop","Kal","Millioneigher","NoobCaptain","Origami","Ravenoïde","Bobby","Stridtul",
            "Tyraenon","Ikobot","Valygar","E.E.","BFIG","Sheepy","Conductiv","mad2342","Pantolomin", "Etermes"};

        public static List<string> fo_Names = new List<string> {"Thriceborn","Shai-Hulud","Shorr Kan","Yurtle","Lorax","Seer","Belial","Torinus",
            "Voland","Yar-Shalak","Ghul","Gheist","Melachot","Xélote","Nacht-Zur'acht","Bane","Oshazahul","Slithering","Azelot",
            "Ursuk","Hottaku","Weirdling","Outsider","Tleilaxu","Tuek","Whisperblade","Bladehands", "Yokes", "Thoth", "Amon", "Belit", "Numedides", "Taramis",
        "Azarax","Valyndor","Ebonshade","Zephyrial","Drakarn","Shadowweaver","Nyxaris","Thalorin","Voxariel","Malachai","Caelinor","Zylthar","Serpentia","Vaelus","Myrkaal", "Xylerith",
            "Vortigan", "Morvain","Sylveran","Ashryn"
        };

        public static List<string> ban_NameRanks = new List<string> {
 "HUMAN_ENEMIES_KEY_BOSS",
"HUMAN_ENEMIES_KEY_ENFORCER",
"HUMAN_ENEMIES_KEY_RAIDER",
"HUMAN_ENEMIES_KEY_CARRION"
        };
        public static List<string> nj_NameRanks = new List<string> {
            "HUMAN_ENEMIES_KEY_LEADER",
"HUMAN_ENEMIES_KEY_VETERAN",
"HUMAN_ENEMIES_KEY_JACKBOOT",
"HUMAN_ENEMIES_KEY_GREENHORN"};
        public static List<string> syn_NameRanks = new List<string> {
            "HUMAN_ENEMIES_KEY_WARDEN",
"HUMAN_ENEMIES_KEY_RANGER",
"HUMAN_ENEMIES_KEY_PEACEKEEPER",
"HUMAN_ENEMIES_KEY_CITIZEN"};
        public static List<string> anu_NameRanks = new List<string> {
            "HUMAN_ENEMIES_KEY_TAXIARCH",
"HUMAN_ENEMIES_KEY_ADEPT",
"HUMAN_ENEMIES_KEY_ACOLYTE",
"HUMAN_ENEMIES_KEY_NEOPHYTE"};
        public static List<string> pu_NameRanks = new List<string> {
            "HUMAN_ENEMIES_KEY_MACHINA",
"HUMAN_ENEMIES_KEY_METALHEART",
"HUMAN_ENEMIES_KEY_CLEANSED",
"HUMAN_ENEMIES_KEY_MEATHEAD"};
        public static List<string> fo_NameRanks = new List<string> {
            "HUMAN_ENEMIES_KEY_SCOURGE",
"HUMAN_ENEMIES_KEY_EVOLVED",
"HUMAN_ENEMIES_KEY_REBORN",
"HUMAN_ENEMIES_KEY_FLEDGLING"};

        public static string tier1description = "HUMAN_ENEMIES_KEY_TIER_1";//"Creates special effect on allies and/or enemies (Tactic). Allies lose 4 WP if character dies.";
        public static string tier2description = "HUMAN_ENEMIES_KEY_TIER_2";//"Allies who can see the character gain +1 WP per turn and lose 3WP if character dies.";
        public static string tier3description = "HUMAN_ENEMIES_KEY_TIER_3";//"Allies lose 2 WP if character dies.";
        public static string tier4description = "HUMAN_ENEMIES_KEY_TIER_4";//"Allies do not lose WP if character dies. Nobody expects them to live long anyway.";

        public static List<string> tierDescriptions = new List<string> ();

        public static void CreateTierDescriptions()
        {
            try
            {
                tierDescriptions.Add(new LocalizedTextBind() { LocalizationKey = tier1description }.Localize());
                tierDescriptions.Add(new LocalizedTextBind() { LocalizationKey = tier2description }.Localize());
                tierDescriptions.Add(new LocalizedTextBind() { LocalizationKey = tier3description }.Localize());
                tierDescriptions.Add(new LocalizedTextBind() { LocalizationKey = tier4description }.Localize());

            }
            catch (Exception e)
            {
                TFTVLogger.Error(e);
            }
        }

    }
}
