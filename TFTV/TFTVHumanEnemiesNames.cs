using System;
using System.Collections.Generic;

namespace TFTV
{
    internal class TFTVHumanEnemiesNames
    {
    	//Calvitix Trad
        public static string[] adjectives = new string[] {"Fous", "Déjantés", "Sournois", "Sanglants", "Déshonorés", "Glorieux", "Sombres", "des décharges",
          "Rouge", "Verts", "Bleux", "Or", "Morts", "Bullet", "Laser", "Déchiquetés", "Acides", "Toxiques", "Rampants", "Suppurants", "Corrompus", "du Kaos",
          "Explosifs", "Brisés", "Obscènes", "Féroces", "de Feu", "de Glace", "Moyens", "Mech", "de Guerre", "Suicidaires", "Instictifs", "Trancahnts", "Blasés", "Bloodied",
          "Broyés", "Maudits", "Bénis", "Alpha", "Bravo", "Charlie", "Tango", "Zoulous", "Noirs", "Blancs", "Zombies", "affamés", "assoiffés", "De la mort"};

        public static string[] nouns = new string[] { "Vipères", "Singes", "Chèvres", "Arthrons", "Pêcheurs", "Corrupteurs", "Chirons",
            "Sirènes", "Scyllas", "Acherons", "Locustes", "Buzzards", "Vaultours", "Aigles", "Bâtards", "Bâtards", "Tueurs", "Echos",
            "Lapins", "Souris", "Taureaux", "Béhémoths", "Dillos", "Mantes", "Guerriers", "Soldats", "Chiens", "Panthères", "Tortues", "Gars",
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

        public static void CreateRanksDictionary() 
        {
            try
            {
             
                ranks.Add("ban", ban_NameRanks);
                ranks.Add("nj", nj_NameRanks);
                ranks.Add("anu", anu_NameRanks);
                ranks.Add("syn", syn_NameRanks);
                ranks.Add("Purists", pu_NameRanks);
                ranks.Add("FallenOnes", fo_NameRanks);

            }
            catch (Exception e)
            {
                TFTVLogger.Error(e);
            }


        }


        public static List<string> pu_Adjectives = new List<string> { "Métal", "Ferraille", "Titanium", "Huile", "Servo", "Mech", "Cable", "Mesh", "Robot", "Quantum", "Cyber", "Death", "Wire", "Bit", "Gear" };

        public static List <string> ban_Names = new List <string> {"Nuka-Cola","Kessar","Viper","Rictus","Nux","Dag","Ace","Barry","Mohawk","Bearclaw","Clunk",
            "Deepdog","Fuk-Ushima","Fifi Macaffe","Sol","Gutgash","Ironbar","Morsov","Mudguts","Papagallo","Sarse","Sav","Roop","Blackfinger","Scrooloose",
            "Scuttle","Starbuck","Slit","Slake","Tenderloin","Toadie","Toecutter","Toast","Kane","Splitear","Brainrot","Maddog","Coyote","Birdsheet",
            "Gearhead","Yo-yo","Madskunky","Showalter","Grimsrud", "Cobra", "Cyrus", "Cochise", "Rembrandt", "Luther", "Vermin", "D.J.","Orphan",
             "Buzzsaw", "Dynamo", "Fireball", "Subzero"};

        public static List<string> nj_Names = new List<string> {"Rockatansky","Bryant","Richter", "Ripley","Amos" ,"Draper","Caleb" ,"Hunter","Tempest","Kruger",
            "Sinclair","Morgan","Musk","Jackson","Hicks","Vasquez","Hudson","Ferro","Spunkmeyer", "Dietrich","Frost","Drake","Wierzbowski","Payne","Ventura","Dutch",
            "Dillon","Hawkins","Mac","Poncho","Walker","Charlie","Wez","Ziggy", "Alex","Max","Miller","Kenzo","Karal","Katoa","Okoye","Fagan","Avery","Parker",
            "Joyce","Kai","Angel","Jesse","Riley","Ash", "Finley","Shaw","Vickers","Marno","Jikkola","Kris","Strid","Showalter","Grimsrud","Cox","J. Matrix","Blaine",
            "Quaid","Cohaagen","Rasczak","Diz Flores","Duderino","Justo","Zander", "Ace Levy", "Shujimi","Sugar Watkins","Deladier","Lumbreiser","Kitten Smith","Ibanez","Zim"};

        public static List<string> anu_Names = new List<string> {"Walker","Charlie","Wez","Ziggy","Alex","Max","Miller","Kenzo","Karal","Katoa",
            "Okoye","Fagan","Avery","Parker","Joyce","Kai","Angel","Jesse","Riley","Ash","Finley","Shaw","Vickers","Marno","Jikkola",
            "Kris","Strid","Showalter","Grimsrud"};

        public static List<string> syn_Names = new List<string> {"Nagata","Inaros","Avasarala","Liberté","Fraternité","Egalité","Lénine","Tenet",
            "Campion","Meseeks","Squanchy","Nimbus","Mojo","Nostromo","Odyssey","Bono","Eli","Naru","Taabe","Sanchez","Walker","Charlie",
            "Wez","Ziggy","Alex","Max","Miller","Kenzo","Karal","Katoa","Okoye","Fagan","Avery","Parker","Joyce","Kai","Angel","Jesse",
            "Riley","Ash","Finley","Shaw","Vickers","Marno","Jikkola","Kris","Strid","Showalter","Grimsrud","Scruggs","L. Gopnik","S. Ableman","R. Marshak",
            "Ulysses","La Boeuf","Reuben","Cogburn", "Constanza", "Grant", "Lebowksi", "Sobchak"};

        public static List<string> pu_Names = new List<string> {"Walker","Charlie","Wez","Ziggy","Alex","Max","Miller","Kenzo","Karal","Katoa",
            "Okoye","Fagan","Avery","Parker","Joyce","Kai","Angel","Jesse","Riley","Ash","Finley","Shaw","Vickers", "Showalter","Grimsrud", "AirKris",
            "AndyP","Brunks","Dante","Geda","GeoDao","Hokken","Gollop","Kal","Millioneigher","NoobCaptain","Origami","Ravenoide","Bobby","Stridtul",
            "Tyraenon","Ikobot","Valygar","E.E.","BFIG","Sheepy","Conductiv","mad2342","Pantolomin", "Etermes"};

        public static List<string> fo_Names = new List<string> {"Thriceborn","Shai-Hulud","Shorr Kan","Yurtle","Lorax","Seer","Belial","Torinus",
            "Voland","Yar-Shalak","Ghul","Gheist","Melachot","Xelot","Nacht-Zur'acht","Bane","Oshazahul","Slithering","Azelote",
            "Ursuk","Hottaku","Weirdling","Outsider","Tleilaxu","Tuek","Whisperblade","Bladehands", "Yokes"};

        public static List<string> ban_NameRanks = new List<string> { "Boss", "Exécuteur", "Voleur", "Récupérateur" };
        public static List<string> nj_NameRanks = new List<string> { "Officier", "Vétéran", "Soldat", "Bidasse" };
        public static List<string> syn_NameRanks = new List<string> { "Gardien", "Ranger", "Pacificateur", "Citoyen" };
        public static List<string> anu_NameRanks = new List<string> { "Taxiarche", "Adepte", "Acolyte", "Néophyte"};
        public static List<string> pu_NameRanks = new List<string> { "Machine", "Coeur de métal", "Purifié", "B. de Conserve"};
        public static List<string> fo_NameRanks = new List<string> { "Fléau", "Évolué", "Réincarné", "Rejeton" };

        public static string tier1description = "Crée un effet spécial sur les alliés et/ou les ennemis (Tactique). Les alliés perdent 4 Volonté si le personnage meurt.";
        public static string tier2description = "Les alliés qui peuvent voir le personnage gagnent +1 Vol. par tour et perdent 3 Vol. si le personnage meurt.";
        public static string tier3description = "Les alliés perdent 2 Volonté si le personnage meurt.";
        public static string tier4description = "Les alliés ne perdent pas de WP si le personnage meurt. Personne ne s'attend à ce qu'ils vivent longtemps de toute façon..";

        public static List<string>tierDescriptions = new List<string> { tier1description, tier2description, tier3description, tier4description };
        
    }
}
