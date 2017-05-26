using System;
using System.Collections.Generic;
using GTANetworkServer;
using Serverside.Core;

namespace Serverside.CharacterCreator
{
    public class CharacterCreator
    {
        private API api = new API();
        private Client Player { get; set; }

        #region Statyczne listy obiektów

        public static readonly List<String> Mens = new List<String>
        {
            "0","1","2","3","4","5","6","7","8","9","10","11","12","13","14","15","16","17","18","19","20","42","43","44"
        };

        public static readonly List<String> Womans = new List<String>
        {
            "21","22","23","24","25","26","27","28","29","30","31","32","33","34","35","36","37","38","39","40","41"
        };

        public static List<String> MenLegs
        {
            get
            {
                List<String> legs = new List<string>();

                for (int i = 0; i < 85; i++)
                {
                    legs.Add(i.ToString());
                }

                //Wyrzucam wszystkie kurwo-stroje z kreatora
                legs.RemoveAt(11);
                legs.RemoveAt(14);
                legs.RemoveAt(21);
                legs.RemoveAt(40);
                legs.RemoveAt(61);
                legs.Sort();

                return legs;
            }
        }

        public static List<String> WomanLegs
        {
            get
            {
                List<String> legs = new List<string>();

                for (int i = 0; i < 88; i++)
                {
                    legs.Add(i.ToString());
                }

                //Wyrzucam wszystkie kurwo-stroje z kreatora
                legs.RemoveAt(13);
                legs.RemoveAt(15);
                legs.RemoveAt(17);
                legs.RemoveAt(19);
                legs.RemoveAt(20);
                legs.RemoveAt(21);
                legs.RemoveAt(22);
                legs.RemoveAt(46);
                legs.RemoveAt(56);
                legs.RemoveAt(59);
                legs.RemoveAt(62);
                legs.RemoveAt(63);
                legs.Sort();

                return legs;
            }
        }
        
        public static List<String> MenFeet
        {
            get
            {
                List<String> feets = new List<string>();

                for (int i = 0; i < 58; i++)
                {
                    feets.Add(i.ToString());
                }

                return feets;
            }
        }

        public static List<String> WomanFeets
        {
            get
            {
                List<String> feets = new List<string>();

                for (int i = 0; i < 61; i++)
                {
                    feets.Add(i.ToString());
                }

                return feets;
            }
        }

        public static List<String> MenAccesories
        {
            get
            {
                List<String> accesories = new List<string>();

                for (int i = 0; i < 124; i++)
                {
                    accesories.Add(i.ToString());
                }

                //Nic tu nie ma
                accesories.RemoveRange(1, 8);
                accesories.RemoveRange(56, 17);
                //accesories.RemoveRange(95, 13);

                accesories.RemoveAt(13);
                accesories.RemoveAt(14);
                accesories.RemoveAt(15);
                accesories.RemoveAt(84);
                

                return accesories;
            }
        }

        public static List<String> WomanAccesories
        {
            get
            {
                List<String> accesories = new List<string>();

                for (int i = 0; i < 94; i++)
                {
                    accesories.Add(i.ToString());
                }

                //Nic tu nie ma
                accesories.Remove("43");
                accesories.Remove("44");
                accesories.Remove("45");
                accesories.Remove("46");
                accesories.Remove("47");
                accesories.Remove("48");
                accesories.Remove("49");
                accesories.Remove("50");
                accesories.Remove("51");
                accesories.Remove("52");
                accesories.Remove("63");
                accesories.Remove("74");
                accesories.Remove("75");
                accesories.Remove("76");
                accesories.Remove("77");
                accesories.Remove("78");
                accesories.Remove("79");
                accesories.Remove("80");

                return accesories;
            }
        }


        public static List<String> MenHats
        {
            get
            {
                List<String> hats = new List<string>();

                for (int i = 0; i < 102; i++)
                {
                    hats.Add(i.ToString());
                }

                return hats;
            }
        }

        public static List<String> WomanHats
        {
            get
            {
                List<String> hats = new List<string>();

                for (int i = 0; i < 101; i++)
                {
                    hats.Add(i.ToString());
                }

                return hats;
            }
        }

        public static List<String> MenGlasses
        {
            get
            {
                List<String> glasses = new List<string>();

                for (int i = 0; i < 25; i++)
                {
                    glasses.Add(i.ToString());
                }

                return glasses;
            }
        }

        public static List<String> WomanGlasses
        {
            get
            {
                List<String> glasses = new List<string>();

                for (int i = 0; i < 27; i++)
                {
                    glasses.Add(i.ToString());
                }

                return glasses;
            }
        }

        public static List<String> MenEars
        {
            get
            {
                List<String> ears = new List<string>();

                for (int i = 0; i < 36; i++)
                {
                    ears.Add(i.ToString());
                }

                return ears;
            }
        }

