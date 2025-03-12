using HarmonyLib;
using MelonLoader;
using UnityEngine;

[assembly: MelonInfo(typeof(StealthMuck.Core), "StealthMuck", "1.0.0", "ghfakegh", null)]
[assembly: MelonGame("Dani", "Muck")]

namespace StealthMuck
{
    public class Core : MelonMod
    {
        private PlayerStatus cachedPlayer = null;

        static bool StaminaToggleChecker = false;
        static bool HealthToggleChecker = false;
        static bool HungerToggleChecker = false;
        public static bool AttackSpeedToggleChecker = false; // Перенес в public static для доступа из патча

        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("✅ Чит загружен.");
            HarmonyInstance.PatchAll();
        }

        public override void OnUpdate()
        {
            if (cachedPlayer == null || cachedPlayer.gameObject == null)
            {
                cachedPlayer = FindLocalPlayer();

                if (cachedPlayer != null)
                {
                    LoggerInstance.Msg("✅ Объект игрока найден!");
                }
                else
                {
                    LoggerInstance.Warning("❗️ Игрок не найден, пробую снова...");
                }
            }

            if (Input.GetKeyDown(KeyCode.V))
            {
                AttackSpeedToggleChecker = !AttackSpeedToggleChecker;
                LoggerInstance.Msg(AttackSpeedToggleChecker
                    ? "⚔️ Ускоренная атака ВКЛ"
                    : "🐌 Ускоренная атака ВЫКЛ");
            }

            Stamina(cachedPlayer);
            Health(cachedPlayer);
            Hunger(cachedPlayer);
        }

        private void Health(PlayerStatus cachedPlayer)
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                HealthToggleChecker = !HealthToggleChecker;
                LoggerInstance.Msg(HealthToggleChecker
                    ? "❤️ Бесконечное здоровье ВКЛ"
                    : "💔 Бесконечное здоровье ВЫКЛ");
            }

            if (cachedPlayer != null && HealthToggleChecker)
            {
                cachedPlayer.hp = 100f;
            }
        }

        private void Stamina(PlayerStatus cachedPlayer)
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                StaminaToggleChecker = !StaminaToggleChecker;
                LoggerInstance.Msg(StaminaToggleChecker
                    ? "✅ Бесконечная стамина ВКЛ"
                    : "❌ Бесконечная стамина ВЫКЛ");
            }

            if (cachedPlayer != null && StaminaToggleChecker)
            {
                cachedPlayer.stamina = 100f;
            }
        }

        private void Hunger(PlayerStatus cachedPlayer)
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                HungerToggleChecker = !HungerToggleChecker;
                LoggerInstance.Msg(HungerToggleChecker
                    ? "🍗 Бесконечная еда ВКЛ"
                    : "🥩 Бесконечная еда ВЫКЛ");
            }

            if (cachedPlayer != null && HungerToggleChecker)
            {
                cachedPlayer.hunger = 100f;
            }
        }

        private PlayerStatus FindLocalPlayer()
        {
            foreach (var player in GameObject.FindObjectsOfType<PlayerStatus>())
            {
                if (player != null && player.stamina > 0)
                {
                    LoggerInstance.Msg($"✅ Игрок найден на объекте: {player.gameObject.name}");
                    return player;
                }
            }

            LoggerInstance.Warning("❗️ Не удалось найти объект игрока.");
            return null;
        }

        [HarmonyPatch(typeof(InventoryItem), "Copy")]
        public class Patch
        {
            private static float originalAttackSpeed;
            private static void Postfix(InventoryItem __instance)
            {
                originalAttackSpeed = __instance.attackSpeed; // Сохраняем оригинальную скорость атаки перед изменениями
                __instance.attackSpeed = Core.AttackSpeedToggleChecker ? 1000f : originalAttackSpeed;
            }
        }
    }
}