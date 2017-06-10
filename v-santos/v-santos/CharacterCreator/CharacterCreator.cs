using System;
using System.Collections.Generic;
using GTANetworkServer;
using Serverside.Controllers;

namespace Serverside.CharacterCreator
{
    public class CharacterCreator
    {
        private API _api = new API();
        private CharacterController CharacterController { get; set; }

        public CharacterCreator(CharacterController characterController)
        {
            CharacterController = characterController;
            
            if (CharacterController.Character.AccessoryId != null) AccessoryId = CharacterController.Character.AccessoryId.Value;
            if (CharacterController.Character.AccessoryTexture != null) AccessoryTexture = CharacterController.Character.AccessoryTexture.Value;
            if (CharacterController.Character.EarsId != null) EarsId = CharacterController.Character.EarsId.Value;
            if (CharacterController.Character.EarsTexture != null) EarsTexture = CharacterController.Character.EarsTexture.Value;
            if (CharacterController.Character.EyeBrowsOpacity != null) EyeBrowsOpacity = CharacterController.Character.EyeBrowsOpacity.Value;
            if (CharacterController.Character.EyebrowsId != null) EyeBrowsId = CharacterController.Character.EyebrowsId.Value;
            if (CharacterController.Character.FatherId != null) FatherId = CharacterController.Character.FatherId.Value;
            if (CharacterController.Character.FirstEyebrowsColor != null) FirstEyeBrowsColor = CharacterController.Character.FirstEyebrowsColor.Value;
            if (CharacterController.Character.SecondEyebrowsColor != null) SecondEyeBrowsColor = CharacterController.Character.SecondEyebrowsColor.Value;
            if (CharacterController.Character.FirstLipstickColor != null) FirstLipstickColor = CharacterController.Character.FirstLipstickColor.Value;
            if (CharacterController.Character.SecondLipstickColor != null) SecondLipstickColor = CharacterController.Character.SecondLipstickColor.Value;
            if (CharacterController.Character.FirstMakeupColor != null) FirstMakeupColor = CharacterController.Character.FirstMakeupColor.Value;
            if (CharacterController.Character.SecondMakeupColor != null) SecondMakeupColor = CharacterController.Character.SecondMakeupColor.Value;
            if (CharacterController.Character.MotherId != null) MotherId = CharacterController.Character.MotherId.Value;
            if (CharacterController.Character.GlassesId != null) GlassesId = CharacterController.Character.GlassesId.Value;
            if (CharacterController.Character.GlassesTexture != null) GlassesTexture = CharacterController.Character.GlassesTexture.Value;
            if (CharacterController.Character.HairId != null) HairId = CharacterController.Character.HairId.Value;
            if (CharacterController.Character.HairTexture != null) HairTexture = CharacterController.Character.HairTexture.Value;
            if (CharacterController.Character.HairColor != null) HairColor = CharacterController.Character.HairColor.Value;
            if (CharacterController.Character.HatId != null) HatId = CharacterController.Character.HatId.Value;
            if (CharacterController.Character.HatTexture != null) HatTexture = CharacterController.Character.HatTexture.Value;
            if (CharacterController.Character.LegsId != null) LegsId = CharacterController.Character.LegsId.Value;
            if (CharacterController.Character.LegsTexture != null) LegsTexture = CharacterController.Character.LegsTexture.Value;
            if (CharacterController.Character.LipstickOpacity != null) LipstickOpacity = CharacterController.Character.LipstickOpacity.Value;
            if (CharacterController.Character.MakeupId != null) MakeupId = CharacterController.Character.MakeupId.Value;
            if (CharacterController.Character.MakeupOpacity != null) MakeupOpacity = CharacterController.Character.MakeupOpacity.Value;
            if (CharacterController.Character.UndershirtId != null) UndershirtId = CharacterController.Character.UndershirtId.Value;
            if (CharacterController.Character.TorsoId != null) TorsoId = CharacterController.Character.TorsoId.Value;
            if (CharacterController.Character.TopTexture != null) TopTexture = CharacterController.Character.TopTexture.Value;
            if (CharacterController.Character.TopId != null) TopId = CharacterController.Character.TopId.Value;
            if (CharacterController.Character.ShoesTexture != null) FeetsTexture = CharacterController.Character.ShoesTexture.Value;
            if (CharacterController.Character.ShoesId != null) FeetsId = CharacterController.Character.ShoesId.Value;
            if (CharacterController.Character.ShapeMix != null) ShapeMix = CharacterController.Character.ShapeMix.Value;
        }

