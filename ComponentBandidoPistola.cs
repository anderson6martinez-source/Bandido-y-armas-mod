using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
    // El bandido tiene 1 slot de inventario con la Pistola9mm. El juego dibuja en su mano el item
    // que hay en ese slot (por su id) y el bandido dispara balas Bala9mm al objetivo que persigue.
    // SIN COMPILAR: los nombres de API salen de tus DLL 2.4. Revisa errores al compilar.
    public class ComponentBandidoPistola : Component, IUpdateable, IInventory
    {
        // ---- Ajustes ----
        private const float MinDistance = 3f;
        private const float MaxDistance = 18f;
        private const float ShotInterval = 1.2f;
        private const float BulletSpeed = 60f;
        private const float Spread = 0.03f;
        private const int MagazineSize = 12;
        private const float ReloadTime = 3f;

        private SubsystemProjectiles m_subsystemProjectiles;
        private SubsystemPickables m_subsystemPickables;
        private SubsystemTime m_subsystemTime;
        private ComponentCreature m_componentCreature;
        private ComponentChaseBehavior m_componentChase;
        private ComponentMiner m_componentMiner;
        private readonly System.Random m_random = new System.Random();

        private int m_slotValue;     // lo que hay en el slot (la pistola)
        private int m_slotCount;
        private int m_bulletValue;
        private int m_ammo = MagazineSize;
        private double m_nextShotTime;

        public int UpdateOrder { get { return 0; } }
        public float FloatUpdateOrder { get { return 0f; } }

        // ================= IInventory: 1 solo slot =================
        Project IInventory.Project { get { return Project; } }
        public int SlotsCount { get { return 1; } }
        public int VisibleSlotsCount { get { return 1; } set { } }
        public int ActiveSlotIndex { get { return 0; } set { } }

        public int GetSlotValue(int slotIndex) { return slotIndex == 0 ? m_slotValue : 0; }
        public int GetSlotCount(int slotIndex) { return (slotIndex == 0 && m_slotValue != 0) ? m_slotCount : 0; }
        public int GetSlotCapacity(int slotIndex, int value) { return 0; }          // nadie puede meter cosas
        public int GetSlotProcessCapacity(int slotIndex, int value) { return 0; }

        public void AddSlotItems(int slotIndex, int value, int count)
        {
            if (slotIndex == 0) { m_slotValue = value; m_slotCount = count; }
        }

        public void ProcessSlotItems(int slotIndex, int value, int count, int processCount,
                                     out int processedValue, out int processedCount)
        {
            processedValue = 0;
            processedCount = 0;
        }

        public int RemoveSlotItems(int slotIndex, int count) { return 0; }          // no se la pueden quitar del slot

        public void DropAllItems(Vector3 position)
        {
            // Al morir suelta la pistola como objeto recogible
            if (m_slotValue != 0 && m_subsystemPickables != null)
            {
                m_subsystemPickables.AddPickable(m_slotValue, 1, position, Vector3.Zero, null);
                m_slotValue = 0;
                m_slotCount = 0;
            }
        }

        // ================= Carga =================
        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
        {
            base.Load(valuesDictionary, idToEntityMap);
            m_subsystemProjectiles = Project.FindSubsystem<SubsystemProjectiles>(true);
            m_subsystemPickables = Project.FindSubsystem<SubsystemPickables>(true);
            m_subsystemTime = Project.FindSubsystem<SubsystemTime>(true);
            m_componentCreature = Entity.FindComponent<ComponentCreature>(true);
            m_componentChase = Entity.FindComponent<ComponentChaseBehavior>(true);
            m_componentMiner = Entity.FindComponent<ComponentMiner>(true);

            m_bulletValue = Terrain.MakeBlockValue(Bala9mmBlock.Index, 0, 0);

            // Pone la pistola en el slot y le dice al minero que use este inventario.
            // El modelo humano dibuja en la mano el item que el minero tiene activo.
            m_slotValue = Terrain.MakeBlockValue(Pistola9mmBlock.Index, 0, 0);
            m_slotCount = 1;
            m_componentMiner.Inventory = this;
        }

        // ================= Disparo =================
        public void Update(float dt)
        {
            // Por si el minero reinicio su inventario al cargar
            if (m_componentMiner.Inventory != this) m_componentMiner.Inventory = this;

            ComponentCreatureModel model = m_componentCreature.ComponentCreatureModel;
            ComponentCreature target = m_componentChase.Target;
            if (target == null || m_componentCreature.ComponentHealth.Health <= 0f)
            {
                model.AimHandAngleOrder = 0f;    // baja el brazo
                return;
            }

            Vector3 from = model.EyePosition;
            ComponentBody targetBody = target.ComponentBody;
            Vector3 to = targetBody.Position + new Vector3(0f, targetBody.StanceBoxSize.Y * 0.6f, 0f);

            float distance = Vector3.Distance(from, to);
            bool inRange = distance >= MinDistance && distance <= MaxDistance;

            model.AimHandAngleOrder = inRange ? 1.4f : 0f;   // levanta el brazo para apuntar
            if (!inRange) return;

            double now = m_subsystemTime.GameTime;
            if (now < m_nextShotTime) return;

            Vector3 dir = Vector3.Normalize(to - from);
            dir += new Vector3(
                ((float)m_random.NextDouble() - 0.5f) * 2f * Spread,
                ((float)m_random.NextDouble() - 0.5f) * 2f * Spread,
                ((float)m_random.NextDouble() - 0.5f) * 2f * Spread);
            dir = Vector3.Normalize(dir);

            m_subsystemProjectiles.FireProjectile(
                m_bulletValue, from + dir * 0.6f, dir * BulletSpeed, Vector3.Zero, m_componentCreature);

            m_ammo--;
            if (m_ammo <= 0)
            {
                m_ammo = MagazineSize;
                m_nextShotTime = now + ReloadTime;
            }
            else
            {
                m_nextShotTime = now + ShotInterval;
            }
        }
    }
}