        public static List<String> WomanEars
        {
            get
            {
                List<String> ears = new List<string>();

                for (int i = 0; i < 17; i++)
                {
                    ears.Add(i.ToString());
                }

                return ears;
            }
        }

        public static List<String> MensHairId
        {
            get
            {
                List<String> hairs = new List<string>();

                for (int i = 0; i < 36; i++)
                {
                    hairs.Add(i.ToString());
                }

                //Noktowizja
                hairs.Remove("23");

                return hairs;
            }
        }

        public static List<String> WomansHairId
        {
            get
            {
                List<String> hairs = new List<string>();

                for (int i = 0; i < 38; i++)
                {
                    hairs.Add(i.ToString());
                }

                //Noktowizja
                hairs.Remove("24");

                return hairs;
            }
        }

        #endregion

        //public static List<String> OpacityList
        //{
        //    get
        //    {
        //        var list = new List<String>(10);

        //        for (float i = 0; i < list.Count; i += 0.1f)
        //        {
        //            list.Add(i.ToString());
        //        }

        //        return list;
        //    }
        //}

        public CharacterCreator(Client player)
        {
            Player = player;
        }

        private int hairId;

        public int HairId
        {
            get { return hairId; }

            set
            {
                hairId = value;
                api.setPlayerClothes(Player, 2, value, 0);
            }
        }

        private int hairTexture;

        public int HairTexture
        {
            get { return hairTexture; }

            set
            {
                hairTexture = value;
                api.setPlayerClothes(Player, 2, hairId, value);
            }
        }

        private int hairColor;

        public int HairColor
        {
            get { return hairColor; }
            set
            {
                hairColor = value;
                //_SET_PED_HAIR_COLOR(Ped ped, int colorID, int highlightColorID)

                //dziwnie działa
                //api.setPlayerClothes(Player, 2, hairId, value);

                api.sendNativeToPlayer(Player, Hash._SET_PED_HAIR_COLOR, Player.handle, value, 0);
            }
        }

        private int motherId;

        public int MotherId
        {
            get { return motherId; }
            set
            {
                //SET_PED_HEAD_BLEND_DATA(Ped ped, int shapeFirstID, int shapeSecondID,
                //int shapeThirdID, int skinFirstID, int skinSecondID, int skinThirdID, float shapeMix,
                //float skinMix, float thirdMix, BOOL isParent)
                motherId = value;
                api.sendNativeToPlayer(Player, Hash.SET_PED_HEAD_BLEND_DATA, Player.handle, value, fatherId,
                    0, value, fatherId, 0, shapeMix, skinMix, 0f, false);
            }
        }

        private int fatherId;

        public int FatherId
        {
            get { return fatherId; }
            set
            {
                fatherId = value;
                api.sendNativeToPlayer(Player, Hash.SET_PED_HEAD_BLEND_DATA, Player.handle, motherId, value,
                    0, motherId, value, 0, shapeMix, skinMix, 0f, false);
            }
        }

        private float shapeMix;

        public float ShapeMix
        {
            get { return shapeMix; }
            set
            {
                shapeMix = value;
                api.sendNativeToPlayer(Player, Hash.SET_PED_HEAD_BLEND_DATA, Player.handle, motherId, fatherId,
                    0, motherId, fatherId, 0, value, skinMix, 0f, false);
            }
        }

        private float skinMix;

        public float SkinMix
        {
            get { return skinMix; }
            set
            {
                skinMix = value;
                api.sendNativeToPlayer(Player, Hash.SET_PED_HEAD_BLEND_DATA, Player.handle, motherId, fatherId,
                    0, motherId, fatherId, 0, shapeMix, value, 0f, false);
            }
        }       

        private int eyeBrowsId;

        public int EyeBrowsId
        {
            get { return eyeBrowsId; }
            set
            {
                eyeBrowsId = value;
                //SET_PED_HEAD_OVERLAY(Ped ped, int overlayID, int index, float opacity)
                api.sendNativeToPlayer(Player, Hash.SET_PED_HEAD_OVERLAY, Player.handle, 2, value, eyeBrowsOpacity);
            }
        }

        private float eyeBrowsOpacity;

        public float EyeBrowsOpacity
        {
            get { return eyeBrowsOpacity; }
            set
            {
                eyeBrowsOpacity = value;
                api.sendNativeToPlayer(Player, Hash.SET_PED_HEAD_OVERLAY, Player.handle, 2, eyeBrowsId, value);
            }
        }

        private int firstEyeBrowsColor;

        public int FirstEyeBrowsColor
        {
            get { return firstEyeBrowsColor; }
            set
            {
                firstEyeBrowsColor = value;
                //_SET_PED_HEAD_OVERLAY_COLOR(Ped ped, int overlayID, int colorType, int colorID, int secondColorID)
                api.sendNativeToPlayer(Player, Hash._SET_PED_HEAD_OVERLAY_COLOR, Player.handle, 2, 1,
                    value, secondEyeBrowsColor);
            }
        }