        public void Save()
        {
            CharacterController.Character.AccessoryId = AccessoryId;
            CharacterController.Character.AccessoryTexture = AccessoryTexture;
            CharacterController.Character.EarsId = EarsId;
            CharacterController.Character.EarsTexture = EarsTexture;
            CharacterController.Character.EyeBrowsOpacity = EyeBrowsOpacity;
            CharacterController.Character.EyebrowsId = EyeBrowsId;
            CharacterController.Character.FatherId = FatherId;
            CharacterController.Character.FirstEyebrowsColor = FirstEyeBrowsColor;
            CharacterController.Character.SecondEyebrowsColor = SecondEyeBrowsColor;
            CharacterController.Character.FirstLipstickColor = FirstLipstickColor;
            CharacterController.Character.SecondLipstickColor = SecondLipstickColor;
            CharacterController.Character.FirstMakeupColor = FirstMakeupColor;
            CharacterController.Character.SecondMakeupColor = SecondMakeupColor;
            CharacterController.Character.MotherId = MotherId;
            CharacterController.Character.GlassesId = GlassesId;
            CharacterController.Character.GlassesTexture = GlassesTexture;
            CharacterController.Character.HairId = HairId;
            CharacterController.Character.HairTexture = HairTexture;
            CharacterController.Character.HairColor = HairColor;
            CharacterController.Character.HatId = HatId;
            CharacterController.Character.LegsId = LegsId;
            CharacterController.Character.LegsTexture = LegsTexture;
            CharacterController.Character.LipstickOpacity = LipstickOpacity;
            CharacterController.Character.MakeupId = MakeupId;
            CharacterController.Character.MakeupOpacity = MakeupOpacity;
            CharacterController.Character.UndershirtId = UndershirtId;
            CharacterController.Character.TorsoId = TorsoId;
            CharacterController.Character.TopTexture = TopTexture;
            CharacterController.Character.TopId = TopId;
            CharacterController.Character.ShoesTexture = FeetsTexture;
            CharacterController.Character.ShoesId = FeetsId;
            CharacterController.Character.ShapeMix = ShapeMix;
            CharacterController.Save();
        }

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


        private int _hairId;

        public int HairId
        {
            get => _hairId;

            set
            {
                _hairId = value;
                _api.setPlayerClothes(CharacterController.AccountController.Client, 2, value, 0);
            }
        }

        private int _hairTexture;

        public int HairTexture
        {
            get => _hairTexture;

            set
            {
                _hairTexture = value;
                _api.setPlayerClothes(CharacterController.AccountController.Client, 2, _hairId, value);
            }
        }

        private int _hairColor;

        public int HairColor
        {
            get => _hairColor;
            set
            {
                _hairColor = value;
                //_SET_PED_HAIR_COLOR(Ped ped, int colorID, int highlightColorID)

                //dziwnie działa
                //api.setPlayerClothes(CharacterController.AccountController.Client, 2, hairId, value);

                _api.sendNativeToPlayer(CharacterController.AccountController.Client, Hash._SET_PED_HAIR_COLOR, CharacterController.AccountController.Client.handle, value, 0);
            }
        }

        private int _motherId;

        public int MotherId
        {
            get => _motherId;
            set
            {
                //SET_PED_HEAD_BLEND_DATA(Ped ped, int shapeFirstID, int shapeSecondID,
                //int shapeThirdID, int skinFirstID, int skinSecondID, int skinThirdID, float shapeMix,
                //float skinMix, float thirdMix, BOOL isParent)
                _motherId = value;
                _api.sendNativeToPlayer(CharacterController.AccountController.Client, Hash.SET_PED_HEAD_BLEND_DATA, CharacterController.AccountController.Client.handle, value, _fatherId,
                    0, value, _fatherId, 0, _shapeMix, _skinMix, 0f, false);
            }
        }

