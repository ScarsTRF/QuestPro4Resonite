﻿using HarmonyLib;
using ResoniteModLoader;
using FrooxEngine;
using System.Net;
using QuestProModule.ALXR;
using Elements.Core;
using System;
using FrooxEngine.UIX;

namespace QuestProModule
{
    public class QuestProMod : ResoniteMod
    {
		[AutoRegisterConfigKey]
		private readonly static ModConfigurationKey<string> QuestProIP = new ModConfigurationKey<string>("quest_pro_IP", "Quest Pro IP. This can be found in ALXR's settings, requires a restart to take effect", () => "127.0.0.1");

        [AutoRegisterConfigKey]
        private readonly static ModConfigurationKey<float> EyeOpennessExponent = new ModConfigurationKey<float>("quest_pro_eye_open_exponent", "Exponent to apply to eye openness.  Can be updated at runtime.  Useful for applying different curves for how open your eyes are.", () => 1.0f);

        [AutoRegisterConfigKey]
        private readonly static ModConfigurationKey<float> EyeWideMultiplier = new ModConfigurationKey<float>("quest_pro_eye_wide_multiplier", "Multiplier to apply to eye wideness.  Can be updated at runtime.  Useful for multiplying the amount your eyes can widen by.", () => 1.0f);

        [AutoRegisterConfigKey]
        private static readonly ModConfigurationKey<float> EyeMovementMultiplier = new ModConfigurationKey<float>("quest_pro_eye_movement_multiplier", "Multiplier to adjust the movement range of the user's eyes.  Can be updated at runtime.", () => 1.0f);

        public static ALXRModule qpm;

        static ModConfiguration _config;

        public static float EyeOpenExponent = 1.0f;
        public static float EyeWideMult = 1.0f;
        public static float EyeMoveMulti = 1.0f;

        public override string Name => "QuestPro4Resonite";
		public override string Author => "dfgHiatus & Geenz & Sinduy & Dante Tucker";
		public override string Version => "1.0.2";
		public override string Link => "https://github.com/sjsanjsrh/QuestPro4Resonite";
		public override void OnEngineInit()
		{
            _config = GetConfiguration();

            _config.OnThisConfigurationChanged += OnConfigurationChanged;

            new Harmony("net.dfgHiatus.QuestPro4Resonite").PatchAll();
		}

        [HarmonyPatch(typeof(InputInterface), MethodType.Constructor)]
        [HarmonyPatch(new Type[] { typeof(Engine) })]
        public class InputInterfaceCtorPatch
        {
            public static void Postfix(InputInterface __instance)
            {
                try
                {
                    qpm = new ALXRModule();

                    _ = qpm.Initialize(_config.GetValue(QuestProIP));

                    __instance.RegisterInputDriver(new EyeDevice());
                    __instance.RegisterInputDriver(new MouthDevice());

                    Engine.Current.OnShutdown += () => qpm.Teardown();
                }
                catch (Exception ex)
                {
                    Warn("Module failed to initiallize.");
                    Warn(ex.ToString());
                }
            }
        }

        private void OnConfigurationChanged(ConfigurationChangedEvent @event)
        {
            if (@event.Label == "NeosModSettings variable change") return;
            UniLog.Log($"Var changed! {@event.Label}");
            if (@event.Key == EyeOpennessExponent)
            {
                float openExp = 1.0f;
                if (@event.Config.TryGetValue(EyeOpennessExponent, out openExp))
                {
                    EyeOpenExponent = openExp;
                }
            }

            if (@event.Key == EyeOpennessExponent)
            {
                float wideMult = 1.0f;
                if (@event.Config.TryGetValue(EyeWideMultiplier, out wideMult))
                {
                    EyeWideMult = wideMult;
                }
            }

            if (@event.Key == EyeMovementMultiplier)
            {
                float moveMulti = 1.0f;
                if (@event.Config.TryGetValue(EyeMovementMultiplier, out moveMulti))
                {
                    EyeMoveMulti = moveMulti;
                }
            }
        }
    }
}