        private int secondEyeBrowsColor;

        public int SecondEyeBrowsColor
        {
            get { return secondEyeBrowsColor; }
            set
            {
                secondEyeBrowsColor = value;
                api.sendNativeToPlayer(Player, Hash._SET_PED_HEAD_OVERLAY_COLOR, Player.handle, 2, 1,
                    firstEyeBrowsColor, value);
            }
        }

        private int lipstickId;

        public int LipstickId
        {
            get { return eyeBrowsId; }
            set
            {
                lipstickId = value;
                //SET_PED_HEAD_OVERLAY(Ped ped, int overlayID, int index, float opacity) 
                api.sendNativeToPlayer(Player, Hash.SET_PED_HEAD_OVERLAY, Player.handle, 8, value, lipstickOpacity);
            }
        }

        private float lipstickOpacity;

        public float LipstickOpacity
        {
            get { return eyeBrowsOpacity; }
            set
            {
                lipstickOpacity = value;
                api.sendNativeToPlayer(Player, Hash.SET_PED_HEAD_OVERLAY, Player.handle, 8, lipstickId, value);
            }
        }

        private int firstLipstickColor;

        public int FirstLipstickColor
        {
            get { return firstLipstickColor; }
            set
            {
                firstLipstickColor = value;
                //_SET_PED_HEAD_OVERLAY_COLOR(Ped ped, int overlayID, int colorType, int colorID, int secondColorID)
                //2
                api.sendNativeToPlayer(Player, Hash._SET_PED_HEAD_OVERLAY_COLOR, Player.handle, 8, 1,
                    value, secondLipstickColor);
            }
        }

        private int secondLipstickColor;

        public int SecondLipstickColor
        {
            get { return secondLipstickColor; }
            set
            {
                secondLipstickColor = value;
                //_SET_PED_HEAD_OVERLAY_COLOR(Ped ped, int overlayID, int colorType, int colorID, int secondColorID)
                //2
                api.sendNativeToPlayer(Player, Hash._SET_PED_HEAD_OVERLAY_COLOR, Player.handle, 8, 1,
                    firstEyeBrowsColor, value);
            }
        }

        private int makeupId;

        public int MakeupId
        {
            get { return makeupId; }
            set
            {
                makeupId = value;
                //SET_PED_HEAD_OVERLAY(Ped ped, int overlayID, int index, float opacity) 
                api.sendNativeToPlayer(Player, Hash.SET_PED_HEAD_OVERLAY, Player.handle, 4, value, makeupOpacity);
            }
        }

        private float makeupOpacity;

        public float MakeupOpacity
        {
            get { return makeupOpacity; }
            set
            {
                makeupOpacity = value;
                //SET_PED_HEAD_OVERLAY(Ped ped, int overlayID, int index, float opacity) 
                api.sendNativeToPlayer(Player, Hash.SET_PED_HEAD_OVERLAY, Player.handle, 4, makeupId, value);
            }
        }

        private int firstMakeupColor;

        public int FirstMakeupColor
        {
            get { return firstMakeupColor; }
            set
            {
                firstMakeupColor = value;
                //_SET_PED_HEAD_OVERLAY_COLOR(Ped ped, int overlayID, int colorType, int colorID, int secondColorID)
                //0
                api.sendNativeToPlayer(Player, Hash._SET_PED_HEAD_OVERLAY_COLOR, Player.handle, 4, 1,
                    value, secondMakeupColor);
            }
        }

        private int secondMakeupColor;

        public int SecondMakeupColor
        {
            get { return secondMakeupColor; }
            set
            {
                secondMakeupColor = value;
                //_SET_PED_HEAD_OVERLAY_COLOR(Ped ped, int overlayID, int colorType, int colorID, int secondColorID)
                //0
                api.sendNativeToPlayer(Player, Hash._SET_PED_HEAD_OVERLAY_COLOR, Player.handle, 4, 1,
                    firstMakeupColor, value);
            }
        }

        public void SetPlayerFaceFeatures(Dictionary<int, float> faceFeatures)
        {
            if (faceFeatures.Count <= 21)
            {
                foreach (var item in faceFeatures)
                {
                    //_SET_PED_FACE_FEATURE(Ped ped, int index, float scale)
                    api.sendNativeToPlayer(Player, Hash._SET_PED_FACE_FEATURE, Player.handle, item.Key, item.Value);
                }
            }
        }

        private int legsId;

        public int LegsId
        {
            get { return legsId; }
            set
            {
                legsId = value;
                api.setPlayerClothes(Player, 4, value, legsTexture);
            }
        }