        private int _fatherId;

        public int FatherId
        {
            get => _fatherId;
            set
            {
                _fatherId = value;
                _api.sendNativeToPlayer(CharacterController.AccountController.Client, Hash.SET_PED_HEAD_BLEND_DATA, CharacterController.AccountController.Client.handle, _motherId, value,
                    0, _motherId, value, 0, _shapeMix, _skinMix, 0f, false);
            }
        }

        private float _shapeMix;

        public float ShapeMix
        {
            get => _shapeMix;
            set
            {
                _shapeMix = value;
                _api.sendNativeToPlayer(CharacterController.AccountController.Client, Hash.SET_PED_HEAD_BLEND_DATA, CharacterController.AccountController.Client.handle, _motherId, _fatherId,
                    0, _motherId, _fatherId, 0, value, _skinMix, 0f, false);
            }
        }

        private float _skinMix;

        public float SkinMix
        {
            get => _skinMix;
            set
            {
                _skinMix = value;
                _api.sendNativeToPlayer(CharacterController.AccountController.Client, Hash.SET_PED_HEAD_BLEND_DATA, CharacterController.AccountController.Client.handle, _motherId, _fatherId,
                    0, _motherId, _fatherId, 0, _shapeMix, value, 0f, false);
            }
        }       

        private int _eyeBrowsId;

        public int EyeBrowsId
        {
            get => _eyeBrowsId;
            set
            {
                _eyeBrowsId = value;
                //SET_PED_HEAD_OVERLAY(Ped ped, int overlayID, int index, float opacity)
                _api.sendNativeToPlayer(CharacterController.AccountController.Client, Hash.SET_PED_HEAD_OVERLAY, CharacterController.AccountController.Client.handle, 2, value, _eyeBrowsOpacity);
            }
        }

        private float _eyeBrowsOpacity;

        public float EyeBrowsOpacity
        {
            get => _eyeBrowsOpacity;
            set
            {
                _eyeBrowsOpacity = value;
                _api.sendNativeToPlayer(CharacterController.AccountController.Client, Hash.SET_PED_HEAD_OVERLAY, CharacterController.AccountController.Client.handle, 2, _eyeBrowsId, value);
            }
        }

        private int _firstEyeBrowsColor;

        public int FirstEyeBrowsColor
        {
            get => _firstEyeBrowsColor;
            set
            {
                _firstEyeBrowsColor = value;
                //_SET_PED_HEAD_OVERLAY_COLOR(Ped ped, int overlayID, int colorType, int colorID, int secondColorID)
                _api.sendNativeToPlayer(CharacterController.AccountController.Client, Hash._SET_PED_HEAD_OVERLAY_COLOR, CharacterController.AccountController.Client.handle, 2, 1,
                    value, _secondEyeBrowsColor);
            }
        }

        private int _secondEyeBrowsColor;

        public int SecondEyeBrowsColor
        {
            get => _secondEyeBrowsColor;
            set
            {
                _secondEyeBrowsColor = value;
                _api.sendNativeToPlayer(CharacterController.AccountController.Client, Hash._SET_PED_HEAD_OVERLAY_COLOR, CharacterController.AccountController.Client.handle, 2, 1,
                    _firstEyeBrowsColor, value);
            }
        }

        private int _lipstickId;

        public int LipstickId
        {
            get => _eyeBrowsId;
            set
            {
                _lipstickId = value;
                //SET_PED_HEAD_OVERLAY(Ped ped, int overlayID, int index, float opacity) 
                _api.sendNativeToPlayer(CharacterController.AccountController.Client, Hash.SET_PED_HEAD_OVERLAY, CharacterController.AccountController.Client.handle, 8, value, _lipstickOpacity);
            }
        }

        private float _lipstickOpacity;

        public float LipstickOpacity
        {
            get => _eyeBrowsOpacity;
            set
            {
                _lipstickOpacity = value;
                _api.sendNativeToPlayer(CharacterController.AccountController.Client, Hash.SET_PED_HEAD_OVERLAY, CharacterController.AccountController.Client.handle, 8, _lipstickId, value);
            }
        }

