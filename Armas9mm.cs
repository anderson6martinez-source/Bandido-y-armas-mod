using System.Collections.Generic;
using Engine;
using Engine.Graphics;

namespace Game
{
    // SIN COMPILAR: los nombres de API salen de tus DLL 2.4. Revisa errores al compilar.
    // Assets que usa (ya estan dentro de Bandido24.scmod):
    //   Models/Pistola9mm + Textures/Items/Pistola9mm
    //   Models/Bala9mm    + Textures/Items/Bala9mm

    public class Pistola9mmBlock : Block
    {
        public const int Index = 220;   // CAMBIA el numero si ya lo usa otro bloque tuyo

        private readonly BlockMesh m_mesh = new BlockMesh();
        private Texture2D m_texture;

        public Pistola9mmBlock()
        {
            DefaultDisplayName = "Pistola 9mm";
            DefaultDescription = "Pistola semiautomatica de 9 milimetros. Usa balas de 9 mm.";
            DefaultCategory = "Weapons";
            MaxStacking = 1;
            IsPlaceable = false;
            DefaultMeleePower = 1f;
            DefaultIconViewScale = 1.6f;
        }

        public override void Initialize()
        {
            Model model = ContentManager.Get<Model>("Models/Pistola9mm");
            ModelMesh mesh = model.FindMesh("Pistola9mm", true);
            Matrix bone = BlockMesh.GetBoneAbsoluteTransform(mesh.ParentBone);
            m_mesh.AppendModelMeshPart(mesh.MeshParts[0], bone, false, false, false, false, Color.White);
            m_texture = ContentManager.Get<Texture2D>("Textures/Items/Pistola9mm");
            base.Initialize();
        }

        public override IEnumerable<int> GetCreativeValues()
        {
            yield return Terrain.MakeBlockValue(Index, 0, 0);
        }

        public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color,
                                       float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
        {
            BlocksManager.DrawMeshBlock(primitivesRenderer, m_mesh, m_texture, color, size, ref matrix, environmentData);
        }
    }

    public class Bala9mmBlock : Block
    {
        public const int Index = 221;   // CAMBIA el numero si ya lo usa otro bloque tuyo

        private readonly BlockMesh m_mesh = new BlockMesh();
        private Texture2D m_texture;

        public Bala9mmBlock()
        {
            DefaultDisplayName = "Bala 9mm";
            DefaultDescription = "Municion de 9 milimetros para la pistola.";
            DefaultCategory = "Weapons";
            MaxStacking = 40;
            IsPlaceable = false;
            DefaultProjectilePower = 25f;     // dano de cada bala (ajustalo a tu gusto)
            DisintegratesOnHit = true;        // la bala desaparece al pegar
            DefaultIconViewScale = 3f;
        }

        public override void Initialize()
        {
            Model model = ContentManager.Get<Model>("Models/Bala9mm");
            ModelMesh mesh = model.FindMesh("Bala9mm", true);
            Matrix bone = BlockMesh.GetBoneAbsoluteTransform(mesh.ParentBone);
            m_mesh.AppendModelMeshPart(mesh.MeshParts[0], bone, false, false, false, false, Color.White);
            m_texture = ContentManager.Get<Texture2D>("Textures/Items/Bala9mm");
            base.Initialize();
        }

        public override IEnumerable<int> GetCreativeValues()
        {
            yield return Terrain.MakeBlockValue(Index, 0, 0);
        }

        public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color,
                                       float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
        {
            BlocksManager.DrawMeshBlock(primitivesRenderer, m_mesh, m_texture, color, size, ref matrix, environmentData);
        }
    }
}