        private int legsTexture;

        public int LegsTexture
        {
            get { return legsTexture; }
            set
            {
                legsTexture = value;
                api.setPlayerClothes(Player, 4, legsId, value);
            }
        }

        private int feetsId;

        public int FeetsId
        {
            get { return feetsId; }
            set
            {
                feetsId = value;
                api.setPlayerClothes(Player, 6, value, feetsTexture);
            }
        }

        private int feetsTexture;

        public int FeetsTexture
        {
            get { return feetsTexture; }
            set
            {
                feetsTexture = value;
                api.setPlayerClothes(Player, 6, feetsId, value);
            }
        }

        private int accessoryId;

        public int AccessoryId
        {
            get { return accessoryId; }
            set
            {
                accessoryId = value;
                api.setPlayerClothes(Player, 7, value, accessoryTexture);
            }
        }

        private int accessoryTexture;

        public int AccessoryTexture
        {
            get { return accessoryTexture; }
            set
            {
                accessoryTexture = value;
                api.setPlayerClothes(Player, 7, accessoryId, value);
            }
        }

        private int hatId;

        public int HatId
        {
            get { return hatId; }
            set
            {
                hatId = value;
                api.setPlayerAccessory(Player, 0, value, hatTexture);
            }
        }

        private int hatTexture;

        public int HatTexture
        {
            get { return hatTexture; }
            set
            {
                hatTexture = value;
                api.setPlayerAccessory(Player, 0, hatId, value);
            }
        }

        private int glassesId;

        public int GlassesId
        {
            get { return glassesId; }
            set
            {
                glassesId = value;
                api.setPlayerAccessory(Player, 1, value, glassesTexture);
            }
        }

        private int glassesTexture;

        public int GlassesTexture
        {
            get { return glassesTexture; }
            set
            {
                glassesTexture = value;
                api.setPlayerAccessory(Player, 1, glassesId, value);
            }
        }
        
        private int earsId;

        public int EarsId
        {
            get { return earsId; }
            set
            {
                earsId = value;
                api.setPlayerAccessory(Player, 2, value, earsTexture);
            }
        }

        private int earsTexture;

        public int EarsTexture
        {
            get { return earsTexture; }
            set
            {
                earsTexture = value;
                api.setPlayerAccessory(Player, 2, earsId, value);
            }
        }

        public void ClearPlayerAccessory()
        {
            
        }

        private int topId;

        public int TopId
        {
            get { return topId; }
            set
            {
                topId = value;
                //api.sendNativeToPlayer(Player, Hash.SET_PED_COMPONENT_VARIATION, Player.handle, 3, value, topTexture, 2);
                api.setPlayerClothes(Player, 11, value, topTexture);
            }
        }

        private int topTexture;

        public int TopTexture
        {
            get { return topTexture; }
            set
            {
                topTexture = value;
                //api.sendNativeToPlayer(Player, Hash.SET_PED_COMPONENT_VARIATION, Player.handle, 3, topId, value, 2);
                api.setPlayerClothes(Player, 11, topId, value);
            }
        }

        private int torsoId;

        public int TorsoId
        {
            get { return torsoId; }
            set
            {
                torsoId = value;
                api.setPlayerClothes(Player, 3, value, 0);
            }
        }

        private int undershirtId;

        public int UndershirtId
        {
            get { return undershirtId; }
            set
            {
                undershirtId = value;
                api.setPlayerClothes(Player, 8, value, undershirtTexture);
            }
        }

        private int undershirtTexture;

        public int UndershirtTexture
        {
            get { return undershirtTexture; }
            set
            {
                undershirtTexture = value;
                api.setPlayerClothes(Player, 8, undershirtId, value);
            }
        }
    }
//    paletteId/palletColor - 0 to 3. 
//enum PedVariationData
//    {
//        PED_VARIATION_FACE = 0,
//        PED_VARIATION_HEAD = 1,
//        PED_VARIATION_HAIR = 2,
//        PED_VARIATION_TORSO = 3,
//        PED_VARIATION_LEGS = 4,
//        PED_VARIATION_HANDS = 5,
//        PED_VARIATION_FEET = 6,
//        PED_VARIATION_EYES = 7,
//        PED_VARIATION_ACCESSORIES = 8,
//        PED_VARIATION_TASKS = 9,
//        PED_VARIATION_TEXTURES = 10,
//        PED_VARIATION_TORSO2 = 11
//    };
//    Usage: 
//SET_PED_COMPONENT_VARIATION(playerPed, PED_VARIATION_FACE, GET_NUMBER_OF_PED_DRAWABLE_VARIATIONS(playerPed, PED_VARIATION_FACE), GET_NUMBER_OF_PED_TEXTURE_VARIATIONS(playerPed, PED_VARIATION_FACE, 0), 2);
}