        private int _firstLipstickColor;

        public int FirstLipstickColor
        {
            get => _firstLipstickColor;
            set
            {
                _firstLipstickColor = value;
                //_SET_PED_HEAD_OVERLAY_COLOR(Ped ped, int overlayID, int colorType, int colorID, int secondColorID)
                //2
                _api.sendNativeToPlayer(CharacterController.AccountController.Client, Hash._SET_PED_HEAD_OVERLAY_COLOR, CharacterController.AccountController.Client.handle, 8, 1,
                    value, _secondLipstickColor);
            }
        }

        private int _secondLipstickColor;

        public int SecondLipstickColor
        {
            get => _secondLipstickColor;
            set
            {
                _secondLipstickColor = value;
                //_SET_PED_HEAD_OVERLAY_COLOR(Ped ped, int overlayID, int colorType, int colorID, int secondColorID)
                //2
                _api.sendNativeToPlayer(CharacterController.AccountController.Client, Hash._SET_PED_HEAD_OVERLAY_COLOR, CharacterController.AccountController.Client.handle, 8, 1,
                    _firstEyeBrowsColor, value);
            }
        }

        private int _makeupId;

        public int MakeupId
        {
            get => _makeupId;
            set
            {
                _makeupId = value;
                //SET_PED_HEAD_OVERLAY(Ped ped, int overlayID, int index, float opacity) 
                _api.sendNativeToPlayer(CharacterController.AccountController.Client, Hash.SET_PED_HEAD_OVERLAY, CharacterController.AccountController.Client.handle, 4, value, _makeupOpacity);
            }
        }

        private float _makeupOpacity;

        public float MakeupOpacity
        {
            get => _makeupOpacity;
            set
            {
                _makeupOpacity = value;
                //SET_PED_HEAD_OVERLAY(Ped ped, int overlayID, int index, float opacity) 
                _api.sendNativeToPlayer(CharacterController.AccountController.Client, Hash.SET_PED_HEAD_OVERLAY, CharacterController.AccountController.Client.handle, 4, _makeupId, value);
            }
        }

        private int _firstMakeupColor;

        public int FirstMakeupColor
        {
            get => _firstMakeupColor;
            set
            {
                _firstMakeupColor = value;
                //_SET_PED_HEAD_OVERLAY_COLOR(Ped ped, int overlayID, int colorType, int colorID, int secondColorID)
                //0
                _api.sendNativeToPlayer(CharacterController.AccountController.Client, Hash._SET_PED_HEAD_OVERLAY_COLOR, CharacterController.AccountController.Client.handle, 4, 1,
                    value, _secondMakeupColor);
            }
        }

        private int _secondMakeupColor;

        public int SecondMakeupColor
        {
            get => _secondMakeupColor;
            set
            {
                _secondMakeupColor = value;
                //_SET_PED_HEAD_OVERLAY_COLOR(Ped ped, int overlayID, int colorType, int colorID, int secondColorID)
                //0
                _api.sendNativeToPlayer(CharacterController.AccountController.Client, Hash._SET_PED_HEAD_OVERLAY_COLOR, CharacterController.AccountController.Client.handle, 4, 1,
                    _firstMakeupColor, value);
            }
        }

        public void SetPlayerFaceFeatures(Dictionary<int, float> faceFeatures)
        {
            if (faceFeatures.Count <= 21)
            {
                foreach (var item in faceFeatures)
                {
                    //_SET_PED_FACE_FEATURE(Ped ped, int index, float scale)
                    _api.sendNativeToPlayer(CharacterController.AccountController.Client, Hash._SET_PED_FACE_FEATURE, CharacterController.AccountController.Client.handle, item.Key, item.Value);
                }
            }
        }

        private int _legsId;

        public int LegsId
        {
            get => _legsId;
            set
            {
                _legsId = value;
                _api.setPlayerClothes(CharacterController.AccountController.Client, 4, value, _legsTexture);
            }
        }

        private int _legsTexture;

        public int LegsTexture
        {
            get => _legsTexture;
            set
            {
                _legsTexture = value;
                _api.setPlayerClothes(CharacterController.AccountController.Client, 4, _legsId, value);
            }
        }

        private int _feetsId;

        public int FeetsId
        {
            get => _feetsId;
            set
            {
                _feetsId = value;
                _api.setPlayerClothes(CharacterController.AccountController.Client, 6, value, _feetsTexture);
            }
        }

        private int _feetsTexture;

        public int FeetsTexture
        {
            get => _feetsTexture;
            set
            {
                _feetsTexture = value;
                _api.setPlayerClothes(CharacterController.AccountController.Client, 6, _feetsId, value);
            }
        }

        private int _accessoryId;

        public int AccessoryId
        {
            get => _accessoryId;
            set
            {
                _accessoryId = value;
                _api.setPlayerClothes(CharacterController.AccountController.Client, 7, value, _accessoryTexture);
            }
        }

        private int _accessoryTexture;

        public int AccessoryTexture
        {
            get => _accessoryTexture;
            set
            {
                _accessoryTexture = value;
                _api.setPlayerClothes(CharacterController.AccountController.Client, 7, _accessoryId, value);
            }
        }

        private int _hatId;

        public int HatId
        {
            get => _hatId;
            set
            {
                _hatId = value;
                _api.setPlayerAccessory(CharacterController.AccountController.Client, 0, value, _hatTexture);
            }
        }

        private int _hatTexture;

        public int HatTexture
        {
            get => _hatTexture;
            set
            {
                _hatTexture = value;
                _api.setPlayerAccessory(CharacterController.AccountController.Client, 0, _hatId, value);
            }
        }

        private int _glassesId;

        public int GlassesId
        {
            get => _glassesId;
            set
            {
                _glassesId = value;
                _api.setPlayerAccessory(CharacterController.AccountController.Client, 1, value, _glassesTexture);
            }
        }

        private int _glassesTexture;

        public int GlassesTexture
        {
            get => _glassesTexture;
            set
            {
                _glassesTexture = value;
                _api.setPlayerAccessory(CharacterController.AccountController.Client, 1, _glassesId, value);
            }
        }
        
        private int _earsId;

        public int EarsId
        {
            get => _earsId;
            set
            {
                _earsId = value;
                _api.setPlayerAccessory(CharacterController.AccountController.Client, 2, value, _earsTexture);
            }
        }

        private int _earsTexture;

        public int EarsTexture
        {
            get => _earsTexture;
            set
            {
                _earsTexture = value;
                _api.setPlayerAccessory(CharacterController.AccountController.Client, 2, _earsId, value);
            }
        }

        public void ClearPlayerAccessory()
        {
            
        }

        private int _topId;

        public int TopId
        {
            get => _topId;
            set
            {
                _topId = value;
                //api.sendNativeToPlayer(CharacterController.AccountController.Client, Hash.SET_PED_COMPONENT_VARIATION, CharacterController.AccountController.Client.handle, 3, value, topTexture, 2);
                _api.setPlayerClothes(CharacterController.AccountController.Client, 11, value, _topTexture);
            }
        }

        private int _topTexture;

        public int TopTexture
        {
            get => _topTexture;
            set
            {
                _topTexture = value;
                //api.sendNativeToPlayer(CharacterController.AccountController.Client, Hash.SET_PED_COMPONENT_VARIATION, CharacterController.AccountController.Client.handle, 3, topId, value, 2);
                _api.setPlayerClothes(CharacterController.AccountController.Client, 11, _topId, value);
            }
        }

        private int _torsoId;

        public int TorsoId
        {
            get => _torsoId;
            set
            {
                _torsoId = value;
                _api.setPlayerClothes(CharacterController.AccountController.Client, 3, value, 0);
            }
        }

        private int _undershirtId;

        public int UndershirtId
        {
            get => _undershirtId;
            set
            {
                _undershirtId = value;
                _api.setPlayerClothes(CharacterController.AccountController.Client, 8, value, _undershirtTexture);
            }
        }

        private int _undershirtTexture;

        public int UndershirtTexture
        {
            get => _undershirtTexture;
            set
            {
                _undershirtTexture = value;
                _api.setPlayerClothes(CharacterController.AccountController.Client, 8, _undershirtId, value);
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